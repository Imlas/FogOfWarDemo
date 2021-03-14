using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaddieSpawner : NetworkBehaviour
{
    //BaddieSpawner is an entirely server-based/controlled entity. Clients should not interract with it at all
    [SerializeField] private GameObject baddieProto;
    [SerializeField] private Transform spawnTransform;

    public override void OnStartServer()
    {
        base.OnStartServer();

        //Debug.Log("OnStartServer called for baddie spawner.");
        StartCoroutine(nameof(SpawnBaddiesConstantly), 2f);

    }

    IEnumerator SpawnBaddiesConstantly(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            SpawnBaddie();
        }
    }

    private void SpawnBaddie()
    {
        Debug.Log("Spawn!");
        GameObject newBaddie = Instantiate(baddieProto, spawnTransform);
        NetworkServer.Spawn(newBaddie);

        BaddieManager.Instance.AddBaddie(newBaddie);

    }




    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
