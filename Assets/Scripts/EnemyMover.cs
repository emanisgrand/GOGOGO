using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Stationary, 
    Patrol,
    Spinner
}
public class EnemyMover : Mover
{
    // default to local positive z. The local direction to move in. 
    public Vector3 directionToMove = new Vector3(0f,0f, Board.spacing);

    // current movement mode
    public MovementType movementType = MovementType.Stationary;
    
    // wait time for stationary monsters
    public float standTime = 1f;

    Animator m_animator;
    
    protected override void Awake() {
        base.Awake();
        m_animator = GetComponent<Animator>();
        // faceDestination = true;
    }
    
    protected override void Start() {
        base.Start();
    }

    // finish one movement turn
    public void MoveOneTurn()
    {
        switch (movementType)
        {
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Stationary:
                Stand();
                break;
            case MovementType.Spinner:
                Spin();
                break;
        }
    }

    void Patrol() {
        StartCoroutine(nameof(PatrolRoutine));
    }

    IEnumerator PatrolRoutine() {
        // starting position cached
        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f,
            m_currentNode.Coordinate.y);
        
        // one space ahead
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);
        
        // two spaces ahead
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);
        
        // get to the new destination
        Move(newDest, 0f);
        
        // wait until movement has completed
        while (isMoving)
        {
            yield return null;
        }
        
        // check for a dead end
        if ( m_board != null) {
            // destination node
            Node newDestNode = m_board.FindNodeAt(newDest);
            
            // node two spaces away
            Node nextDestNode = m_board.FindNodeAt(nextDest);
    
            // if the Node two spaces away doesn't exist OR isn't connected to the destination Node
            if (nextDestNode == null || !newDestNode.LinkedNodes.Contains(nextDestNode)) {
                // turn and face the original node and set it as the new destination
                destination = startPos;
                FaceDestination();
                
                // wait for rotation to end
                yield return  new WaitForSeconds(rotateTime);
            }
        }
        
        // broadcast the end of movement
        OnMovementFinished.Invoke();
    }
    
    void Stand()
    {
        StartCoroutine(nameof(StandRoutine));
    }

    IEnumerator StandRoutine()
    {
        yield return new WaitForSeconds(standTime);
        
        base.OnMovementFinished.Invoke();
    }

    void Spin()
    {
        StartCoroutine(nameof(SpinRoutine));
    }

    IEnumerator SpinRoutine() {
        // local z forward
        Vector3 localForward = new Vector3(0f,0f,Board.spacing);
        
        // the destination will always be one space directly behind
        destination = transform.TransformVector(localForward * -1f) + transform.position;
        
        // rotate 180 degrees
        FaceDestination();

        // wait until the end of the the rotation
        yield return new WaitForSeconds(rotateTime);
        
        // broadcast the end of movement
        OnMovementFinished.Invoke();
    }
    
}