using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    public int Id { get; private set; }
    
    public bool IsPlayer { get; private set; }

    public void Initialize(int id, bool isPLayer)
    {
        Id = id;
        IsPlayer = isPLayer;
    }
}
