using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Board : MonoBehaviour {
    // Uniform distance between nodes
    public static float spacing = 2f;

    // four cardinal directions
    public static Vector2[] directions =
    {
        new Vector2(spacing, 0),
        new Vector2(-spacing, 0f),
        new Vector2(0f, spacing),
        new Vector2(0f, -spacing)
    };
   
    // all of the nodes on the board
    List<Node> m_allNodes = new List<Node>();
    public List<Node> AllNodes => m_allNodes;

    // the node directly under the player
    private Node m_playerNode;
    public Node PlayerNode => m_playerNode;

    // the Node representing the end of the maze
    private Node m_goalNode;
    public Node GoalNode => m_goalNode;

    public GameObject goalPrefab;
    public float drawGoalTime = 2f;
    public float drawGoalDelay = 2f;
    public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeInOutSine;
    
    private PlayerMover m_playerMover;
    private bool _ismPlayerMoverNotNull;

    private void Start()
    {
        _ismPlayerMoverNotNull = m_playerMover != null;
    }

    void Awake(){
        m_playerMover = FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
        GetNodeList();

        m_goalNode = FindGoalNode();
    }

    public void GetNodeList(){
        // create a Node [array] that finds all objects of type <Node>
        Node[] nList = FindObjectsOfType<Node>();
        // convert the array to a list using the New List constructor, passing in the array to handle the conversion.
        m_allNodes = new List<Node>(nList);
    }

    public Node FindNodeAt(Vector3 pos){
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return m_allNodes.Find(n => n.Coordinate == boardCoord);
    }

    Node FindGoalNode(){
        return m_allNodes.Find(n => n.isLevelGoal);
    }

    public Node FindPLayerNode(){
        if (_ismPlayerMoverNotNull && !m_playerMover.isMoving)
        {
            return FindNodeAt(m_playerMover.transform.position);
        }
        return null;
    }

    public void UpdatePlayerNode(){
        m_playerNode = FindPLayerNode();
    }
    
    private void OnDrawGizmos(){
        Gizmos.color = new Color(0f,1f, 1f, 0.5f);
        
        if (m_playerNode != null)
        {
            Gizmos.DrawSphere(m_playerNode.transform.position, 0.2f);
        }
    }

    public void DrawGoal() {
        if (goalPrefab != null && m_goalNode != null)
        {
            GameObject goalInstance = Instantiate(goalPrefab, m_goalNode.transform.position, Quaternion.identity);
            
            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType
                ));
        }
    }

    public void InitBoard() {
        //m_playerNode?.InitNode();
        if (m_playerNode != null) {
            m_playerNode.InitNode();
        }
    }
}
