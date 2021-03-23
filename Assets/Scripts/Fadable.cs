using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Fadable : NetworkBehaviour
{
    /// <summary>
    /// This is a class dedicated to smoothly changing the opactiy of the material(s?) attatched to a gameobject, and syncing that between clients
    /// </summary>

    [SerializeField] private Material mainMat; //This might be modified later as a List
    [SerializeField] private float fadeTime;
    [SerializeField] private float unFadeTime;

    private IEnumerator coroutine;
    private float startTime;
    private float curValue;

    public void Fade()
    {
        Fade(fadeTime);
    }

    public void Fade(float _fadeTime)
    {
        startTime = Time.time;
        curValue = 1f;
        coroutine = SmoothFade(_fadeTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator SmoothFade(float _fadeTime)
    {
        while(curValue >= 0f)
        {
            curValue = ((Time.time - startTime) / _fadeTime);
            Debug.Log($"{curValue}");
        }
        yield return null;
    }

    public void UnFade()
    {
        UnFade(unFadeTime);
    }

    public void UnFade(float _unFadeTime)
    {
        startTime = Time.time;
    }


    [ContextMenu(("Fade Out over 3s"))]
    private void EditorFadeTest()
    {
        Fade(3f);
    }


}
