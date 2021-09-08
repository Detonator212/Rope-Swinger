using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LevelGenerator : NetworkBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    int levelLength = 30; //change this value to change the number of buildings in the level
    Vector3 oldPosition;
    Vector3 newPosition;
    GameObject newObject;
    public List<GameObject> buildings = new List<GameObject>(); //if you want to add new buildings, add them to this list via the Unity editor
    public GameObject finishLine;
    float groundLevel = -11.1f;

    [SyncVar]
    int levelSeed;
    bool generated = false;

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called every frame
    void Update()
    {
        //if the level hasn't been generated
        if (generated == false)
        {
            //if this code is running on the server, generate a seed
            if (isServer)
            {
                levelSeed = Random.Range(0, 1000); //pick seed (just a random number)
            }

            //if a level seed has been generated
            if (levelSeed != null)
            {
                Random.seed = levelSeed; //sets Unity's seed to the value of levelSeed
                GenerateLevel();
                generated = true; //the level has now been generated
            }
        }
    }

    void GenerateLevel()
    {
        oldPosition = gameObject.transform.position; //the starting position for the function is StartLine's position

        for (int i = 0; i < levelLength + 1; i++)
        {
            newPosition = oldPosition + new Vector3(30, 0, 0); //calculation of postion of where the next object will be placed (to change gap between buildings, change first number in the new Vector3)

            //if we've reached the last object of the level
            if (i == levelLength)
            {
                newObject = Instantiate(finishLine, newPosition, Quaternion.identity); //instantiate the FinishLine
            }
            else
            {
                newObject = Instantiate(buildings[Random.Range(0, buildings.Count - 1)], newPosition, Quaternion.identity); //instantiate a randomly selected building prefab
            }

            UniformLevel(newObject); //move building to correct y position
            oldPosition = newPosition; //update oldPosition for the next loop
        }
    }

    void UniformLevel(GameObject newObject)
    {
        float height = newObject.GetComponent<SpriteRenderer>().bounds.size.y; //height of building object
        float lowestCoord = newObject.transform.position.y - height / 2; //y coord of bottom of object

        //if object goes through ground
        if (lowestCoord < groundLevel)
        {
            float difference = groundLevel - lowestCoord; //finds distance between bottom of object and ground
            newObject.transform.position += new Vector3(0, difference); //moves object up to ground level
        }
        //if object floats above ground
        else if (lowestCoord > groundLevel)
        {
            float difference = lowestCoord - groundLevel; //finds distance between bottom of object and ground
            newObject.transform.position -= new Vector3(0, difference); //moves object down to ground level
        }
    }
}

