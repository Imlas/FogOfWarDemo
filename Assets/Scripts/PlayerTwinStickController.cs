using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerTwinStickController : NetworkBehaviour
{
    public float moveSpeed = 6f;
    public LayerMask layerMask; //configured in the editor for just the ground plane

    Rigidbody rb;
    Camera viewCamera;
    Vector3 velocity;

    //The "network" version of Start
    public override void OnStartAuthority()
    {
        rb = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
    }

    //Will only run on clients, not on the server
    [ClientCallback]
    private void Update()
    {
        //We only want to be running this for the "player" that the client owns
        if (!hasAuthority) { return; }

        //Can't do screen to world point for this - need to use a raycast instead is my guess
        //Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 mousePos = hit.point;
            //Debug.Log($"MousePos: {mousePos}");
            transform.LookAt(mousePos + Vector3.up * transform.position.y);
        }
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
