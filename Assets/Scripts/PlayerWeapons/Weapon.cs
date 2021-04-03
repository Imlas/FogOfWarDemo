using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//Note, we make this a network behavior, since it needs to spawn stuff down the road
//Fun fact: Making it a networkBehavior, which is a MonoBehavior, means this *must* exist as a component on a GameObject
public class Weapon : NetworkBehaviour
{
    //None of these should need to be syncvars, since you're never firing other player's guns/etc. Nor is most of it ever being changed dynamically

    [SerializeField] protected bool usesAmmo;

    [SerializeField] protected int maxClipAmmo;
    [SerializeField] protected int currentClipAmmo;

    [SerializeField] protected int maxReserveAmmo; //Note, clip ammo is seperate from reserve ammo (ie. you can have clip ammo (bullets in gun), but no reserve ammo (spare bullets)
    [SerializeField] protected int currentReserveAmmo;

    [SerializeField] protected bool isAutomatic; //True if you can "hold down" fire and keep firing. False if "fire" must be released and re-pressed to fire next. Not super sure where this get checked yet
    [SerializeField] protected float fireRate; //Shots per sec (so the inverse is num secs between shots)
    protected float timeLastFired = Mathf.NegativeInfinity;

    [SerializeField] protected float damage; //Per bullet, or per seccond for lasers

    [SerializeField] protected float range; //The max distance a projectile can travel, or the length of a laser

    [SerializeField] protected float spreadAngle;  // the potential max difference +/- from the intended shot angle
    [SerializeField] protected float adsSpreadAngle;  // a decreased spread while "aiming down sights"

    #region Getters
    //The big wall of getters. Not super sure if I like this style but we're tryin' it.
    //Also not sure if there's any huge benefit over just setting all of the fields public
    public bool UsesAmmo { get => usesAmmo;}
    public int MaxClipAmmo { get => maxClipAmmo; }
    public int CurrentClipAmmo { get => currentClipAmmo; }
    public int MaxCmaxReserveAmmolipAmmo { get => maxReserveAmmo; }
    public int CurrentReserveAmmo { get => currentReserveAmmo; }
    public bool IsAutomatic { get => isAutomatic; }
    public float FireRate { get => fireRate; }
    public float TimeLastFired { get => timeLastFired; }
    public float Damage { get => damage; }
    public float Range { get => range; }
    public float SpreadAngle { get => spreadAngle; }
    public float AdsSpreadAngle { get => adsSpreadAngle; }
    #endregion


    //This is here for an easy check so that the player (clients) aren't constantly spamming commands (which is supposedly bad). Probably. Idk.
    public bool CanShoot()
    {
        if (currentClipAmmo == 0 || Time.time < timeLastFired + (1 / fireRate))
        {
            //No ammo in clip or not enough time since last shot
            return false;
        }

        //Maybe add in a check for recent weapon swaps or the player being stunned? Some of that would belong elsewhere, though.

        return true;
    }

    [Command]
    public virtual void CmdShoot()
    {
        Debug.Log("Base Shoot");
        throw new NotImplementedException();
    }

    [Command]
    public virtual void CmdReload()
    {
        Debug.Log("Base Reload");
        throw new NotImplementedException();
    }

    //Note, this is for adding ammo to the reserveAmmo (ie. pick up more ammo)
    public virtual void AddReserveAmmo(int numAmmo)
    {
        Debug.Log("Base AddAmmo");
        throw new NotImplementedException();
    }

}
