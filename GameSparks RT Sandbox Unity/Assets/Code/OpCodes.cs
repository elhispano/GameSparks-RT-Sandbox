﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpCodes
{
    None = 0,
    PlayerTransform = 2,
    PlayerShoot = 3,
    UpdatePlayerShoots = 4,
    RegisterShootHit = 5,
    AllPlayersConnected = 100,
    TimeStamp = 101,
    Countdown = 102,
}
