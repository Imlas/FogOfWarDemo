using System;
using System.Collections;
using System.Collections.Generic;
//using Mirror;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //None of these should need to be syncvars, since you're never firing other player's guns/etc. Nor is most of it ever being changed dynamically
    [SerializeField] protected string weaponName;

    [SerializeField] protected bool usesAmmo; //Could be false, but will need UI support, different reload/etc.

    [SerializeField] protected WeaponShotType weaponShotType; //Enum

    [SerializeField] protected int maxClipAmmo;
    [SerializeField] protected int currentClipAmmo;

    [SerializeField] protected int maxReserveAmmo; //Note, clip ammo is seperate from reserve ammo (ie. you can have clip ammo (bullets in gun), but no reserve ammo (spare bullets)
    [SerializeField] protected int currentReserveAmmo;

    [SerializeField] protected bool isAutomatic; //True if you can "hold down" fire and keep firing. False if "fire" must be released and re-pressed to fire next. Not super sure where this get checked yet
    [SerializeField] protected float fireRate; //Shots per sec (so the inverse is num secs between shots)
    [SerializeField] protected int shotsPerBurst = 1; //The number of shots per "trigger pull". Most weapons will be 1. Burst fire will be higher. Also used for number of shotgun "pellets"
    [SerializeField] protected float burstShotTime; //Time in between shots of a single burst
    protected float timeLastFired = Mathf.NegativeInfinity;

    [SerializeField] protected float damage; //Per bullet/pellet, or per seccond for lasers

    [SerializeField] protected float range; //The max distance a projectile can travel, or the length of a laser

    [SerializeField] protected float spreadAngle;  // the potential max difference +/- from the intended shot angle
    [SerializeField] protected float adsSpreadAngle;  // a decreased spread while "aiming down sights"

    [SerializeField] protected GameObject muzzleFlashGFX; //The gfx that is instantiated at the fire point and aligned with the fire point
    [SerializeField] protected GameObject bulletGFX; //The gfx that is instantiated at the fire point and aligned with the actual shot direction

    [SerializeField] protected VFXType muzzleVFXType;
    [SerializeField] protected VFXType streakVFXType;
    [SerializeField] protected VFXType hitVFXType;




    #region Getters
    //The big wall of getters. Not super sure if I like this style but we're tryin' it.
    //Also not sure if there's any huge benefit over just setting all of the fields public
    public string WeaponName { get => weaponName; }
    public bool UsesAmmo { get => usesAmmo; }
    public WeaponShotType  GetWeaponShotType {get => weaponShotType;}
    public int MaxClipAmmo { get => maxClipAmmo; }
    public int CurrentClipAmmo { get => currentClipAmmo; }
    public int MaxCmaxReserveAmmolipAmmo { get => maxReserveAmmo; }
    public int CurrentReserveAmmo { get => currentReserveAmmo; }
    public bool IsAutomatic { get => isAutomatic; }
    public float FireRate { get => fireRate; }
    public int ShotsPerBurst { get => shotsPerBurst; }
    public float BurstShotTime { get => burstShotTime; }
    public float TimeLastFired { get => timeLastFired; }
    public float Damage { get => damage; }
    public float Range { get => range; }
    public float SpreadAngle { get => spreadAngle; }
    public float AdsSpreadAngle { get => adsSpreadAngle; }
    public GameObject MuzzleFlashGFX { get => muzzleFlashGFX; }
    public GameObject BulletGFX { get => bulletGFX; }
    public VFXType MuzzleVFXType { get => muzzleVFXType; }
    public VFXType StreakVFXType { get => streakVFXType; }
    public VFXType HitVFXType { get => hitVFXType; }


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

    //Shoot doesn't actually instantiate anything, but resets the "last fired time" and decrements ammo
    public void Shoot()
    {
        //Debug.Log("Base Shoot");
        timeLastFired = Time.time;
        currentClipAmmo--;
    }

    public void Reload()
    {
        //Debug.Log("Base Reload");
        //For now this just instantly reloads. Should probably have a delay placed on it in the dude controller
        int bulletsToReload = Mathf.Min(maxClipAmmo - currentClipAmmo, currentReserveAmmo);

        currentReserveAmmo -= bulletsToReload;
        currentClipAmmo += bulletsToReload;

    }

    //Note, this is for adding ammo to the reserveAmmo (ie. pick up more ammo)
    public void AddReserveAmmo(int numAmmo)
    {
        Debug.Log("Base AddAmmo");
        throw new NotImplementedException();
    }
}

public enum WeaponShotType
{
    Hitscan,
    Shotgun_Hitscan,
    Constant_Ray,
    Projectile
}




