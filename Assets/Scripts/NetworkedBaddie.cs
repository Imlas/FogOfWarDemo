using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

public class NetworkedBaddie : NetworkBehaviour
{
    //Alternative action priority list layout
    // if currentTarget == null, get new target
    // if canAttack, then attack (checking for range, attack cooldown, and rotation)
    // if !canAttack, but is inRange, then rotate to face target
    // if !isInRange, then recalc path and move towards target (rechecking every reSeekDelay secs to recalc path)

    //NetworkedBaddie is an entirely server-based/controlled entity. Clients should not interract with it at all
    //So it turns out that this doens't become active until the server starts
    //NavMeshAgent navAgent;
    GameObject currentTarget;
    BaddiePathfinder baddiePathfinder;

    //For now, neither of these are used. Currently moveSpeed and turnRate are implemented directly into the navAgent component
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnRate;

    [SerializeField] private float attackRange;
    [SerializeField] private float stopDistance; //stop distance should be smaller than attackDistance

    //[SerializeField] private float reSeekDelay; //time in seconds between having the navAgent recalc a path to new destination
    //private float timeOfLastReSeek;

    [SerializeField] private float maxFireAngleDifference;
    [SerializeField] private float attackCooldown; //time in seconds in between attacks
    private float timeOfLastAttack;



    public override void OnStartServer()
    {
        base.OnStartServer();

        //Grab components as needed
        //navAgent = GetComponent<NavMeshAgent>();
        baddiePathfinder = GetComponent<BaddiePathfinder>();

        timeOfLastAttack = Time.time;
        //timeOfLastReSeek = 0f; //In this case, we want a unit to be able to seek instantly upon creation, but not attack


    }

    /// <summary>
    /// Returns the nearest GameObject from the list returned by BaddieManager.getPlayers()
    /// </summary>
    /// <returns></returns>
    private GameObject FindNearestPlayer()
    {
        GameObject nearestPlayer = null;

        float dist2Closest = Mathf.Infinity;
        foreach(GameObject player in BaddieManager.Instance.getPlayers())
        {
            if ((this.transform.position - player.transform.position).sqrMagnitude <= dist2Closest)
            {
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }

    /// <summary>
    /// Checks if the distance between this gameObject and currentTarget is less than attackRange.
    /// Will also return false if currentTarget is null.
    /// </summary>
    /// <returns></returns>
    private bool IsInRange()
    {
        if(currentTarget == null)
        {
            return false;
        }
        return (this.transform.position - currentTarget.transform.position).sqrMagnitude < attackRange * attackRange;
    }

    /// <summary>
    /// Simply checks if the current Time.time is later than timeOfLastAttack + attackCooldown.
    /// </summary>
    /// <returns></returns>
    private bool IsAttackOffCooldown()
    {
        return Time.time >= timeOfLastAttack + attackCooldown;
    }

    ///// <summary>
    ///// Checks if the current time is later than the last time we checked for pathfinding plus delay
    ///// </summary>
    ///// <returns></returns>
    //private bool IsReSeekOffCooldown()
    //{
    //    return Time.time >= timeOfLastReSeek + reSeekDelay;
    //}

    /// <summary>
    /// NOT YET IMPLEMENTED
    /// Checks if the difference of the angle between the facing of the unit and the target is less than maxFireAngleDifference.
    /// Also returns false if currentTarget is null.
    /// </summary>
    /// <returns></returns>
    private bool IsFacingTarget()
    {
        if(currentTarget == null)
        {
            return false;
        }
        return true; //Obviously there should be some math here comparing the difference of an angle with maxFireAngleDifference
    }



    private void AttackTarget()
    {
        //Debug.Log($"Attack! at {Time.time}");
        //Instantiate projectile, play attack animation of present, play sound effect
        timeOfLastAttack = Time.time;
    }

    private void TurnToTarget()
    {
        //Debug.Log($"Turning to face {currentTarget}");
        //Turns the turningPoint (which might be a child part, might be the main GO) towards currentTarget
    }

    public void ReSeekTarget()
    {
        ////Recalcs a path to the current target
        ////For now just sets the NavAgent destination and lets it handle everything
        //if (navAgent.pathPending || currentTarget == null)
        //{
        //    return;
        //}

        //navAgent.SetDestination(new Vector3(currentTarget.transform.position.x, 0, currentTarget.transform.position.z));

        //timeOfLastReSeek = Time.time;
        ////Debug.Log($"Reseek to position {currentTarget.transform.position} at time {Time.time}");
        if(currentTarget == null)
        {
            return;
        }
        baddiePathfinder.StartPathTo(currentTarget);
    }

    private void MoveToTarget()
    {
        //Moves towards current pathfinding point
        //For now this does nothing, since we're using the built-in agent self-movement
        //Once we transition to an independant pathfinder, this will move our unit towards the next pathfinding point (and away from nearby units? idk)
    }



    //Alternative action priority list layout
    // if currentTarget == null, get new target
    // if canAttack, then attack (checking for range, attack cooldown, and rotation)
    // if !canAttack, but is inRange, then rotate to face target
    // if !isInRange, then recalc path and move towards target (rechecking every reSeekDelay secs to recalc path)


    [ServerCallback]
    void Update()
    {
        //First, check if we have a target, if not then get the nearest player
        if(currentTarget == null)
        {
            currentTarget = FindNearestPlayer(); //Eventually baddies should be able to target player structures/etc, not just players themselves
        }

        if(IsAttackOffCooldown() && IsInRange() && IsFacingTarget())
        {
            AttackTarget();
        }
        else if(IsInRange() && !IsFacingTarget())
        {
            TurnToTarget();
        }
        else if (!IsInRange())
        {
            ReSeekTarget();
        }

        //MoveToTarget();
    }

    [ServerCallback]
    void OnCollisionEnter(Collision col)
    {
        

    }

    //[ContextMenu(("Send Path to Debug"))]
    //private void PathToDebug()
    //{
    //    if(navAgent == null)
    //    {
    //        Debug.Log("agent is somehow null. idk");
    //        return;
    //    }
    //    Debug.Log($"Agent has a path? {navAgent.hasPath}. End point is: {navAgent.pathEndPosition}.");
    //    foreach( Vector3 point in navAgent.path.corners){
    //        Debug.Log($"Point: {point}");
    //    }
    //}

}
