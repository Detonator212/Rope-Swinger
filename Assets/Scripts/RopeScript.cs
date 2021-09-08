using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RopeScript : NetworkBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    //Variables from PlayerScript
    [SyncVar]
    public NetworkInstanceId playerNetID;
    [SyncVar]
    public Vector2 clickPosition;

    //The player object this rope belongs to
    GameObject player;

    //Rope variables
    float segmentLength = 0.5f;
    List<Node> Nodes = new List<Node>();
    List<Segment> Segments = new List<Segment>();
    Vector2 acceleration = new Vector2(0, -9.8f); //second number defines gravity strength

    //World variables
    float groundLevel = -11f; //y coordinate of ground

    class Node
    {
        public Vector2 oldPosition;
        public Vector2 currentPosition;

        public Node(Vector2 IcurrentPosition)
        {
            oldPosition = IcurrentPosition;
            currentPosition = IcurrentPosition;
        }
    }

    //a segment is a pair of nodes
    class Segment
    {
         public Node node1;
         public Node node2;
         public float length;

        public Segment(Node Inode1, Node Inode2, float Ilength)
        {
            node1 = Inode1;
            node2 = Inode2;
            length = Ilength;
        }
    }

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called once when the rope is instantiated
    void Start()
    {
        player = ClientScene.FindLocalObject(playerNetID); //finds this script's player in the scene using netId
        CreateRope();
    }

    //this function is called every frame
    void Update()
    {
        if (this.hasAuthority) //if the object running this code has authority 
        {
            //if a rope is no longer active then destroy this rope
            if (player.GetComponent<PlayerScript>().isRopeActive == false)
            {
                CmdDestroyRope();
            }
        }

        UpdateNodes();

        //run UpdateSegments() multiple times to improve physics stability
        for (int i = 0; i < 50; i++)
        {
            UpdateSegments();
        }

        RenderRope();
    }

    void CreateRope()
    {
        Vector2 unitVector = (clickPosition - (Vector2)player.transform.position).normalized; //create vector of magnitude 1 pointing from player's position to click position
        Nodes.Add(new Node(player.transform.position)); //add a new rope node to the rope nodes list which is the player's position

        while ((Vector2.Distance(Nodes[Nodes.Count - 1].currentPosition, clickPosition) > segmentLength)) //while the distance between the last node in nodes and the click position is greater than the segmentLength
        {
            Nodes.Add(new Node(Nodes[Nodes.Count - 1].currentPosition + unitVector * segmentLength)); //add a new node to nodes which is the segmentLength away from the last node in the direction of the unitVector
        }
        Nodes.Add(new Node(clickPosition)); //add a new node to nodes which is at the click position

        for (int i = 0; i < Nodes.Count - 1; i++) //loop through nodes list
        {
            Segments.Add(new Segment(Nodes[i], Nodes[i + 1], Vector2.Distance(Nodes[i].currentPosition, Nodes[i + 1].currentPosition))); //create a segment out of a pair of nodes
        }
    }

    void RenderRope()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>(); //temporary variable which represents the LineRenderer component
        lineRenderer.positionCount = Nodes.Count; //the number of nodes in the Nodes list is the number of vertices the LineRenderer will have
        Vector3[] nodePositions = new Vector3[Nodes.Count]; //defines array which will contain the positions of each node in the Nodes list

        //add position of each node in the Nodes list to the nodePositions array
        for (int i = 0; i < Nodes.Count; i++)
        {
            nodePositions[i] = (Nodes[i].currentPosition);
        }

        lineRenderer.SetPositions(nodePositions); //sets the positions of the LineRenderer's vertices to be the positions of the nodes
    }

    void UpdateNodes()
    {
        //loop through every node and apply gravity
        for (int i = 0; i < Nodes.Count; i++)
        {
            Node currentNode = Nodes[i]; //temporary Node object which represents the node we're currently dealing with in the loop
            Vector2 newPosition = currentNode.currentPosition + (currentNode.currentPosition - currentNode.oldPosition) + acceleration * Time.deltaTime * Time.deltaTime; //calculates the new position for the node
            currentNode.oldPosition = currentNode.currentPosition; //the current node's currentPosition from the last frame becomes the oldPosition

            //if the node is about touch or go below the ground, put it at ground level
            if (newPosition.y <= groundLevel)
            {
                newPosition.y = groundLevel;
            }

            currentNode.currentPosition = newPosition; //move the current node to the calculated new position
        }
    }

    void UpdateSegments()
    {
        //loop through every segment and put every segment's pair of nodes to the desired distance apart
        for (int i = 0; i < Segments.Count; i++)
        {
            Segment currentSegment = Segments[i]; //temporary Segment object which represents the segment we're currently dealing with in the loop
            
            float distance = Vector2.Distance(currentSegment.node1.currentPosition, currentSegment.node2.currentPosition); //temp variable storing actual distance between nodes of current segment
            float percentage = (distance - currentSegment.length) / currentSegment.length / 2; //temp variable storing by what percentage each node is away from where they should be
            Vector2 offset = (currentSegment.node2.currentPosition - currentSegment.node1.currentPosition) * percentage; //temp variable storing the vector which each node should be moved by
            
            currentSegment.node1.currentPosition = currentSegment.node1.currentPosition + offset; //move the first node of the segment
            currentSegment.node2.currentPosition = currentSegment.node2.currentPosition - offset; //move the second node of the segment
        }

        Nodes[0].currentPosition = player.transform.position; //sets first node to player's position
        Nodes[Nodes.Count - 1].currentPosition = clickPosition; //sets last node to clickPosition
    }

    [Command] //this code will be executed on the server
    void CmdDestroyRope()
    {
        Destroy(gameObject); //destroy the object this script is attached to
    }


}
