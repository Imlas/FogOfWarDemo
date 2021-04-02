using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AssaultRifle : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotSpeed; //The speed of the projectile. For now this is constant, but may look better to have a small ~5%? variation

    [Command]
    public override void CmdShoot()
    {
        Debug.Log("ARifle Shoot");

        //We do the same check for ammo and c/d here as a security measure.
        if (!base.CanShoot())
        {
            return;
        }

        //Spawn projectile(s) at the spawn point, modified by the spreadAngle
        //For now the shot point is static on the player. In the future this will possibly be looking for a 'weapon model' component, and grab the FirePoint from there
        Transform shotPoint = this.GetComponentInParent<DudeController>().FirePoint;

        GameObject newShot = Instantiate(bulletPrefab, shotPoint.position, Quaternion.identity); //Note, doesn't yet modify the angle
        NetworkServer.Spawn(newShot);


        //Push the projectile(s)
        newShot.GetComponent<Rigidbody>().velocity = shotSpeed * Vector3.forward;

        //Play sounds
        //One day I'll have audio in anything I make. Maybe. Not today.

        //Decrement ammo
        base.CurrentClipAmmo--;

        //update timeLastFired
        base.timeLastFired = Time.time;
    }

    [Command]
    public override void CmdReload()
    {
        Debug.Log("ARifle Reload");
        //For now this just reloads instantly, no cooldown or time required
        //Will need to modify it to take some amount of time, trigger some appropriate animations/sound effects
        //Maybe allow canceling (by changing weapons?)


        if(CurrentClipAmmo == MaxClipAmmo || CurrentReserveAmmo == 0)
        {
            //Clip ammo is maxed or total ammo is 0
            return;
        }

        //For now, we'll stick with a model of not "wasting" existing bullets in the clip
        int bulletsToRefill = Mathf.Min(MaxClipAmmo - CurrentClipAmmo, CurrentReserveAmmo);

        //Add this number of bullets to the clip, and subtract it from reserves
        CurrentClipAmmo += bulletsToRefill;
        CurrentReserveAmmo -= bulletsToRefill;

    }

}
