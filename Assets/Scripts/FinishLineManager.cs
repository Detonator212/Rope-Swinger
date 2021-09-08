using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FinishLineManager : NetworkBehaviour
{
    //these variables store the number of finished and dead players for all players to see
    [SyncVar]
    public int numFinishedPlayers = 0;
    [SyncVar]
    public int numDeadPlayers = 0;
}

