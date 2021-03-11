using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class NetworkPlayerMovement : NetworkBehaviour
{
    private Camera cam;
    private NavMeshAgent navAgent;

    #region Client
    //The "network" version of Start
    public override void OnStartAuthority()
    {
        cam = Camera.main;
        navAgent = GetComponent<NavMeshAgent>();
    }

    //Will only run on clients, not on the server
    [ClientCallback]
    private void Update()
    {
        //We only want to be running this for the "player" that the client owns
        if (!hasAuthority) { return; }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CmdMove(hit.point);
            }
        }
    }
    #endregion

    #region Server
    /// <summary>
    /// Requests the server to set the player's NavMeshAgent to the specified movePoint.
    /// </summary>
    /// <param name="movePoint"></param>
    [Command]
    private void CmdMove(Vector3 movePoint)
    {
        //Some validation
        if(!NavMesh.SamplePosition(movePoint, out NavMeshHit navHit, 1f, NavMesh.AllAreas)){
            return;
        }

        //Set as a destination
        navAgent.SetDestination(navHit.position);
    }
    #endregion
}
