using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyManager : TurnManager {
    
    // ref of EnemyMover component
    EnemyMover m_enemyMover;
    
    // ref of EnemySensor component
    EnemySensor m_enemySensor;

    // ref of EnemyAttack script
    EnemyAttack m_enemyAttack;

    // ref of Board component
    Board m_board;

    // switch for triggering enemy death 
    private bool m_isDead = false;
    public bool IsDead => m_isDead;
    
    // actions to invoke upon enemy death
    public UnityEvent deathEvent;
    
    // member variable construction
    protected override void Awake() {
        base.Awake();
        
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
        m_enemyMover = GetComponent<EnemyMover>();
        m_enemySensor = GetComponent<EnemySensor>();
        m_enemyAttack = GetComponent<EnemyAttack>();
        
    }

    // routine to play enemy's turn
    public void PlayTurn() {
        if (m_isDead)  {
            FinishTurn();
            return;
        }
        StartCoroutine("PlayTurnRoutine");
    }
    
    // emeny's main routine. if its possible to attack the player, move then wait
    IEnumerator PlayTurnRoutine() {
        if (m_gameManager != null && !m_gameManager.IsGameOver) {
            
            // sense for the player
            m_enemySensor.UpdateSensor();
            
            // wait
            yield return new WaitForSeconds(0f);

            if (m_enemySensor.FoundPlayer) {
                
                // signal to the game manager to run the lose level event
                m_gameManager.LoseLevel();
                
                // the player's position at this point
                Vector3 playerPosition = new Vector3(m_board.PlayerNode.Coordinate.x, 0f, 
                        m_board.PlayerNode.Coordinate.y);
                
                // move to where the Player currently is
                m_enemyMover.Move(playerPosition, 0f);
                
                // wait until the end of the enemy iTween animation
                while (m_enemyMover.isMoving) {
                    yield return null;
                }
                
                // lunge at the player
                m_enemyAttack.Attack();
            } 
            else { 
                // carry out movement
                m_enemyMover.MoveOneTurn();           
            }
        }
    }

    public void InvokeDeathEvent() {
        if (m_isDead) {
            return;
        }
        m_isDead = true;
        deathEvent?.Invoke();
    }
}
