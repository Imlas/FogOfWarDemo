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
    public bool reachedEndOfPath;

    [SerializeField] GameObject targetGO;

    [SerializeField] LayerMask LoSBlockerMask;
    [SerializeField] float speed = 5.5f;
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
        if(Time.time < (lastRepath + rePathRate))
        {
            return;
        }


        if(_targetGO == targetGO && !seeker.IsDone())
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

    [ServerCallback]
    void Update()
    {
        //If we don't have a target, then just exit out
        if(targetGO == null)
        {
            return;
        }

        //If enough time has passed, and we aren't still getting a new path, get a new path
        if(Time.time > (lastRepath + rePathRate) && seeker.IsDone())
        {
            //Debug.Log($"Last Repath Time {lastRepath}. Current Time: {Time.time}");
            lastRepath = Time.time;
            StartPathTo(targetGO);
        }

        //If we don't have a path (either still getting one, or we've reached the end, then exit out
        if(path == null)
        {
            return;
        }

        //Check if we're close enough to the final point and then within LoS of it. If so then just chill.
        Vector3 nonYTarget = new Vector3(targetGO.transform.position.x - this.transform.position.x, 0, targetGO.transform.position.z - this.transform.position.z);
        if(nonYTarget.sqrMagnitude < finalStopDistance * finalStopDistance) //Doing a cheaper non-sqroot check
        {
            if(!Physics.Raycast(this.transform.position, nonYTarget, nonYTarget.magnitude, LoSBlockerMask))
            {
                //In this case we're "close enough" to the target, and nothing (on LoSBlocker) is blocking us, so we're done moving this frame.
                Debug.Log($"I'm close enough to my target.");
                currentVelocity = Vector3.zero; //We could modify this later to coast to a stop a bit
                path = null;
                return;
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
                Debug.Log($"Reached the *end* of the path. Target is at: {targetGO.transform.position}. I am at: {this.transform.position}");
                currentVelocity = Vector3.zero;
                path = null;
                return;
                //Maybe some reachedEndOfPath callback here if needed, for now we'll just null the path
            }
        }

        //So now we just need to move towards the currentWaypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;

        currentVelocity = dir * speed;
        //Debug.Log($"Pos: {this.transform.position}, Point:{path.vectorPath[currentWaypoint]} {currentWaypoint}");
        //Also need to set the rotation somewhere


    }

    [ServerCallback]
    private void FixedUpdate()
    {
        rb.velocity = currentVelocity;
        //rb.MovePosition(transform.position + currentVelocity * Time.deltaTime);

    }
}
