using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] Color solidColor = Color.white; // new Color (1f,1f,1f,1f)
    [SerializeField] Color clearColor = new Color(1f,1f,1f,0f);

    [SerializeField] float delay = 0.5f;
    [SerializeField] float timeToFade = 1f;
    [SerializeField] iTween.EaseType easeType = iTween.EaseType.easeOutExpo;

    private MaskableGraphic m_graphic;

    private void Awake() {
        m_graphic = GetComponent<MaskableGraphic>();
    }

    void UpdateColor(Color newColor)
    {
        m_graphic.color = newColor;
    }

    public void FadeOff()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", solidColor,
            "to", clearColor,
            "time", delay,
            "easeType", easeType,
            "onupdatetarget", gameObject, 
                "onupdate", "UpdateColor"
            ));
    }

    public void FadeOn()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", clearColor,
            "to", solidColor,
            "time", delay,
            "easeType", easeType,
            "onupdatetarget", gameObject, 
            "onupdate", "UpdateColor"
        ));
    }
}
