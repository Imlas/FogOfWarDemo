using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ImlaNetworkManager : NetworkManager
{
    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);

    //    Debug.Log("I connected to a server");
    //}

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        DudeController player = conn.identity.gameObject.GetComponent<DudeController>();

        player.SetDisplayName($"Player {numPlayers}");
        BaddieManager.Instance.AddPlayer(player.gameObject);

        Debug.Log($"New player spawned at {player.gameObject.transform.position}");


        //MyNetworkPlayer netPlayer = conn.identity.gameObject.GetComponent<MyNetworkPlayer>();
        //netPlayer.SetDisplayName($"Player {numPlayers}");

        //Color randColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        //netPlayer.SetDisplayColor(randColor);
        //Debug.Log($"There are now {numPlayers} players");
    }

    

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //Stop spawning baddies if there's no more players
        if(numPlayers == 0)
        {
            //blah
        }

        DudeController player = conn.identity.gameObject.GetComponent<DudeController>();
        BaddieManager.Instance.RemovePlayer(player.gameObject);

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);


    }
}
