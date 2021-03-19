using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BaddieManager : NetworkBehaviour
{
    //This manager keeps track of where/how many players, how many baddies, triggers baddie spawners as needed
    //

    private static BaddieManager _instance;

    public static BaddieManager Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> baddies;
    [SerializeField] private List<GameObject> players;

    //Should probably have the tags pull from a centralized tag manager or some shit, idk
    public string playerTag;
    public string baddieTag;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            baddies.Clear();
            players.Clear();
        }
    }

    public void AddBaddie(GameObject newBaddie)
    {
        //Should maybe check if newBaddie has a component of type baddie
        baddies.Add(newBaddie);
    }

    public List<GameObject> getBaddies()
    {
        return baddies;
    }

    public void RemoveBaddie(GameObject oldBaddie)
    {
        baddies.Remove(oldBaddie);
    }

      public void AddPlayer(GameObject newPlayer)
    {
        //Should maybe check if newPlayer has a component of type player
        players.Add(newPlayer);
    }

    public List<GameObject> getPlayers()
    {
        return players;
    }


    public void RemovePlayer(GameObject oldPlayer)
    {
        players.Remove(oldPlayer);
    }

}
