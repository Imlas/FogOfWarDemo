using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Fadable : NetworkBehaviour
{
    /// <summary>
    /// This is a class dedicated to smoothly changing the opactiy of the material(s?) attatched to a gameobject, and syncing that between clients
    /// </summary>

    [SerializeField] private MeshRenderer rend;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float fadeInTime;



    private IEnumerator coroutine;
    [SyncVar] private float curAlpha;

    private bool isFadingOut;
    private bool isFadingIn;


    public override void OnStartServer()
    {
        base.OnStartServer();

        curAlpha = 0f;
        SetAlphaTo(curAlpha);
        isFadingOut = false;
        isFadingIn = false;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        SetAlphaTo(curAlpha);
    }

    /// <summary>
    /// Will slowly (over fadeOutTime seconds) decrease the alpha of the colors of all of the materials on the linked MeshRenderer.
    /// Uses the default fadeOutTime.
    /// </summary>
    //[ClientRpc]
    //public void RPCFadeOut()
    //{
    //    RPCFadeOut(fadeOutTime);
    //}

    /// <summary>
    /// Will slowly (over _fadeOutTime seconds) decrease the alpha of the colors of all of the materials on the linked MeshRenderer.
    /// </summary>
    /// <param name="_fadeOutTime"></param>
    [ClientRpc]
    public void RPCFadeOut(float _fadeOutTime)
    {
        if (isFadingOut || curAlpha == 0f)
        {
            return;
        }

        if (coroutine != null && isFadingIn)
        {
            StopCoroutine(coroutine);
            isFadingIn = false;
        }

        coroutine = SmoothFadeOut(_fadeOutTime);
        isFadingOut = true;
        StartCoroutine(coroutine);
        //Debug.Log($"Starting Fade Out at {Time.time}. curAlpha is {curAlpha}");
    }

    private IEnumerator SmoothFadeOut(float _fadeOutTime)
    {
        //curValue will lerp from 1.0 to 0.0
        while (curAlpha > 0f)
        {
            curAlpha -= Time.deltaTime / _fadeOutTime;

            //We then get each material in the renderer, and set the alpha of the BaseColor to the curValue
            SetAlphaTo(curAlpha);
            yield return null;
        }

        //In case we've overshot 0.0, we'll just set it to a flat 0. 
        curAlpha = 0f;
        SetAlphaTo(curAlpha);
        isFadingOut = false;
        //Debug.Log($"Finished Fade Out at {Time.time}. curAlpha is {curAlpha}");
    }

    /// <summary>
    /// Loops through all of the materials in this GameObjects linked MeshRenderer and sets the BaseColorAlpha to newAlpha
    /// </summary>
    /// <param name="newAlpha"></param>
    private void SetAlphaTo(float newAlpha)
    {
        foreach (Material mat in rend.materials)
        {
            //Debug.Log($"Material color is {mat.GetColor("_BaseColor")}");
            Color curColor = mat.GetColor("_BaseColor");
            mat.SetColor("_BaseColor", new Color(curColor.r, curColor.g, curColor.b, newAlpha));
        }

    }

    //[ClientRpc]
    //public void RPCFadeIn()
    //{
    //    RPCFadeIn(fadeInTime);
    //}

    [ClientRpc]
    public void RPCFadeIn(float _fadeInTime)
    {
        if (isFadingIn || curAlpha == 1.0f)
        {
            return;
        }

        if (coroutine != null && isFadingOut)
        {
            StopCoroutine(coroutine);
            isFadingOut = false;
        }

        coroutine = SmoothFadeIn(_fadeInTime);
        isFadingIn = true;
        StartCoroutine(coroutine);
        //Debug.Log($"Starting Fade In at {Time.time}. curAlpha is {curAlpha}");

    }

    private IEnumerator SmoothFadeIn(float _fadeInTime)
    {
        //curValue will lerp from 0.0 to 1.0
        while (curAlpha <= 1.0f)
        {
            //curAlpha = ((Time.time - startTime) / _fadeInTime);
            curAlpha += Time.deltaTime / _fadeInTime;

            //We then get each material in the renderer, and set the alpha of the BaseColor to the curValue
            SetAlphaTo(curAlpha);
            yield return null;
        }

        //In case we've overshot 1.0, we'll just set it to a flat 1. 
        curAlpha = 1f;
        SetAlphaTo(curAlpha);
        isFadingIn = false;
        //Debug.Log($"Finished Fade In at {Time.time}. curAlpha is {curAlpha}");

    }

    [ClientRpc]
    public void RPCAlphaToLog(string testString)
    {
        Debug.Log($"The alpha of this Fadable is: {curAlpha}. {testString}");
    }


    [ContextMenu(("Fade Out"))]
    private void EditorFadeOutTest()
    {
        RPCFadeOut(fadeOutTime);
        //foreach(Material mat in rend.materials)
        //{
        //    Debug.Log($"Material color is {mat.GetColor("_BaseColor")}");
        //    Color curColor = mat.GetColor("_BaseColor");
        //    mat.SetColor("_BaseColor", new Color(curColor.r, curColor.g, curColor.b, 0.0f));
        //}
    }

    [ContextMenu(("Fade In"))]
    private void EditorFadeInTest()
    {
        RPCFadeIn(fadeInTime);
        //foreach(Material mat in rend.materials)
        //{
        //    Debug.Log($"Material color is {mat.GetColor("_BaseColor")}");
        //    Color curColor = mat.GetColor("_BaseColor");
        //    mat.SetColor("_BaseColor", new Color(curColor.r, curColor.g, curColor.b, 0.0f));
        //}
    }

}
