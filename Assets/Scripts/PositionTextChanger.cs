using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PositionTextChanger : MonoBehaviour
{
    //VARIABLES ---------------------------------------------------------------------------------------------------------

    public int position;
    public TextMeshProUGUI positionText;

    //CODE --------------------------------------------------------------------------------------------------------------

    //this function is called every frame
    void Update()
    {
        string stringPosition = position.ToString(); //stringPosition is the string version of position

        //add correct suffix depending on number
        //if 1 add st (1st)
        if (stringPosition == "1")
        {
            stringPosition += "st";
        }
        //if 2 add nd (2nd)
        else if (stringPosition == "2")
        {
            stringPosition += "nd";
        }
        //if 3 add rd (3rd)
        else if (stringPosition == "3")
        {
            stringPosition += "rd";
        }
        //otherwise add th (4th)
        else
        {
            stringPosition += "th";
        }

        //this accounts for up to 20 player places, if more are allowed add more conditions

        positionText.text = stringPosition; //change GUI text to value of stringPosition
    }
}


