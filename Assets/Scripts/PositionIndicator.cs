using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PositionIndicator : NetworkBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    //Player Variables
    public bool isFinished = false;

    //World variables
    GameObject finishLine;

    //GUI variables
    public static int position;

    //Position calculation variables
    GameObject[] players;
    SortedList<float, GameObject> distances;
    float distance;

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called every frame
    void Update()
    {
        finishLine = GameObject.Find("FinishLine(Clone)"); //object in scene called FinishLine will be the finish line

        players = GameObject.FindGameObjectsWithTag("Player"); //reset the list of players each frame for when a new one joins (until a lobby manager is used)
        distances = new SortedList<float, GameObject>(); //reset the distances each player is from the start line every frame

        //only call these functions is finishLine has been assigned
        if (finishLine != null)
        {
            PlaceTracker();
            EndLineDetection();
            CmdEndGame();
        }
    }

    void EndLineDetection()
    {
        //if the object this script is attached to is the local player
        if (this.isLocalPlayer)
        {
            //if the player hasn't already finished
            if (isFinished == false)
            {
                //if the player has crossed the finish line
                if (gameObject.transform.position.x > finishLine.transform.position.x)
                {
                    isFinished = true; //the player has finished
                    Debug.Log("isFinished = true");
                    CmdAddToNumFinishedPlayers();
                }
            }
        }
    }

    [Command]
    void CmdAddToNumFinishedPlayers()
    {
        finishLine.GetComponent<FinishLineManager>().numFinishedPlayers += 1; //tells FinishLineManager script to increase the number of finished players
    }

    void PlaceTracker()
    {
        //if the player hasn't finished the level, find their position number
        if (isFinished == false)
        {
            //loop through array of players
            foreach (GameObject playerInPlayers in players)
            {
                distance = finishLine.transform.position.x - playerInPlayers.transform.position.x; //calculates distance along x axis between FinishLine object and the current player in the array
                distances.Add(distance, playerInPlayers); //adds calculated distance as key and the current player in loop as value
            }
            //loop through list of distances
            for (int i = 0; i < players.Length; i++)
            {
                //if the game object in the list we're looking at is the object this script is attached to (the local player)
                if (distances.Values[i] == gameObject)
                {
                    //if the object this script is attached to is the local player
                    if (this.isLocalPlayer)
                    {
                        position = i + 1; //position is the index we're currently looking at +1 because it starts at 0
                        Camera.main.GetComponent<PositionTextChanger>().position = i + 1; //send position number to PositionTextChanger which will edit the GUI element
                    }
                }
            }
        }
    }
    
    [Command]
    void CmdEndGame()
    {
        //if the number of dead players + the number of finished players = the total number of players then end the game
        if (finishLine.GetComponent<FinishLineManager>().numDeadPlayers + finishLine.GetComponent<FinishLineManager>().numFinishedPlayers == players.Length)
        {
            NetworkManager.singleton.ServerChangeScene("Endgame Screen");
        }
    }
}

