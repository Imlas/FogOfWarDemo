using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Fadable : NetworkBehaviour
{
    /// <summary>
    /// This is a class dedicated to smoothly changing the opactiy of the material(s?) attatched to a gameobject, and syncing that between clients
    /// </summary>

    [SerializeField] private MeshRenderer rend; //This might be modified later as a List
    [SerializeField] private float fadeTime;
    [SerializeField] private float unFadeTime;

    private IEnumerator coroutine;
    private float startTime;
    private float curValue;

    public void FadeOut()
    {
        FadeOut(fadeTime);
    }

    public void FadeOut(float _fadeOutTime)
    {
        startTime = Time.time;
        curValue = 1f;
        coroutine = SmoothFadeOut(_fadeOutTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator SmoothFadeOut(float _fadeOutTime)
    {
        //curValue will lerp from 1.0 to 0.0
        while(curValue >= 0f)
        {
            curValue = 1f - ((Time.time - startTime) / _fadeOutTime);

            //We then get each material in the renderer, and set the alpha of the BaseColor to the curValue
            SetAlphaTo(curValue);
            yield return null;
        }

        //In case we've overshot 0.0, we'll just set it to a flat 0. 
        curValue = 0f;
        SetAlphaTo(curValue);

    }

    /// <summary>
    /// Loops through all of the materials in this GameObjects linked MeshRenderer and sets the BaseColorAlpha to newAlpha
    /// </summary>
    /// <param name="newAlpha"></param>
    public void SetAlphaTo(float newAlpha)
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
        FadeIn(unFadeTime);
    }

    public void FadeIn(float _fadeInTime)
    {
        startTime = Time.time;
        curValue = 0.0f;
        coroutine = SmoothFadeIn(_fadeInTime);
        StartCoroutine(coroutine);

    }

    private IEnumerator SmoothFadeIn(float _fadeInTime)
    {
        //curValue will lerp from 0.0 to 1.0
        while (curValue <= 1.0f)
        {
            curValue = ((Time.time - startTime) / _fadeInTime);

            //We then get each material in the renderer, and set the alpha of the BaseColor to the curValue
            SetAlphaTo(curValue);
            yield return null;
        }

        //In case we've overshot 1.0, we'll just set it to a flat 1. 
        curValue = 1f;
        SetAlphaTo(curValue);

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
