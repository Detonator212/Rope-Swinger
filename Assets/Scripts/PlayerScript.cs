using UnityEngine;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    //Player variables
    Vector2 oldPosition;
    Vector2 currentPosition;
    Vector2 newPosition;
    Vector2 acceleration = new Vector2(0, -12f); //second number defines gravity strength (will need to be changed in PlayerMovement() too)
    bool isDead = false;

    //Rope variables
    public bool isRopeActive = false;
    Vector2 clickPosition;
    float ropeLength;
    GameObject currentRope;
    public GameObject ropePrefab;

    //World variables
    float groundLevel = -10f; //y coordinate of ground
    GameObject finishLine;

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called once before the first frame
    void Start()
    {
        if (this.isLocalPlayer) //if the object running this script is the local player
        {
            Camera.main.GetComponent<CameraScript>().player = gameObject; //tells camera to follow the object running this script
        }
    }

    //this function is called every frame
    void Update()
    {
        finishLine = GameObject.Find("FinishLine(Clone)"); //object in scene called FinishLine will be the finish line
        //finishLine will be used for changing the values of numDeadPlayers and numFinishedPlayers

        //only run the next bit of code if finishLine has been assigned
        if (finishLine != null)
        {
            if (isDead == false) //if the player isn't dead
            {
                PlayerMovement();
            }
        }
    }

    void PlayerMovement()
    {
        if (this.isLocalPlayer) //checks if the object running this code is the local player
        {
            currentPosition = transform.position; //sets currentPosition to the position of the object this script is attached to (the player object)
            newPosition = currentPosition + (currentPosition - oldPosition) + acceleration * Time.deltaTime * Time.deltaTime; //calculates the new position for the player

            //### RESET BOOST ###
            acceleration = new Vector2(0, -12f); //resets acceleration to default (change second number to change strenghth of gravity)

            oldPosition = currentPosition; //currentPosition from the last loop becomes the oldPosition

            if (Input.GetMouseButtonDown(0)) //if the left mouse button is being pressed
            {
                if (isRopeActive == false) //if a rope is not active
                {
                    clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //sets clickPosition to be the world position of the mouse click
                    RaycastHit2D hit = Physics2D.Raycast(clickPosition, new Vector2(0, 0)); //temporary variable which stores whether clickPosition is within a collider

                    if (hit.collider != null) //if the click position is in a building
                    {
                        ropeLength = Vector2.Distance(clickPosition, currentPosition); //sets length to be the distance between the clickPosition and the player's position
                        CmdRopeSpawn(clickPosition);
                        isRopeActive = true; //this indicates that a rope is now active
                    }
                }
                else
                {
                    isRopeActive = false; //this indicates that a rope is no longer active
                }
            }

            //this function stops the player falling through the ground
            GroundDetection();
            //this function keeps the player within a circle around where they have clicked if a rope is active
            ConstrainPlayer();

            transform.position = newPosition; //moves the player to the calculated newPosition
        }
    }

    void ConstrainPlayer()
    {
        if (isRopeActive == true) //if a rope is active (the player should be swinging)
        {
            if (Vector2.Distance(clickPosition, newPosition) > ropeLength) //if the distance between the player's next position and where they clicked is larger than it was at the time of clicking
            {
                //### BOOST ###
                //if the player is at the edge of the circle limit around the clickPosition and they are travelling to the right
                if ((currentPosition.x >= clickPosition.x - 1) && (currentPosition.x <= clickPosition.x + 1) && oldPosition.x < newPosition.x)
                {
                    acceleration += new Vector2(60f, 0f); //boost of acceleration (change the first number to change strength of boost)
                }

                Vector2 tempVector = newPosition - clickPosition; //temporary vector pointing from the click position to the player's next position
                float percentage = ropeLength / Vector2.Distance(clickPosition, newPosition); //the actual distance between the player's next position and the click position as a percentage of length
                newPosition = clickPosition + tempVector * percentage; //player's next position is moved within the boundaries of the circle around the click position
            }
        }
    }

    void GroundDetection()
    {
        if (isDead == false) //if the player is not dead
        {
            if (newPosition.y <= groundLevel) //if the player has hit or gone below the ground
            {
                isDead = true; //sets the player to be dead

                newPosition.y = groundLevel; //sets the player's y coordinate to be ground level

                //if the player hasn't already finished the level
                if (gameObject.GetComponent<PositionIndicator>().isFinished == false)
                {
                    Camera.main.GetComponent<DeathIndicator>().isDead = true; //tells DeathIndicator script to display that the player is dead on UI

                    CmdAddToNumDeadPlayers();
                }
            }
        }
    }

    [Command] //this code will be executed on the server
    void CmdAddToNumDeadPlayers()
    {
        finishLine.GetComponent<FinishLineManager>().numDeadPlayers += 1; //tells FinishLineManager script to increase the number of dead players
    }

    [Command] //this code will be executed on the server
    private void CmdRopeSpawn(Vector2 clickPosition)
    {
        currentRope = GameObject.Instantiate(ropePrefab); //instantiate the Rope object
        currentRope.GetComponent<RopeScript>().clickPosition = clickPosition; //set the click position of RopeScript to be what is in this script
        currentRope.GetComponent<RopeScript>().playerNetID = gameObject.GetComponent<NetworkIdentity>().netId; //send player's netId to RopeScript
        NetworkServer.SpawnWithClientAuthority(currentRope, gameObject); //instantiate the Rope object in all instances
    }
}