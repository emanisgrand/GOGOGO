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
        new Vector2(spacing, 0f),
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
    
    // xyz position of the captured enemy pieces
    public List<Transform> capturePositions;
    // index for the current empty slot in the captured pieces list
    int m_currentCapturePosition = 0;
        public int CurrentCapturePosition { get => m_currentCapturePosition; 
            set => m_currentCapturePosition = value; }
    
    // visualizations for the capture positions
    [SerializeField] float m_emptySize = 0.4f;
    [SerializeField] private Color m_emptyColor = Color.blue;
    
    // establish a call to the PlayerMover script
    PlayerMover m_playerMover;

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
        if ( m_playerMover != null && !m_playerMover.isMoving)
        {
            return FindNodeAt(m_playerMover.transform.position);
        }
        return null;
    }

    public List<EnemyManager> FindEnemiesAt(Node node) {
        
        List<EnemyManager> foundEnemies = new List<EnemyManager>();
        EnemyManager[] enemies = FindObjectsOfType<EnemyManager>();

        foreach (var enemy in enemies) {
            // create access to the EnemyMover component attached to each object in the enemies array
            EnemyMover mover = GetComponent<EnemyMover>();
            
            // add the enemy on the passed in node to the enemies list
            if (mover.CurrentNode == node) {
                foundEnemies.Add(enemy);
            }
        }
        return foundEnemies;
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
        Gizmos.color = m_emptyColor;
        foreach (var empty in capturePositions) {
            Gizmos.DrawCube(empty.position, Vector3.one * m_emptySize);      
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
        m_playerNode?.InitNode();
    }
}
