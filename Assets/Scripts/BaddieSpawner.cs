using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaddieSpawner : NetworkBehaviour
{
    //BaddieSpawner is an entirely server-based/controlled entity. Clients should not interract with it at all
    [SerializeField] private GameObject baddieProto;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private float timeBetweenSpawns;
    private float timeOfLastSpawn;
    [SerializeField] private int maxBaddies;

    public bool isSpawningBaddies = true;

    public override void OnStartServer()
    {
        base.OnStartServer();

        //Debug.Log("OnStartServer called for baddie spawner.");
        //StartCoroutine(nameof(SpawnBaddiesConstantly), 2f);
        timeOfLastSpawn = Time.time;

        //For testing, spawn one immediately
        SpawnBaddie();

    }

    //IEnumerator SpawnBaddiesConstantly(float delay)
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(delay);
    //        SpawnBaddie();
    //    }
    //}

    [Server]
    private void SpawnBaddie()
    {
        //Debug.Log("Spawn!");
        GameObject newBaddie = Instantiate(baddieProto, spawnTransform.position, Quaternion.identity);
        NetworkServer.Spawn(newBaddie);

        BaddieManager.Instance.AddBaddie(newBaddie);

        timeOfLastSpawn = Time.time;
    }


    [ServerCallback]
    void Update()
    {
        if(maxBaddies > BaddieManager.Instance.getBaddies().Count && Time.time >= timeOfLastSpawn + timeBetweenSpawns && isSpawningBaddies)
        {
            SpawnBaddie();
        }
    }

    [ContextMenu(("Spawn New Baddie"))]
    private void EditorSpawnBaddie()
    {
        if(baddieProto == null)
        {
            return;
        }

        SpawnBaddie();
    }
}
