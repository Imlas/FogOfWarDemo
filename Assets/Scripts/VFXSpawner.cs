using System;
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
    [SerializeField] private GameObject baddieAttackWarmup;
    [SerializeField] private GameObject baddieAttackFlare;




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

    //Apparently RPCs cant be overloaded - this is dumb
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

            case VFXType.BaddieAttackWarmup:
                trans = networkIdentity.gameObject.GetComponent<NetworkedBaddie>().firePoint;
                vfxGO = Instantiate(baddieAttackWarmup, _position, _rotation);
                vfxGO.GetComponent<FollowTransform>().targetTrans = trans;
                break;

            case VFXType.BaddieAttackFlare:
                trans = networkIdentity.gameObject.GetComponent<NetworkedBaddie>().firePoint;
                vfxGO = Instantiate(baddieAttackFlare, _position, _rotation);
                vfxGO.GetComponent<FollowTransform>().targetTrans = trans;
                break;

            default:
                Debug.Log("Unplanned case error for RPCSpawnVFX");
                break;
        }
    }

    [ClientRpc]
    public void RPCSpawnVFXwTarget(NetworkIdentity networkIdentity, VFXType index, Vector3 _position, Quaternion _rotation, Vector3 _targetPosition)
    {
        GameObject vfxGO;
        //Transform trans;

        switch (index)
        {
            case VFXType.ARifleBulletStreak:
                //Debug.Log($"Streak starting at: {_position} going to {_targetPosition}");
                vfxGO = Instantiate(aRifleBulletStreak, _position, _rotation);
                vfxGO.GetComponent<GenericBullet>().SetTarget(_targetPosition);
                break;

            default:
                Debug.Log("Unplanned case error for RPCSpawnVFXwTarget");
                break;
        }

    }

    /* From https://answers.unity.com/questions/1582657/how-do-i-delay-instantiating-a-prefab.html
     * Usage will be the following
     *  StartCoroutine(Timeout(
     () => {
         //This will be the code executed
         Instantiate(foo);
     }, 2f));
    */
    public static IEnumerator Timeout(Action action, float time)
    {
        //yield return new WaitForSecondsRealtime(time);
        yield return new WaitForSeconds(time);

        action();
    }

}




public enum VFXType
{
    TestCube,
    ARifleMuzzleFlash,
    ARifleBulletStreak,
    ARifleBulletHit,
    BaddieAttackWarmup,
    BaddieAttackFlare
}

