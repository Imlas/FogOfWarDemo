using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderWithoutClear : MonoBehaviour
{
    [SerializeField] private RenderTexture currentFoVTexture;
    [SerializeField] private RenderTexture historicFoVTexture;
    [SerializeField] private RenderTexture combinedFoVTexture;
    [SerializeField] private Material doesNotClearShaderMat;
    [SerializeField] private Material foVConstructionMat;
    [SerializeField] private Texture solidBlack;

    private void Start()
    {
        Graphics.Blit(solidBlack, combinedFoVTexture);
        //Debug.Break();
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.Blit(currentFoVTexture, historicFoVTexture, doesNotClearShaderMat);
        Graphics.Blit(currentFoVTexture, combinedFoVTexture, foVConstructionMat);

    }
}
