using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameScreenTextChanger : MonoBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    int position;
    public TextMeshProUGUI guiText;

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called once before the first frame
    void Start()
    {
        position = PositionIndicator.position; //set local value for position to value from PositionIndicator

        string endGameScreenText = position.ToString();
        if (endGameScreenText == "1")
        {
            endGameScreenText = "You finished 1st!";
        }
        else if (endGameScreenText == "2")
        {
            endGameScreenText = "You finished 2nd!";
        }
        else if (endGameScreenText == "3")
        {
            endGameScreenText = "You finished 3rd";
        }
        else
        {
            endGameScreenText = "You finished 4th";
        }
        //if more players allowed in future, simply add more if statements

        guiText.text = endGameScreenText;
    }
}

