using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetworkedBaddie : NetworkBehaviour
{

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
        SeekTarget(); //We'll need a way to do this regularly (every ~1s?) to account for the player moving

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
