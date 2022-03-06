using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    CheckpointManager checkpointManager;
    PlayerController playerController;

    public event Action onChainLinkPick;
    public event Action onVictory;

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
        AudioManager.Instance.StartGameMusic();
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    public Vector3 GetLatestCheckpointPosition()
    {
        return checkpointManager.CurrentCheckpoint.transform.position;
    }

    public void PickedChainLink()
    {
        AudioManager.Instance.PlaySoundEffect("Link");
        onChainLinkPick?.Invoke();
    }

    public void Victory()
    {
        playerController.Victory();
        onVictory?.Invoke();
    }
}
