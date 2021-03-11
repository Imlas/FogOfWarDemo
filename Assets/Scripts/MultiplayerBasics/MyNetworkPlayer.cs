using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))]
    [SerializeField]
    private string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
    [SerializeField] private Color displayColor = Color.white;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TMP_Text displayNameText;

    private NavMeshAgent navAgent;
    private Camera cam;

    #region Server

    //The server tag isn't strictly needed. As a syncvar, displayname can already only be changed by the server. This just helps us slightly from accidentally calling this on a client.
    //Server tag things can only be run by the server
    //
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [Server]
    public void SetDisplayColor(Color newColor)
    {
        displayColor = newColor;

    }

    //Commands are something the client is asking the server to do. The actual command is run on the server end of things. Name must be CmdFOO
    /// <summary>
    /// From the server-end, will validate the given string, and set the player's display name to it (if valid).
    /// </summary>
    /// <param name="newDisplayName"></param>
    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        //Some initial name validation here
        if(newDisplayName.Length > 2 && newDisplayName.Length < 20)
        {
            RpcTextToLog(newDisplayName);
            SetDisplayName(newDisplayName);
        }
        else
        {
            RpcTextToLog("Player name not valid.");
        }

    }

    #endregion

    #region Client

    //Note, these hook functions must have both the old value and the new value passed in, even if you don't use both
    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        meshRenderer.material.color = newColor;
    }

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        displayNameText.SetText(newName);
    }

    [ContextMenu(("Set My Name"))]
    private void SetMyName()
    {
        CmdSetDisplayName("a");
    }



    //ClientRpc tag is used for things the server is asking (all) clients to do
    //TargetRpc is used for targeted calls. By default it will only call for the owner
    /// <summary>
    /// Has all of the clients write the string logText to Debug.Log
    /// </summary>
    /// <param name="logText"></param>
    [ClientRpc]
    public void RpcTextToLog(string logText)
    {
        Debug.Log(logText);
    }

    #endregion
}
