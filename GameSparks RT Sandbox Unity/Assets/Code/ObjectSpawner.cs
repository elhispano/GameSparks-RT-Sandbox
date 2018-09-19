using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab;
    
    public static ObjectSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public GameObject SpawnPlayerCharacter(Vector3 position)
    {
        GameObject newPlayer = Instantiate(m_playerPrefab,position,Quaternion.identity);

        return newPlayer;
    }
}
