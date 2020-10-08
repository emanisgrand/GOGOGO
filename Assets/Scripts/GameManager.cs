using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Serializable]
    public enum Turn
    {
        Player,
        Enemy
    }
    // reference to the GameBoard 
    Board m_board;
    
    // reference to the PlayerManager
    PlayerManager m_player;

    List<EnemyManager> m_enemies;

    Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn => m_currentTurn;

    // has start been pressed?
    bool m_hasLevelStarted = false;
    public bool HasLevelStarted { get => m_hasLevelStarted; set => m_hasLevelStarted = value; }
    
    // has gameplay begun?
    bool m_isGamePlaying = false;
    public bool IsGamePlaying { get => m_isGamePlaying; set => m_isGamePlaying = value; }
    
    // have we met the conditions for a game over?
    bool m_isGameOver = false;
    public bool IsGameOver { get => m_isGameOver; set => m_isGameOver = value; }
    
    // have the end level graphics finished animating?
    bool m_hasLevelFinished = false;
    public bool HasLevelFinished { get => m_hasLevelFinished; set => m_hasLevelFinished = value; }

    // delay between start level and play level routines
    public float delay = 1f;

    // all events invoked for Setup, StartLevel, PlayLevel, and EndLevel coroutines
    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;
    
    void Awake() {
        // populate the Board and PlayerManager components
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
        m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        m_enemies = FindObjectsOfType<EnemyManager>().ToList();  // automatically converts
    }

    void Start() {
        // start the main game loop if the PlayerManager and Board are present
        if (m_board != null && m_player != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.Log("GAMEMANAGER Error: no player or board found!");
        }
    }

    IEnumerator RunGameLoop() {
        // The main game loop to run, separated into different stages/coroutines
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    // the initial stage after the level has been loaded
    IEnumerator StartLevelRoutine() {
        Debug.Log("SETUP LEVEL");
        //
        // if (setupEvent != null)
        // {
        //     setupEvent.Invoke();
        // }
        //
        
        setupEvent?.Invoke();
        //setupEvent?.Invoke();
        
        Debug.Log("START LEVEL");
        m_player.PlayerInput.InputEnabled = false;
        while (!m_hasLevelStarted) {
            // show start screen
            // user presses button to start
            // HasLevelStarted = true
            yield return null;
        }

        // trigger events when we press the start button
        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
        
    }
    
    // gameplay stage
    IEnumerator PlayLevelRoutine(){
        Debug.Log("PLAY LEVEL");
        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.PlayerInput.InputEnabled = true;

        // trigger any events as we start playing the level
        playLevelEvent?.Invoke();
        // if (playLevelEvent != null){
        //     playLevelEvent.Invoke();
        // }
        
        while (!m_isGameOver) {
            // pause for a single frame
            yield return null;
            
            // check for level winning condition
            m_isGameOver = IsWinner();
            
            // check for the lose condition
            // lose
            // player dies
            // m_isGameOver = true
        }
        // Debug.Log("WIN! ============================");
    }

    public void LoseLevel() {
        StartCoroutine(nameof(LoseLevelRoutine));
    }

    IEnumerator LoseLevelRoutine() {
        // the game is over
        m_isGameOver = true;
        
        // wait for a short delay, and then...
        yield return new WaitForSeconds(1.5f);
        
        // invoke the loseLevelEvent
        loseLevelEvent?.Invoke();
        
        yield return new WaitForSeconds(2f);
        
        Debug.Log("YOU LOSE! =============================");
        
        RestartLevel();
    }
    
    // end the level after gameplay is complete
    IEnumerator EndLevelRoutine() {
        Debug.Log("END LEVEL");
        m_player.PlayerInput.InputEnabled = false;

        endLevelEvent?.Invoke();
        // show the end screen
        while (!m_hasLevelFinished)
        {
            // user presses the button to continue
            
            // HasLevelFinished = true
            yield return null;    
        }
        
        // reload the current scene
        RestartLevel();
    }

    // restart the current level
    void RestartLevel() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // atach to StartButton, triggers PlayLevelRoutine
    public void PlayLevel() {
        m_hasLevelStarted = true;
    }

    // Has the player reached the goal node?
    private bool IsWinner()
    {
        // the null propagation operator below makes the player node == the goal node.
        // return m_board?.PlayerNode ?? m_board.GoalNode;
        if (m_board.PlayerNode != null) {
            return (m_board.PlayerNode == m_board.GoalNode);
        }
        return false;
    }

    void PlayPlayerTurn() {
        m_currentTurn = Turn.Player;
        m_player.IsTurnComplete = false;
        
        // allow player to move
    }
    
    // switch to Enemy turn
    void PlayEnemyturn() {
        m_currentTurn = Turn.Enemy;
        
        foreach (var enemy in m_enemies) 
        {
            if (!enemy.IsDead) {
                enemy.IsTurnComplete = false;
                enemy.PlayTurn();
            }
        }
    }
    
    // are all enemies done with their turn?
    bool IsEnemyTurnComplete() {
        foreach (var enemy in m_enemies)
        {
            if (enemy.IsDead) continue;
            if (!enemy.IsTurnComplete) return false;
        }
        return true;
    }

    // switch between the Player and Enemy turns
    public void UpdateTurn() {
        if (m_currentTurn == Turn.Player && m_player != null) 
        {
            if (m_player.IsTurnComplete && !AreAllEnemiesCaptured()) { PlayEnemyturn(); }
        } 
        else if (m_currentTurn == Turn.Enemy)
        {
            if (IsEnemyTurnComplete())
            {
                PlayPlayerTurn();
            }
        }
    }

    bool AreAllEnemiesCaptured() {
        foreach (var enemy in m_enemies) {
            if (!enemy.IsDead) return false;
        }
        return true;
    }
    
}
