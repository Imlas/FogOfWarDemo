using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedBaddie : NetworkBehaviour
{

    //NetworkedBaddie is an entirely server-based/controlled entity. Clients should not interract with it at all

    public override void OnStartServer()
    {
        base.OnStartServer();

        //Grab components as needed
        

        //Find the player, set navagent destination

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
