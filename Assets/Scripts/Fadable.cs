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
    private float startTime;
    private float curAlpha;

    private bool isFadingOut;
    private bool isFadingIn;


    public override void OnStartServer()
    {
        base.OnStartServer();


        //Perhaps some check here of the material's alpha to set isFaded.
        curAlpha = 1f;
        isFadingOut = false;
        isFadingIn = false;
    }


    public void FadeOut()
    {
        FadeOut(fadeOutTime);
    }

    public void FadeOut(float _fadeOutTime)
    {
        if (isFadingOut || curAlpha == 0.0f)
        {
            return;
        }

        if (coroutine != null && isFadingIn)
        {
            StopCoroutine(coroutine);
            isFadingIn = false;
        }

        startTime = Time.time;
        coroutine = SmoothFadeOut(_fadeOutTime);
        StartCoroutine(coroutine);
        isFadingOut = true;
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

    public void FadeIn()
    {
        FadeIn(fadeInTime);
    }

    public void FadeIn(float _fadeInTime)
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

        startTime = Time.time;
        coroutine = SmoothFadeIn(_fadeInTime);
        StartCoroutine(coroutine);
        isFadingIn = true;
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
    }


    [ContextMenu(("Fade Out"))]
    private void EditorFadeOutTest()
    {
        FadeOut();
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
        FadeIn();
        //foreach(Material mat in rend.materials)
        //{
        //    Debug.Log($"Material color is {mat.GetColor("_BaseColor")}");
        //    Color curColor = mat.GetColor("_BaseColor");
        //    mat.SetColor("_BaseColor", new Color(curColor.r, curColor.g, curColor.b, 0.0f));
        //}
    }

}
