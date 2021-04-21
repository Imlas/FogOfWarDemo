using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class VFXSpawner : NetworkBehaviour
{
    private static VFXSpawner _instance;

    public static VFXSpawner Instance { get { return _instance; } }

    [SerializeField] private GameObject testCube;
    [SerializeField] private GameObject aRifleMuzzleFlash;
    [SerializeField] private GameObject aRifleBulletStreak;
    [SerializeField] private GameObject aRifleBulletHit;




    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    [ClientRpc]
    public void RPCSpawnVFX(NetworkIdentity networkIdentity, VFXType index)
    {
        //Need to add another function parameter for arbitrary spawn transforms. Maybe just have an optional position/rotation?
        GameObject vfxGO;
        Transform trans;
        Debug.Log($"Incoming spawn from {networkIdentity.gameObject.transform.position}");

        switch (index)
        {
            case VFXType.TestCube:
                vfxGO = Instantiate(testCube, networkIdentity.gameObject.transform.position, networkIdentity.gameObject.transform.rotation);
                break;

            case VFXType.ARifleMuzzleFlash:
                trans = networkIdentity.gameObject.GetComponent<DudeController>().firePoint;
                vfxGO = Instantiate(aRifleMuzzleFlash, trans.position, trans.rotation);
                vfxGO.GetComponent<FollowTransform>().targetTrans = trans;
                break;

            case VFXType.ARifleBulletStreak:
                trans = networkIdentity.gameObject.GetComponent<DudeController>().firePoint;
                vfxGO = Instantiate(aRifleBulletStreak, trans.position, trans.rotation);
                break;

            case VFXType.ARifleBulletHit:
                //Uhh... for some reason I can't throw an exception here. Likely b/c of the switch?
                //throw new NotImplementedException();
                Debug.Log("NotImplemented Switch case in VFXSpawner!");
                break;
        }
    }
}



public enum VFXType
{
    TestCube,
    ARifleMuzzleFlash,
    ARifleBulletStreak,
    ARifleBulletHit
}

