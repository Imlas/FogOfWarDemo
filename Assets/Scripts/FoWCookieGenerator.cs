using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoWCookieGenerator : MonoBehaviour
{
    public RenderTexture mainFoWTexture;
    public RenderTexture secondaryFoWTexture;
    public RenderTexture tempTexture;
    public RenderTexture combinedFoWTexture;
    //public Texture blackTexture;
    public Material cookieGenMat; //A material with the appropriate shader to add the two textures
    public Material blurMat;


    // Update is called once per frame
    void Update()
    {
        //ResetWorkingTexture();

        Graphics.Blit(mainFoWTexture, combinedFoWTexture);
        Graphics.Blit(secondaryFoWTexture, combinedFoWTexture, cookieGenMat);
        Graphics.Blit(combinedFoWTexture, tempTexture, blurMat);
        Graphics.Blit(tempTexture, combinedFoWTexture);
    }

    //void ResetWorkingTexture()
    //{
    //    Graphics.Blit(blackTexture, combinedFoWTexture);
    //}
}
