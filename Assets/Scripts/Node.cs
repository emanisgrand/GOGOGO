using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // (x,z) coordinate on the Board, Coordinate property always returns a rounded number
    private Vector2 m_coordinate;
    public Vector2 Coordinate { get{ return Utility.Vector2Round(m_coordinate); } }
    
    // list of nodes that are adjacent based on Board spacing.
    private List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; }}  //=> m_neighborNodes;

    // list of nodes that this node is already linked to
    private List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get{ return m_linkedNodes; }}

    // reference to the Board component
    Board m_Board;
    
    // reference to the node's mesh
    [SerializeField] GameObject geometry;

    public GameObject linkPrefab;
    
    // time for the scale animation to play
    [SerializeField] float scaleTime = 0.3f;

    // ease in-out for the animations
    [SerializeField] iTween.EaseType easeType = iTween.EaseType.easeInExpo;
    
    // time delay for the animation
    [SerializeField] float delay = 1f;

    // whether or not the node has been initialized
    bool m_isInitialized = false;

    public LayerMask obstacleLayer;

    public bool isLevelGoal = false;
    
    void Awake()
    {
        // locate the reference to the Board component
        m_Board = FindObjectOfType<Board>();
        
        // set the coordinate using the transform's x and z values
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
    }

    void Start()
    {
        // start with the mesh hidden by scaling it to zero
        if (geometry != null)
        {
            geometry.transform.localScale = Vector3.zero;
        }
    
        // find all neighboring nodes
        if (m_Board != null)
        {
            m_neighborNodes = FindNeighbors(m_Board.AllNodes);
        }
    }

    public void ShowGeometry()
    {
        if (geometry != null)
        {
            iTween.ScaleTo(geometry, iTween.Hash(
            "time", scaleTime,
            "scale", Vector3.one,
            "easetype", easeType,
            "delay", delay
            ));
        }
    }
    
    // given a list of Nodes, return a subset of the list that are neighbors
    public List<Node> FindNeighbors(List<Node> nodes)
    {
        // temporary list of nodes to return
        List<Node> nList = new List<Node>();
        
        // loop through all of the Board directions
        foreach (Vector2 dir in Board.directions)
        {
            // find a node neighbor at the current direction...
            Node foundNeighbor = FindNeighborAt(nodes, dir);  
            
            // if there is a neighbor at this direction, add it to the list
            if (foundNeighbor != null && !nList.Contains(foundNeighbor))
            {
                nList.Add(foundNeighbor);
            }
        }
        // return our temporary list
        return nList;
    }

    Node FindNeighborAt(List<Node> nodes, Vector2 dir)
    {
        return nodes.Find(n => n.Coordinate == Coordinate + dir);
    }
    // overload and pass in the found neighbor nodes from the method above. 
    public Node FindNeighborAt(Vector2 dir)
    {
        return FindNeighborAt(NeighborNodes, dir);
    }
    
    
    public void InitNode()
    {
        // if the node isn't active...
        if (!m_isInitialized) {
            
            // Show the mesh geometry
            ShowGeometry();
            
            // Signal neighbor to do the same
            InitNeighbors();
            
            // Set initialized state to true
            m_isInitialized = true;
        }
    }
    
    void InitNeighbors()
    {
        StartCoroutine("InitNeighborsRoutine");
    }

    IEnumerator InitNeighborsRoutine()
    {
        // dramatic pause between 
        yield return new WaitForSeconds(delay);

        // run initNode on each neighboring node if they're not already linked
        foreach (Node n in m_neighborNodes)
        {
            if (!m_linkedNodes.Contains(n))
            {
                Obstacle obstacle = FindObstacle(n);
                if (obstacle == null) {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    // draw a link from this node to the target node
    void LinkNode(Node targetNode)
    {
        if (linkPrefab != null)
        {
            // instantiate the prefab and parent it to this node
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;
            
            // draw the link
            Link link = linkInstance.GetComponent<Link>();
            if (link != null){
                link.DrawLink(transform.position, targetNode.transform.position);
            }
            
            // track which nodes have already been linked to other nodes 
            if (!m_linkedNodes.Contains(targetNode)) {
                m_linkedNodes.Add(targetNode);
            }
            
            if (!targetNode.LinkedNodes.Contains(this)) {
                targetNode.LinkedNodes.Add(this);
            }
        }
    }

    Obstacle FindObstacle(Node targetNode)
    {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;
        
        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, 
            Board.spacing + 0.1f, obstacleLayer))
        {
            //Debug.Log("NODE FindObstacle: Hit an obstacle from " + this.name + " to " + targetNode.name);
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }
}

