using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    // current target location
    public Vector3 destination;

    // option to face the direction of movement
    public bool faceDestination = false;
    
    // whether or not this is moving
    public bool isMoving = false;

    // time it takes to rotate toward destination
    public float rotateTime = 0.5f;
    
    // what easetype to use for iTweening
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    // how fast this goes
    public float moveSpeed = 1.5f;
    
    // the delay before any call to iTween
    public float iTweenDelay = 0f;

    protected Board m_board;
    
    // the current Node this is on
    protected Node m_currentNode;
    public Node CurrentNode => m_currentNode;

    public UnityEvent OnMovementFinished;
    
    // setup the Mover
    protected  virtual void Awake() {
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
    }
    
    protected virtual void Start() {
        // keep the current node up to date
        UpdateCurrentNode();
    }

    // public MoveRoutine invokation  
    public void Move(Vector3 destinationPos, float delayTime = 0.24f)
    {
        if (isMoving)
        {
            return;
        }
        
        // move only if the destination is at a valid Node
        if (m_board != null) {
            Node targetNode = m_board.FindNodeAt(destinationPos);
            
            // kickoff the coroutine MoveRoutine
            if (targetNode != null && m_currentNode != null 
                                  && m_currentNode.LinkedNodes.Contains(targetNode)) {
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
            } else {
                Debug.Log("MOVER Error: current Node not connected to target Node");
            }
        }
    }
    
    // coroutine that processes movement
    protected virtual IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime) {
        
        // we're on the move!
        isMoving = true;
        
        // set the destination to the destinationPos being passed into the coroutine
        destination = destinationPos;

        if (faceDestination) {
            FaceDestination();
            yield return new WaitForSeconds(0.25f);
        }
        
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

        while (Vector3.Distance(destinationPos, transform.position) > 0.01) {
            yield return null;
        }
        
        // Stop the iTween
        iTween.Stop(gameObject);
        
        // explicitly set the player position to the destination
        transform.position = destinationPos;
        
        // we're no longer moving
        isMoving = false;

        UpdateCurrentNode();
        
    }
    
    // move one space along negative X
    public void MoveLeft() {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        Move(newPosition, 0f);
    }
    
    // move one space along positive X
    public void MoveRight() {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        Move(newPosition, 0f);
    }
    
    // move one space along positive Z
    public void MoveForward() {
        Vector3 newPosition = transform.position + new Vector3(0, 0, Board.spacing);
        Move(newPosition, 0);
    }
    
    // move one space along negative Z
    public void MoveBackward() {
        Vector3 newPosition = transform.position + new Vector3(0, 0, -Board.spacing);
        Move(newPosition, 0);
    }

    // run the Node field's update
    protected void UpdateCurrentNode() {
        m_currentNode = m_board?.FindNodeAt(transform.position);
    }
    
    // turn to face direction of movement 
    protected void FaceDestination() {
        // movement direction
        Vector3 relativePostion = destination - transform.position;
        
        // convert relativePosition to a quaternion
        Quaternion newRotation = Quaternion.LookRotation(relativePostion, Vector3.up);
        
        // euler angle y of component
        float newY = newRotation.eulerAngles.y;
        
        // rotate with iTween
        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", easeType,
            "time", rotateTime
        ));
    }
}
