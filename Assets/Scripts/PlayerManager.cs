using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerDeath))]
public class PlayerManager : TurnManager {
    Board m_Board;
    
    // References to the PlayerMover and PlayerInput scripts.
    public PlayerMover PlayerMover;
    public PlayerInput PlayerInput;

    public UnityEvent deathEvent;

    protected override void Awake() {
        base.Awake();
        // Cached references to the PlayerMover and PlayerInput scripts
        PlayerMover = GetComponent<PlayerMover>();
        PlayerInput = GetComponent<PlayerInput>();
        m_Board = FindObjectOfType<Board>().GetComponent<Board>();
        
        // Make sure input is enabled when we start
        PlayerInput.InputEnabled = true;
    }

    void Update() {
        // if the player is currently moving OR if it's not the player's turn, ignore any input
        if (PlayerMover.isMoving || m_gameManager.CurrentTurn != GameManager.Turn.Player) { return; } 
        
        // get the keyboard input
        PlayerInput.GetKeyInput();
    
        // tie the player input to the PlayerMover's Move methods
        if (PlayerInput.V == 0)
        {
            if (PlayerInput.H < 0)
            {
                PlayerMover.MoveLeft();
            } else if (PlayerInput.H > 0)
            {
                PlayerMover.MoveRight();
            }
        } else if (PlayerInput.H == 0)
        {
            if (PlayerInput.V < 0)
            {
                PlayerMover.MoveBackward();
            } else if (PlayerInput.V > 0)
            {
                PlayerMover.MoveForward();
            }
        }
    }

   public void InvokeDeathEvent() {
       deathEvent?.Invoke();
   }

   public override void FinishTurn() {
       CaptureEnemies();
       base.FinishTurn();
   }

   private void CaptureEnemies() {
       if (m_Board != null) {
           // when a player is on an enemy
           List<EnemyManager> enemies = m_Board.FindEnemiesAt(m_Board.PlayerNode); 
           if (enemies.Count != 0) {
               foreach (var enemy in enemies) {
                   enemy?.InvokeDeathEvent();
               }
           }
       }
   }
}
