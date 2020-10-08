using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    
    [SerializeField] float borderWidth = 0.02f;
    [SerializeField] float lineThickness = 0.5f;
    [SerializeField] float scaleTime = 0.25f;
    [SerializeField] float delay = 0.1f;
    [SerializeField] iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
    
    // ground level at y axis
    private float groundLevel = -0.66f;
    
    public void DrawLink(Vector3 startPos, Vector3 endPos)
    {
        transform.localScale = new Vector3(lineThickness, 1f, 0f);

        Vector3 dirVector = endPos - startPos;

        float zScale = dirVector.magnitude - borderWidth * 2f;
        Vector3 newScale = new Vector3(lineThickness, 1f, zScale);
        
        transform.rotation = Quaternion.LookRotation(dirVector);

        transform.position = startPos + (transform.forward * borderWidth);

        iTween.ScaleTo(gameObject, iTween.Hash(
                "time", scaleTime,
                "scale", newScale,
                "easeType", easeType,
                "delay", delay
            ));
    }
}
