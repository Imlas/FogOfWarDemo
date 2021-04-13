using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

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

    [SerializeField] private Weapon currWeaponEqipped;


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
        if (currWeaponEqipped == null) return; //Shouldn't be needed once we can confirm the player always has a weapon.

        if (Input.GetKeyDown(KeyCode.R)) //Reload is hard-bound to R for now (all of this should eventually be migrated to the new input manager)
        {
            CmdPlayerReload();
        }

        if(Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && currWeaponEqipped.IsAutomatic))
        {
            if (currWeaponEqipped.CanShoot())
            {
                CmdPlayerShoot();
            }            
        }



    }

    [Command]
    private void CmdPlayerShoot()
    {
        //The way I'd like to do this is have this function have the server spawn the authoritative projectile (no gfx needed)
        //And also all for the clients to Instantiate a graphical version of the shots (that have collision, but just disappear without damage/etc.)
        //We would probably want to spawn hit gfx/sfx based only on the server colisions, but see how it looks/feels

        if (!currWeaponEqipped.CanShoot()) return;

        List<GameObject> bullets = currWeaponEqipped.Shoot();

        if(bullets != null)
        {
            foreach(GameObject bullet in bullets)
            {
                //NetworkServer.Spawn(bullet, connectionToClient); //We spawn the bullets server-side
                NetworkServer.Spawn(bullet);
            }



        }

        //Debug.Break();
    }

    [Command]
    private void CmdPlayerReload()
    {
        currWeaponEqipped.Reload();
    }

    [ClientRpc]
    private void RpcPlayerShoot()
    {
        //Spawns the visual bullets that all clients see
        //Also triggers the appropriate sound effect (eventually, once I have sound ever)

    }


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
}
