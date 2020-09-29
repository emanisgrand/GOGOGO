using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : MonoBehaviour
{
    // References to the PlayerMover and PlayerInput scripts.
    public PlayerMover playerMover;
    public PlayerInput playerInput;
    public GameManager gameManager;

    private void Awake()
    {
        // Cached references to the PlayerMover and PlayerInput scripts
        playerMover = GetComponent<PlayerMover>();
        playerInput = GetComponent<PlayerInput>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        
        // Make sure input is enabled when we start
        playerInput.InputEnabled = true;
    }

    void Update()
    {
        // if the player is currently moving, ignore any input
        if (playerMover.isMoving && !gameManager.IsGameOver) { return; } 
        
        // get the keyboard input
        playerInput.GetKeyInput();
    
        // tie the player input to the PlayerMover's Move methods
        if (playerInput.V == 0)
        {
            if (playerInput.H < 0)
            {
                playerMover.MoveLeft();
            } else if (playerInput.H > 0)
            {
                playerMover.MoveRight();
            }
        } else if (playerInput.H == 0)
        {
            if (playerInput.V < 0)
            {
                playerMover.MoveBackward();
            } else if (playerInput.V > 0)
            {
                playerMover.MoveForward();
            }
        }
    }
}
