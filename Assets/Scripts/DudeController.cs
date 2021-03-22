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
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Rigidbody rb;


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

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundPlaneMask))
        {
            Vector3 mousePos = hit.point;
            //Debug.Log($"MousePos: {mousePos}");
            transform.LookAt(mousePos + Vector3.up * transform.position.y);
        }
        //velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        ////rb.velocity = this.velocity;
        //rb.MovePosition(rb.position + velocity * Time.deltaTime);


    }

    //Will only run on clients, not on the server
    [ClientCallback]
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        //rb.MovePosition(transform.position + velocity * Time.deltaTime);
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
