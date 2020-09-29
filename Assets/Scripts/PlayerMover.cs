using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

public class PlayerMover : MonoBehaviour
{
    // the player's current target location
    public Vector3 destination;

    // whether or not the player is moving
    public bool isMoving = false;

    // what easetype to use for iTweening
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    // how fast the player moves
    public float moveSpeed = 1.5f;
    
    // the delay before any call to iTween
    public float iTweenDelay = 0f;

    private Board m_board;
    private bool _isBoardNotNull;

    // Used for initialization
    void Awake()
    {
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
    }
    
    void Start()
    {
        _isBoardNotNull = m_board != null;
        UpdateBoard();
    }

    // IEnumerator Test()
    // {
    //     yield return new WaitForSeconds(1f);
    //     MoveRight();
    //     yield return new WaitForSeconds(2f);
    //     MoveRight();
    //     yield return new WaitForSeconds(2f);
    //     MoveForward();
    //     yield return new WaitForSeconds(2f);
    //     MoveForward();
    // }
    
    // invoke the MoveRoutine through a public method.
    public void Move(Vector3 destinationPos, float delayTime = 0.24f)
    {
        // move only if the destination is at a valid Node
        if (_isBoardNotNull) {
            Node targetNode = m_board.FindNodeAt(destinationPos);
            
            if (targetNode != null && m_board.PlayerNode.LinkedNodes.Contains(targetNode))
            {
                // start the couroutine MoveRoutine
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
            }
        }
        
    }
    
    // the coroutine that moves the player
    IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        // we're on the move!
        isMoving = true;
        
        // set the destination to the destinationPos being passed into the coroutine
        destination = destinationPos;
        
        // briefly pause the coroutine 
        yield return new WaitForSeconds(delayTime);
        
        // Move toward the destinationPos using the easeType and moveSpeed vars
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easeType", easeType,
            "speed", moveSpeed
         ));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01) 
        {
            yield return null;
        }
        
        // Stop the iTween
        iTween.Stop(gameObject);
        
        // explicitly set the player position to the destination
        transform.position = destinationPos;
        
        // we're no longer moving
        isMoving = false;
        
        UpdateBoard();
    }
    
    // move one space along negative X
    public void MoveLeft()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        Move(newPosition, 0f);
    }
    
    // move one space along positive X
    public void MoveRight()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        Move(newPosition, 0f);
    }
    
    // move one space along positive Z
    public void MoveForward()
    {
        Vector3 newPosition = transform.position + new Vector3(0, 0, Board.spacing);
        Move(newPosition, 0);
    }
    
    // move one space along negative Z
    public void MoveBackward()
    {
        Vector3 newPosition = transform.position + new Vector3(0, 0, -Board.spacing);
        Move(newPosition, 0);
    }

    public void UpdateBoard()
    {
        if (_isBoardNotNull)
        {
            m_board.UpdatePlayerNode();
        }
    }
    
}
