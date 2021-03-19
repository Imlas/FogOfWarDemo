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

    [SerializeField] float speed = 5.5f;
    [SerializeField] float nextWaypointMinDistance = 0.5f;
    [SerializeField] float rePathRate = 0.5f;
    float lastRepath = Mathf.NegativeInfinity;

    private Vector3 currentVelocity;
    

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
    }

    [Server]
    public void StartPathTo(GameObject _targetGO)
    {
        if(_targetGO == targetGO && !seeker.IsDone())
        {
            return; //In this case, the target hasn't changed, and we're still looking for a path, so chill out for a tick
        }

        targetGO = _targetGO;
        seeker.StartPath(this.transform.position, targetGO.transform.position, OnPathComplete);
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
    // Update is called once per frame
    void Update()
    {
        if(targetGO == null)
        {
            return;
        }

        //If enough time has passed, get a new path
        if(Time.time > (lastRepath + rePathRate) && seeker.IsDone())
        {
            lastRepath = Time.time;
            StartPathTo(targetGO);
        }


        //If we don't have a path, just exit out
        if (path == null)
        {
            return;
        }

        reachedEndOfPath = false;

        float distanceSqrToNextPoint;
        while (true)
        {
            //We want to find the difference in distance, discarding the y axis
            Vector3 nonYDiff = new Vector3(this.transform.position.x - path.vectorPath[currentWaypoint].x, this.transform.position.z - path.vectorPath[currentWaypoint].z); //

            distanceSqrToNextPoint = nonYDiff.sqrMagnitude;

            ///Debug.Log($"SqrDist {distanceSqrToNextPoint}  MinDistSqr: {nextWaypointMinDistance * nextWaypointMinDistance}");

            if((nextWaypointMinDistance * nextWaypointMinDistance) >= distanceSqrToNextPoint)
            {
                //Check if there's another waypoint still
                if(currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {

                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        Vector3 dir = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;

        currentVelocity = dir * speed;
        //Debug.Log($"Pos: {this.transform.position}, Point:{path.vectorPath[currentWaypoint]} {currentWaypoint}");
        //Also need to set the rotation somewhere

        //rb.velocity = velocity; //This should 100% get worked into a fixed update or some shit

    }

    [ServerCallback]

    private void FixedUpdate()
    {
        rb.velocity = currentVelocity;
        //rb.MovePosition(transform.position + currentVelocity * Time.deltaTime);

    }
}
