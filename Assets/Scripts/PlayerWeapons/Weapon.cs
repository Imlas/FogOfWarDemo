using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//Note, we make this a network behavior, since it needs to spawn stuff down the road
public class Weapon : NetworkBehaviour
{
    //None of these should need to be syncvars, since you're never firing other player's guns/etc. Nor is most of it ever being changed dynamically

    [SerializeField] public bool UsesAmmo { get;}

    [SerializeField] public int MaxClipAmmo { get;}
    [SerializeField] public int CurrentClipAmmo { get; protected set; }

    [SerializeField] public int MaxReserveAmmo { get; set; } //Note, clip ammo is seperate from reserve ammo (ie. you can have clip ammo (bullets in gun), but no reserve ammo (spare bullets)
    [SerializeField] public int CurrentReserveAmmo { get; protected set; }

    [SerializeField] public bool IsAutomatic { get; } //True if you can "hold down" fire and keep firing. False if "fire" must be released and re-pressed to fire next. Not super sure where this get checked yet
    [SerializeField] public float FireRate { get; } //Shots per sec (so the inverse is num secs between shots)
    protected float timeLastFired = Mathf.NegativeInfinity;

    [SerializeField] public float Damage { get; } //Per bullet, or per seccond for lasers

    [SerializeField] public float Range { get; } //The max distance a projectile can travel, or the length of a laser

    [SerializeField] public float SpreadAngle { get; }  // the potential max difference +/- from the intended shot angle
    [SerializeField] public float ADSSpreadAngle { get; }  // a decreased spread while "aiming down sights"

    //This is here for an easy check so that the player (clients) aren't constantly spamming commands. Probably. Idk.
    public bool CanShoot()
    {
        if (CurrentClipAmmo == 0 || Time.time < timeLastFired + (1 / FireRate))
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
        //Should probably have this throw an exception, since it shouldn't ever be called
    }

    [Command]
    public virtual void CmdReload()
    {
        Debug.Log("Base Reload");
        //Should probably have this throw an exception, since it shouldn't ever be called
    }

}
