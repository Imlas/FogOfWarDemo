using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoWCookieGenerator : MonoBehaviour
{
    public RenderTexture mainFoWTexture;
    public RenderTexture secondaryFoWTexture;
    public RenderTexture combinedFoWTexture;
    //public Texture blackTexture;
    public Material cookieGenMat; //A material with the appropriate shader to add the two textures


    // Update is called once per frame
    void Update()
    {
        //ResetWorkingTexture();

        Graphics.Blit(mainFoWTexture, combinedFoWTexture);
        Graphics.Blit(secondaryFoWTexture, combinedFoWTexture, cookieGenMat);
    }

    //void ResetWorkingTexture()
    //{
    //    Graphics.Blit(blackTexture, combinedFoWTexture);
    //}
}
