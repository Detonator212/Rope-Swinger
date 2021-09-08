using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    public GameObject player;

    //CODE --------------------------------------------------------------------------------------------------------------

    void Update() //called every frame
    {
        if (player != null) //checks if the player variable has been assigned
        {
            transform.position = player.transform.position + new Vector3(10, 0, -10); //sets position of the camera to position of the player + an offset
        }
    }
}

