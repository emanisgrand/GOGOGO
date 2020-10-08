using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // ref to GameManager
    protected GameManager m_gameManager;
    
    // turn boolean
    protected bool m_isTurnComplete = false;
    public bool IsTurnComplete { get => m_isTurnComplete; set => m_isTurnComplete = value; }
    
    // init fields
    protected virtual void Awake() {
        m_gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }
    
    // complete the turn. update the game manager
    public virtual void FinishTurn() {
        m_isTurnComplete = true;
        m_gameManager?.UpdateTurn();
    }
}
