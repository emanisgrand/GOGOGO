using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{ 
    // local direction
    public Vector3 directionToSearch = new Vector3(0f, 0f, 2f);

    // node being sensed
    Node m_nodeToSearch;

    // ref to board
    Board m_board;

    // found player?
    bool m_foundPlayer = false;

    public bool FoundPlayer
    {
        get { return m_foundPlayer; }
    }

    void Awake()
    {
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
    }

    // check if the player's been sensed
    public void UpdateSensor()
    {
    // convert local direction to world space position
        Vector3 worldSpacePositionToSearch = transform.TransformVector(directionToSearch)
                                             + transform.position;

        if (m_board != null)
        {
            // find the node at the world space position to search 
            m_nodeToSearch = m_board.FindNodeAt(worldSpacePositionToSearch);

            // if the node is the playerNode than the player has been found
            if (m_nodeToSearch == m_board.PlayerNode) m_foundPlayer = true;
        }
    }

    // for testing purposes only
    // void Update() {
    //     UpdateSensor();
    // }
}