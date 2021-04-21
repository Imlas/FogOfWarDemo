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

    //Apparently RPCs cant be overloaded
    //[ClientRpc]
    //public void RPCSpawnVFX(NetworkIdentity networkIdentity, VFXType index)
    //{
    //    RPCSpawnVFX(networkIdentity, index, networkIdentity.gameObject.transform.position, networkIdentity.gameObject.transform.rotation);
    //}

    [ClientRpc]
    public void RPCSpawnVFX(NetworkIdentity networkIdentity, VFXType index, Vector3 _position, Quaternion _rotation)
    {
        GameObject vfxGO;
        Transform trans;
        //Debug.Log($"Incoming spawn from {_position}");

        switch (index)
        {
            case VFXType.TestCube:
                vfxGO = Instantiate(testCube, _position, _rotation);
                break;

            case VFXType.ARifleMuzzleFlash:
                trans = networkIdentity.gameObject.GetComponent<DudeController>().firePoint;
                vfxGO = Instantiate(aRifleMuzzleFlash, _position, _rotation);
                vfxGO.GetComponent<FollowTransform>().targetTrans = trans;
                break;

            case VFXType.ARifleBulletStreak:
                vfxGO = Instantiate(aRifleBulletStreak, _position, _rotation);
                break;

            case VFXType.ARifleBulletHit:
                vfxGO = Instantiate(aRifleBulletHit, _position, _rotation);

                break;
            default:
                Debug.Log("Unplanned case error");
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

