using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GraphicMoverMode
{
    MoveTo,
    ScaleTo,
    MoveFrom
}
public class GraphicMover : MonoBehaviour {
    // which iTween method are we using to transform from point A to point B
    public GraphicMoverMode mode;
    // source transform (point A) 
    public Transform startXform;
    // target transform (point B)
    public Transform endXform;
    // animation time
    public float moveTime = 1f;
    // delay before iTween animation starts
    public float delay = 0f;
    // loop type, if we are animating in a cycle
    public iTween.LoopType loopType = iTween.LoopType.none;
    // ease in-out
    public iTween.EaseType easeType = iTween.EaseType.easeOutExpo;
        // create null objects to store the beginning and ending transform if none is specified
    private void Awake()
    {
        if (endXform == null){
            endXform = new GameObject(gameObject.name + "XformEnd").transform;

            endXform.position = transform.position;
            endXform.rotation = transform.rotation;
            endXform.localScale = transform.localScale;
        }
        
        if (startXform == null){
            startXform = new GameObject(gameObject.name + "XformStart").transform;

            startXform.position = transform.position;
            startXform.rotation = transform.rotation;
            startXform.localScale = transform.localScale;
        }
        
        Reset();
    }

    // reset the transform to starting values
    public void Reset()
    {
        switch (mode)
        {
            case GraphicMoverMode.MoveTo:
                if (startXform!= null)
                {
                    transform.position = startXform.position;
                }
                break;
            case GraphicMoverMode.MoveFrom:
                if (endXform!=null)
                {
                    transform.position = endXform.position;
                }
                break;
            case GraphicMoverMode.ScaleTo:
                if (startXform!=null)
                {
                    transform.localScale = startXform.localScale;
                }
                break;
        }
    }

    // scale, rotate, or translate the graphic depending on mode
    public void Move()
    {
        switch (mode)
        {
            case GraphicMoverMode.MoveTo:
                iTween.MoveTo(gameObject, iTween.Hash(
                "position", endXform.position,
                "time", moveTime,
                "delay", delay,
                "easeType", easeType,
                "looptype", loopType
                    ));
                break;
            case GraphicMoverMode.ScaleTo:
                iTween.ScaleTo(gameObject, iTween.Hash(
                    "scale", endXform.localScale,
                    "time", moveTime,
                    "delay", delay,
                    "easeType", easeType,
                    "looptype", loopType
                ));
                break;
            case GraphicMoverMode.MoveFrom:
                iTween.MoveFrom(gameObject, iTween.Hash(
                    "position", startXform.position,
                    "time", moveTime,
                    "delay", delay,
                    "easeType", easeType,
                    "looptype", loopType
                ));
                break;
        }
    }
}
