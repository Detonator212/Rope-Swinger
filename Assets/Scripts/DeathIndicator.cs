using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathIndicator : MonoBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    public bool isDead;
    public TextMeshProUGUI isDeadText;

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called every frame
    void Update()
    {
        //if the player is dead
        if (isDead == true)
        {
            isDeadText.text = "DEAD"; //change GUI element contents to "DEAD"
        }
    }
}


