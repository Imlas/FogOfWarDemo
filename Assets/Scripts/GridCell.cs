using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid cell component. Handles mouse interations with the cell
/// </summary>
public class GridCell : MonoBehaviour
{

    /// <summary>
    /// Ranges from 0 to 1 with 0 indicating that the cell is not visible
    /// </summary>
    public float Visibility { get; private set; }
    private bool isVisible = false;

    private void Start()
    {
        MaskRenderer.RegisterCell(this);
    }

    private void OnMouseDown()
    {
        ToggleVisibility();
    }

    private void OnMouseEnter()
    {
        ToggleVisibility();
    }

    /// <summary>
    /// Toggle the visibility and lerp to the new value from the current one
    /// Interupts itself if still in a animation
    /// </summary>
    private void ToggleVisibility()
    {
        if (!Input.GetMouseButton(0))
            return;

        isVisible = !isVisible;
        StopAllCoroutines();
        StartCoroutine(AnimateVisibility(isVisible ? 1.0f : 0.0f));
    }

    private IEnumerator AnimateVisibility(float targetVal)
    {
        float startingTime = Time.time;
        float startingVal = Visibility;
        float lerpVal = 0.0f;
        while(lerpVal < 1.0f)
        {
            lerpVal = (Time.time - startingTime) / 1.0f;
            Visibility = Mathf.Lerp(startingVal, targetVal, lerpVal);
            yield return null;
        }
        Visibility = targetVal;
    }
}
