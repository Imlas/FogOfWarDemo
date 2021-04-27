using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Mirror;

public class BaddiePathfinder : NetworkBehaviour
{
    Seeker seeker;
    Rigidbody rb;

    Path path;
    int currentWaypoint;
    //public bool reachedEndOfPath;

    [SerializeField] GameObject targetGO;
    [SerializeField] public bool isStationary = false;

    [SerializeField] float pathStrength = 1f;
    [SerializeField] float avoidStrength = 0.5f;
    [SerializeField] float avoidRadius = 1f;
    [SerializeField] LayerMask baddieLayerMask;

    [SerializeField] LayerMask LoSBlockerMask;
    [SerializeField] float speed = 5.5f;
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float nextWaypointMinDistance = 0.5f;
    [SerializeField] float rePathRate = 0.5f;
    float lastRepath = Mathf.NegativeInfinity;

    [SerializeField] float finalStopDistance = 3f;

    private Vector3 currentVelocity;
    //[SerializeField] private int repathCount = 0;
    

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();

        //repathCount = 0;
    }

    [Server]
    public void StartPathTo(GameObject _targetGO)
    {
        //If the target hasn't changed, and we still haven't waited long enough, then chill a bit
        if(_targetGO == targetGO && Time.time < (lastRepath + rePathRate))
        {
            return;
        }


        if(!seeker.IsDone())
        {
            return; //In this case, the target hasn't changed, and we're still looking for a path, so chill out for a tick
        }

        targetGO = _targetGO;
        seeker.StartPath(this.transform.position, targetGO.transform.position, OnPathComplete);
        lastRepath = Time.time;

        //repathCount++;
    }

    [Server]
    private void OnPathComplete(Path p)
    {
        //There's some stuff with pooling we can do here, check the documentation
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    //Currently unused?
    [Server]
    void RotateTowardsTarget()
    {
        var targetDir = targetGO.transform.position - this.transform.position;
        var forward = this.transform.forward;
        var localTarget = transform.InverseTransformPoint(targetGO.transform.position);

        float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        Vector3 eulerAngleVecl = new Vector3(0f, angle, 0f);
        Quaternion deltaRot = new Quaternion();
        deltaRot = Quaternion.Euler(eulerAngleVecl);
        rb.MoveRotation(rb.rotation * deltaRot);
    }

    [ServerCallback]
    void Update()
    {
        //If we don't have a target, then just exit out
        if(targetGO == null || isStationary)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        //If enough time has passed, and we aren't still getting a new path, get a new path
        if(Time.time > (lastRepath + rePathRate) && seeker.IsDone())
        {
            //Debug.Log($"Last Repath Time {lastRepath}. Current Time: {Time.time}");
            lastRepath = Time.time;
            StartPathTo(targetGO);
        }

        //If we don't have a path (either still getting one, or we've reached the end, then rotate towards the targetGO and then exit out
        if(path == null)
        {
            Vector3 targetDir = targetGO.transform.position - this.transform.position;
            float rotAngle = -1 * (Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg - 90f);
            //Gonna be super honest - no idea why I have to negative the angle I get from this. But it works.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, rotAngle, 0f), turnSpeed * Time.deltaTime);
            //Debug.Log($"No path, but rotating towards target GO at {targetGO.transform.position}. Rotation angle of {rotAngle}");

            return;
        }

        //Check if we're close enough to the final point and then within LoS of it. If so then just chill.
        Vector3 nonYTarget = new Vector3(targetGO.transform.position.x - this.transform.position.x, 0, targetGO.transform.position.z - this.transform.position.z);
        if(nonYTarget.sqrMagnitude < finalStopDistance * finalStopDistance) //Doing a cheaper non-sqroot check
        {
            if(!Physics.Raycast(this.transform.position, nonYTarget, nonYTarget.magnitude, LoSBlockerMask))
            {
                //In this case we're "close enough" to the target, and nothing (on LoSBlocker) is blocking us, so we're done moving this frame.
                //Debug.Log($"I'm close enough to my target.");
                currentVelocity = Vector3.zero; //We could modify this later to coast to a stop a bit
                path = null;
                return;
                //Maybe some reachedEndOfPath callback here if needed, for now we'll just null the path
            }
        }

        //Next, see if we're close enough to the next point, if so then increment the currentWaypoint
        //An improvement for this would be to place this in a seperate method, then call it again at the end to see if we can skip over multiple waypoints at once
        Vector3 nonYDiff = new Vector3(this.transform.position.x - path.vectorPath[currentWaypoint].x, this.transform.position.z - path.vectorPath[currentWaypoint].z); //
        if(nonYDiff.sqrMagnitude <= nextWaypointMinDistance * nextWaypointMinDistance)
        {
            //Check if there's more waypoints, if so then ++
            if(currentWaypoint + 1 < path.vectorPath.Count)
            {
                currentWaypoint++;
            }
            else
            {
                //Debug.Log($"Reached the *end* of the path. Target is at: {targetGO.transform.position}. I am at: {this.transform.position}");
                currentVelocity = Vector3.zero;
                path = null;
                return;
                //Maybe some reachedEndOfPath callback here if needed, for now we'll just null the path
            }
        }

        Vector3 pathDir;
        //At this point, if we can directly see the objective, just go straight towards it
        if (!Physics.Raycast(this.transform.position, nonYTarget, nonYTarget.magnitude, LoSBlockerMask))
        {
            pathDir = nonYTarget.normalized;
            Debug.Log($"Direct LoS to point {nonYTarget} which is {nonYTarget.magnitude}. Moving straight to target");
        }
        else
        {
            //Otherwise, head down the path
            pathDir = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;
            Debug.Log($"No direct LoS. Moving along path");

        }


        //Find nearby other baddies so we can move away from them
        Vector3 avoidDir = Vector3.zero;

        Collider[] otherBaddieCol = Physics.OverlapSphere(this.transform.position, avoidRadius, baddieLayerMask);
        Debug.Log($"{otherBaddieCol.Length} baddies found nearby.");
        foreach(Collider baddieCol in otherBaddieCol)
        {
            if (baddieCol.CompareTag(TagManager.baddieTag) && !baddieCol.transform.IsChildOf(this.gameObject.transform)) //Double checking the tag - not really needed
            {
                avoidDir += (this.transform.position - baddieCol.gameObject.transform.position);
                Debug.Log($"Baddie found at {baddieCol.gameObject.transform.position}. New avoid vector: {avoidDir}");
            }
        }
        avoidDir = new Vector3(avoidDir.x, 0f, avoidDir.z);


        //Combine the direction of the goal, and our avoidance direction
        currentVelocity = ((pathDir * pathStrength) + (avoidDir.normalized * avoidStrength)).normalized * speed;

        //currentVelocity = pathDir * speed;
        //Debug.Log($"Pos: {this.transform.position}, Point:{path.vectorPath[currentWaypoint]} {currentWaypoint}");

        //Next we rotate towards the direction of pathDir
        //float angle = -1 * (Mathf.Atan2(pathDir.z, pathDir.x) * Mathf.Rad2Deg - 90f);
        float angle = -1 * (Mathf.Atan2(currentVelocity.z, currentVelocity.x) * Mathf.Rad2Deg - 90f);

        //Gonna be super honest - no idea why I have to negative the angle I get from this. But it works.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, angle, 0f), turnSpeed * Time.deltaTime);
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        //currentVelocity is set in Update, just assigning it here in FixedUpdate as is (probably?) appropriate
        rb.velocity = currentVelocity;
        //rb.MovePosition(transform.position + currentVelocity * Time.deltaTime);

    }
}
