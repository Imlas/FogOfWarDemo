using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetworkedBaddie : NetworkBehaviour
{
    //Basic loop (for now - later this will just be one of several "ai types")
    // baddie spawns, finds nearest player, sets player current position as nav destination
    // every *reSeekDelay* seconds, will update player's position as destination
    //  stop the update when distance to player < stop distance (which should be smaller than attack distance)
    //  cycle through attacking player, then check if in-distance, move if needed and repeat
    //  if, on attack, the player is gone (dead), then find another target?

    //NetworkedBaddie is an entirely server-based/controlled entity. Clients should not interract with it at all
    //So it turns out that this doens't become active until the server starts
    NavMeshAgent navAgent;
    GameObject currentTarget;

    [SerializeField] private float attackDistance;
    [SerializeField] private float stopDistance; //stop distance should be smaller than attackDistance
    [SerializeField] private float reSeekDelay;


    public override void OnStartServer()
    {
        base.OnStartServer();

        //Grab components as needed
        navAgent = GetComponent<NavMeshAgent>();


        //Find the player, set navagent destination
        currentTarget = findNearestPlayer();
        SeekTarget();
        StartCoroutine(nameof(ReSeekTarget), reSeekDelay);

    }

    private GameObject findNearestPlayer()
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

    private void SeekTarget()
    {
        navAgent.SetDestination(currentTarget.transform.position);
        //Debug.Log($"Setting navagent target position for {currentTarget.transform.position}");
    }

    IEnumerator ReSeekTarget(float delay)
    {
        while ((this.transform.position - currentTarget.transform.position).sqrMagnitude > stopDistance)
        {
            yield return new WaitForSeconds(delay);
            SeekTarget();
        }

        AttackTarget();
    }

    IEnumerator SimpleDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void AttackTarget()
    {
        Debug.Log("Attack!");
        //Do the attack
        //Delay for some time (half of attack c/d?)
        //StartCoroutine(nameof(SimpleDelay), 1f);
        //Check if target is dead, if yes then find new target
        //Check if in range, if not then path to target
        //AttackTarget();
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        
    }

    [ServerCallback]
    void OnCollisionEnter(Collision col)
    {
        

    }

}
