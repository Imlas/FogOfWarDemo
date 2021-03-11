using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBasicNetworkManager : NetworkManager
{
    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);

    //    Debug.Log("I connected to a server");
    //}

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);


        MyNetworkPlayer netPlayer = conn.identity.gameObject.GetComponent<MyNetworkPlayer>();
        netPlayer.SetDisplayName($"Player {numPlayers}");

        Color randColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        netPlayer.SetDisplayColor(randColor);
        //Debug.Log($"There are now {numPlayers} players");
    }
}
