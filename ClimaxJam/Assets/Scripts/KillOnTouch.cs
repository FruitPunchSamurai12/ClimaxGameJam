using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillOnTouch : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var playerController = collision.collider.GetComponent<PlayerController>();
        if(playerController != null)
        {
            playerController.ResetPlayer();
        }
    }
}
