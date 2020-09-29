using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    // reference to the GameBoard 
    Board m_board;
    
    // reference to the PlayerManager
    PlayerManager m_player;

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

    // delay between the game stages
    public float delay = 1f;

    // all events invoked for Setup, StartLevel, PlayLevel, and EndLevel coroutines
    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    
    void Awake() {
        // populate the Board and PlayerManager components
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
        m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
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
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }
        //setupEvent?.Invoke();
        Debug.Log("START LEVEL");
        m_player.playerInput.InputEnabled = false;
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
        m_player.playerInput.InputEnabled = true;

        // trigger any events as we start playing the level
        playLevelEvent?.Invoke();
        // if (playLevelEvent != null){
        //     playLevelEvent.Invoke();
        // }
        
        while (!m_isGameOver){
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
    
    // end the level after gameplay is complete
    IEnumerator EndLevelRoutine() {
        Debug.Log("END LEVEL");
        m_player.playerInput.InputEnabled = false;

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
        if (m_board.PlayerNode != null)
        {
            return (m_board.PlayerNode == m_board.GoalNode);
        }
        return false;
    }
}
