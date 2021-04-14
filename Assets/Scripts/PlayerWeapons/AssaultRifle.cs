using System.Collections;
using System.Collections.Generic;
//using Mirror;
using UnityEngine;

public class AssaultRifle : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletCosmeticPrefab;
    [SerializeField] private float shotSpeed; //The speed of the projectile. For now this is constant, but may look better to have a small ~5%? variation

    public override BulletReturn Shoot()
    {
        //Debug.Log("ARifle Shoot");
        //(Other weapons might spawn multiple shots at once (ie. shotgun), so we have this as a list
        List<GameObject> serverBullets = new List<GameObject>();
        List<GameObject> gfxBullets = new List<GameObject>();

        //We do the same check for ammo and c/d here as a security measure.
        if (!base.CanShoot())
        {
            return new BulletReturn(false, serverBullets, gfxBullets); //Apparently structs aren't nullable. If I wanted/needed this it would need to be a class
        }


        //Spawn projectile(s) at the spawn point, modified by the spreadAngle
        //For now the shot point is static on the player. In the future this will possibly be looking for a 'weapon model' component, and grab the FirePoint from there
        Transform shotPoint = this.GetComponentInParent<DudeController>().firePoint;

        GameObject newShot = Instantiate(bulletPrefab, shotPoint.position, shotPoint.rotation); //Note, doesn't yet modify the angle
        //NetworkServer.Spawn(newShot);

        //We'll probably want to modify this to match the weapon pattern, with a generic "Bullet" class?
        GenericBullet bullet = newShot.GetComponent<GenericBullet>();
        bullet.damage = this.damage;
        bullet.speed = this.shotSpeed;

        //Push the projectile(s)
        //newShot.GetComponent<Rigidbody>().velocity = shotSpeed * Vector3.forward;
        //Note - projectiles now push themselves (just a transform movement in update)

        //Add the newShot to the list of bullets
        serverBullets.Add(newShot);

        //Decrement ammo
        base.currentClipAmmo--;

        //update timeLastFired
        base.timeLastFired = Time.time;

        return new BulletReturn(true, serverBullets, gfxBullets);
    }

    public override void Reload()
    {
        Debug.Log("ARifle Reload");
        //For now this just reloads instantly, no cooldown or time required
        //Will need to modify it to take some amount of time, trigger some appropriate animations/sound effects
        //Maybe allow canceling (by changing weapons?)


        if(currentClipAmmo == maxClipAmmo || currentReserveAmmo == 0)
        {
            //Clip ammo is maxed or total ammo is 0
            return;
        }

        //For now, we'll stick with a model of not "wasting" existing bullets in the clip
        int bulletsToRefill = Mathf.Min(maxClipAmmo - currentClipAmmo, currentReserveAmmo);

        //Add this number of bullets to the clip, and subtract it from reserves
        currentClipAmmo += bulletsToRefill;
        currentReserveAmmo -= bulletsToRefill;
    }

    public override void AddReserveAmmo(int numAmmo)
    {
        //Add the indicated amount of ammo
        currentReserveAmmo += numAmmo;

        //If we're over the max, discard down to the max
        if(currentReserveAmmo > maxReserveAmmo)
        {
            currentReserveAmmo = maxReserveAmmo;
        }
    }

}
