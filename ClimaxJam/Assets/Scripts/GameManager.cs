using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    CheckpointManager checkpointManager;
    PlayerController playerController;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
        playerController = FindObjectOfType<PlayerController>();
    }

    public Vector3 GetLatestCheckpointPosition()
    {
        return checkpointManager.CurrentCheckpoint.transform.position;
    }

}