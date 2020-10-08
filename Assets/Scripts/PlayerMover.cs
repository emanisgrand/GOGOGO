using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

public class PlayerMover : Mover {
    
    PlayerCompass m_playerCompass;
    
    protected override void Awake() {
        base.Awake();
        m_playerCompass = FindObjectOfType<PlayerCompass>().GetComponent<PlayerCompass>();
    }

    protected override void Start() {
        base.Start();
        UpdateBoard();
    }
    
    // Update the board's playerNode
    void UpdateBoard() {
        m_board?.UpdatePlayerNode();   
    }

    protected override IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime) {
        // disable compass arrows
        m_playerCompass?.ShowArrows(false);
        
        // run the base MoveRoutine
        yield return StartCoroutine(base.MoveRoutine(destinationPos, delayTime));
        
        // update the PlayerNode in the board
        UpdateBoard();
        
        // enable compass arrows
        m_playerCompass?.ShowArrows(true);
        
        // broadcast end of movement
        base.OnMovementFinished.Invoke();
    }
}
