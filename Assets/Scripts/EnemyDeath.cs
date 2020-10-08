using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour {
   // where they'll fly offscreen when they get captured
   Vector3 offScreenOffset = new Vector3(0, 10,0);
   // ref to board
   Board m_board;
   
   // iTween variables
   public float deathDelay = 0f;
   public float offScreenDelay = 1f;
   public float iTweenDelay = 0f;
   public iTween.EaseType easeType = iTween.EaseType.easeInOutQuint;
   public float moveTime = 0.5f;
   
   public void MoveOffBoard(Vector3 target) {
      iTween.MoveTo(gameObject, iTween.Hash(
            "x", target.x,
            "y", target.y,
            "z", target.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "time", moveTime
         ));
   }

   private void Awake() { m_board = FindObjectOfType<Board>().GetComponent<Board>(); }

   //testing
   private void Start() { Die(); }

   // method to call the die coroutine
   public void Die() { StartCoroutine(nameof(DieRoutine)); }

   IEnumerator DieRoutine() {
       yield return new WaitForSeconds(deathDelay);
       Vector3 offScreenPosition = transform.position + offScreenOffset;
       MoveOffBoard(offScreenPosition);
       yield return new WaitForSeconds(moveTime + offScreenDelay);
       
       // error check:: ensure the captureposition is valid
       if (m_board.capturePositions.Count != 0 
           && m_board.CurrentCapturePosition < m_board.capturePositions.Count) {
           
           // the next available empty capture position
           var capturePos = m_board.capturePositions[m_board.CurrentCapturePosition].position;
           
           // move to just above the open capture position 
           transform.position = capturePos + offScreenOffset;
           
           MoveOffBoard(capturePos);
           
           yield return new WaitForSeconds(moveTime);

           // increment the current index and verify that index is a valid one.
           m_board.CurrentCapturePosition++;
           m_board.CurrentCapturePosition = Mathf.Clamp(m_board.CurrentCapturePosition, 
               0, m_board.CurrentCapturePosition - 1);
       }
       
   }
   
}
