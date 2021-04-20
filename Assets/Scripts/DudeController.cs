using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DudeController : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string displayName = "Missing Name";

    private Cinemachine.CinemachineVirtualCamera virtCam;
    private Camera viewCamera;

    [SerializeField] private LayerMask groundPlaneMask;

    public float moveSpeed;
    public float turnSpeed;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Rigidbody rb;

    private Vector3 worldMousePos;

    [SerializeField] public Transform firePoint;


    [SyncVar(hook = nameof(OnWeaponChanged))] [SerializeField] private int syncWeaponEquippedIndex = 0; //We'll add a hook to this later
    [SerializeField] private int localWeaponEquippedIndex = 0;
    [SerializeField] private GameObject[] equippedWeaponGOs;
    [SerializeField] private Weapon currWeaponEqipped;

    [SerializeField] private TextMeshProUGUI ammoText;

    //[SerializeField] private GameObject testCube;

    private void Awake()
    {
        ammoText = SceneAssignmentHelper.Instance.uiAmmoText;

        //Disable all weapons
        foreach(GameObject weaponGO in equippedWeaponGOs)
        {
            if(weaponGO != null)
            {
                weaponGO.SetActive(false);
            }
        }

        //Set the correct weapon to active (if non-null)
        if (equippedWeaponGOs[localWeaponEquippedIndex] != null)
        {
            equippedWeaponGOs[localWeaponEquippedIndex].SetActive(true);
            currWeaponEqipped = equippedWeaponGOs[localWeaponEquippedIndex].GetComponent<Weapon>();
        }
    }

    //The "network" version of Start
    public override void OnStartAuthority()
    {
        //Debug.Log("OnStartAuthority begun");
        //if (!hasAuthority) { return; } //This... might not be needed? idk if this method only runs if you have authority anyway

        //If this is the owned player, then set the cinemachine cam to look at/follow this player object
        //If there's more camera changes later, might better to shove this in a seperate camera controller script
        virtCam = GameObject.FindGameObjectWithTag("MainVirtCam").GetComponent<Cinemachine.CinemachineVirtualCamera>();

        virtCam.m_LookAt = this.transform;
        virtCam.m_Follow = this.transform;

        //Grabbing the main cam since apparently the Camera.main call is icky
        viewCamera = Camera.main;

        //Grabbing some other parts
        //rb = GetComponent<Rigidbody>();

        //Debug.Log("OnStartAuthority finished");
        UpdateAmmoText();
    }

    //Will only run on clients, not on the server
    [ClientCallback]
    void Update()
    {
        //We only want to be running this for the "player" that the client owns
        if (!isLocalPlayer) return;

        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundPlaneMask))
        {
            worldMousePos = hit.point;
            //Debug.Log($"MousePos: {mousePos}");
            //transform.LookAt(mousePos + Vector3.up * transform.position.y);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(mousePos + Vector3.up * transform.position.y), turnSpeed * Time.deltaTime);
            //Debug.Log($"Mouse world: {worldMousePos}. Mouse local: {this.transform.InverseTransformPoint(worldMousePos)}");
            //transform.rotation = Quaternion.LookRotation(this.transform.InverseTransformPoint(mousePos), Vector3.up);
            Vector3 lookDir = worldMousePos - this.transform.position;
            float angle = -1 * (Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg - 90f);
            //Gonna be super honest - no idea why I have to negative the angle I get from this. But it works.

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, angle, 0f), turnSpeed * Time.deltaTime);
        }
        //velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        ////rb.velocity = this.velocity;
        //rb.MovePosition(rb.position + velocity * Time.deltaTime);

        //Now that we've done all that moving, let's check for shooting
        //if (currWeaponEqipped == null) return; //Shouldn't be needed once we can confirm the player always has a weapon.

        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    //Debugging/testing
        //    Debug.Log("Test F1 press!");
        //    CmdTestCommand();
        //}

        if (Input.GetKeyDown(KeyCode.R)) //Reload is hard-bound to R for now (all of this should eventually be migrated to the new input manager)
        {
            CmdPlayerReload();
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                //Mouse Wheel Up
                localWeaponEquippedIndex++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                //Mouse Wheel Down
                localWeaponEquippedIndex--;
            }

            //Rectify the index if out of bounds
            if (localWeaponEquippedIndex < 0) { localWeaponEquippedIndex = equippedWeaponGOs.Length - 1; } //Too low, wrap it back to the end
            if (localWeaponEquippedIndex > equippedWeaponGOs.Length - 1) { localWeaponEquippedIndex = 0; } //Too far, wrap it back to the beginning

            CmdChangeActiveWeapon(localWeaponEquippedIndex);
        }

        if (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && currWeaponEqipped.IsAutomatic))
        {
            if (currWeaponEqipped.CanShoot())
            {
                CmdPlayerShoot();
            }
        }



    }

    void OnWeaponChanged(int _oldIndex, int _newIndex)
    {
        if(_oldIndex >= 0 && _oldIndex <= equippedWeaponGOs.Length - 1 && equippedWeaponGOs[_oldIndex] != null)
        {
            equippedWeaponGOs[_oldIndex].SetActive(false);
        }

        if (_newIndex >= 0 && _newIndex <= equippedWeaponGOs.Length - 1 && equippedWeaponGOs[_newIndex] != null)
        {
            equippedWeaponGOs[_newIndex].SetActive(true);
        }

        if (isLocalPlayer)
        {
            currWeaponEqipped = equippedWeaponGOs[_newIndex].GetComponent<Weapon>();
            UpdateAmmoText();
        }
    }

    //[Command]
    //public void CmdTestCommand()
    //{
    //    Debug.Log("Command receieved. Calling the RPC");
    //    RpcTest();
    //}

    //[ClientRpc]
    //public void RpcTest()
    //{
    //    //It's interesting to note that in the following line the Random number is different for each client
    //    ammoText.text = $"Test {Random.Range(0, 100)}.";

    //    //Instantiate(testCube, this.transform.position, this.transform.rotation);
    //    Debug.Log(currWeaponEqipped.MuzzleFlashGFX.name);
    //    Instantiate(currWeaponEqipped.MuzzleFlashGFX, firePoint.position, firePoint.rotation);
    //}

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        //Some validation here to double check it's in bounds, and that the current weapon isn't on cooldown/etc/etc/

        syncWeaponEquippedIndex = newIndex;
    }

    [Command]
    private void CmdPlayerShoot()
    {
        //Thoughts:
        //Scrap the Weapon children, make Weapon a normal class
        //Give it a weaponType enum, with the options:
        // singleRay (single shots, hitscan)
        // burstRay (similar, but will kick off a coroutine to fire the other shots with delay)
        // shotgunRay (multiple shots at once (maybe with some non-uniform distribution?), hitscan)
        //  both of the above need to include a numberOfShots value
        // constantRay (for a continuous laser - pulse laser would be one of the above)
        // singleMissile (for something that actually fires a projectile with a collider)
        // --For all of the above, can do burst or single, just based on reading a numOfShots variable. If it's 1, then ignore worrying about repeat shots
        // For all of these, this command will do the appropriate case and will either rayCast to determine the hit immediately, or server-side instantiate an invisible (so host won't see it) GameObject
        //  in this case, both the invis and the gfx version should have similar (the same?) script for movement/etc.
        // After server-side bits, the server will ClientRPC to have them all Instantiate the appropriate bullet streak/visible gfx/mussle flash/sfx/etc.


        //The way I'd like to do this is have this function have the server spawn the authoritative projectile (no gfx needed)
        //And also all for the clients to Instantiate a graphical version of the shots (that have collision, but just disappear without damage/etc.)
        //We would probably want to spawn hit gfx/sfx based only on the server colisions, but see how it looks/feels

        if (!currWeaponEqipped.CanShoot()) return;

        switch (currWeaponEqipped.GetWeaponShotType)
        {
            case WeaponShotType.Hitscan:
                //blah
                //Debug.Log("Weapon shot type: Hitscan");
                //Instantiate the muzzle flash GFX at the fire point
                Debug.Log(currWeaponEqipped.WeaponName);
                Debug.Log(currWeaponEqipped.MuzzleFlashGFX.name);

                GameObject tempGO = Instantiate(currWeaponEqipped.MuzzleFlashGFX, firePoint.position, firePoint.rotation);
                NetworkServer.Spawn(tempGO);
                //RpcPlayerInstantiate(tempGO, tempGO.transform.position, tempGO.transform.rotation);


                //Figure out what angle the shot will actually come out at (based on RNG + if the player is adsing)

                //Do the raycast to determine the hit

                //If the raycast hit something damage-able, do damage as appropraite

                //Instantiate the bullet gfx at the actual raycast angle
                //(TODO - we're cheating right now and not modifying the angle)
                //RpcPlayerInstantiate(currWeaponEqipped.BulletGFX, firePoint.position, firePoint.rotation);
                tempGO = Instantiate(currWeaponEqipped.BulletGFX, firePoint.position, firePoint.rotation);
                NetworkServer.Spawn(tempGO);


                //Maybe instantiate the bullet hit gfx at the hit position? (see if it looks weird to have it happen right away, or if it should be delayed slightly)

                //Call the weapon's shoot function to correctly decrement ammo/reset shot time
                currWeaponEqipped.Shoot();

                //Play SFX (lolsound)

                //Update the ammo text
                UpdateAmmoText();

                break;
            case WeaponShotType.Shotgun_Hitscan:
                //blah
                Debug.Log("Weapon shot type: Shotgun");


                break;
            case WeaponShotType.Constant_Ray:
                //blah
                Debug.Log("Weapon shot type: Constant Ray");


                break;
            case WeaponShotType.Projectile:
                //blah
                Debug.Log("Weapon shot type: Projectile");


                break;
        }


        //Debug.Break();
    }

    [Command]
    private void CmdPlayerReload()
    {
        currWeaponEqipped.Reload();

        UpdateAmmoText();
    }

    //[ClientRpc]
    //private void RpcPlayerInstantiate(GameObject _gameObject, Vector3 _position, Quaternion _rotation)
    //{
    //    //THIS WHOLE THING WAS DUMB - THIS IS EXACTLY WHAT NETWORKSERVER.SPAWN(FOO) IS FOR

    //    if(_gameObject == null)
    //    {
    //        Debug.Log("lolwut");
    //        Debug.Break();
    //        return;
    //    }
    //    Instantiate(_gameObject, _position, _rotation);
    //}

    //[ClientRpc]
    //private void RpcPlayerShoot(List<GameObject> bulletGFX)
    //{
    //    //Spawns the visual bullets that all clients see
    //    //Also triggers the appropriate sound effect (eventually, once I have sound ever)
    //    foreach(GameObject bGFX in bulletGFX)
    //    {
    //        Instantiate(bGFX, bGFX.transform.position, bGFX.transform.rotation); //NOT RIGHT??
    //    }
    //}


    //Will only run on clients, not on the server
    [ClientCallback]
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;


        //rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        rb.velocity = this.velocity;

    }

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        //Doesn't do anthing for now.
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    private void UpdateAmmoText()
    {
        if (ammoText == null || currWeaponEqipped == null)
        {
            Debug.Log("Issue updating Ammo text");
            return;
        }

        ammoText.text = $"Ammo: {currWeaponEqipped.CurrentClipAmmo} / {currWeaponEqipped.CurrentReserveAmmo}";
    }
}
