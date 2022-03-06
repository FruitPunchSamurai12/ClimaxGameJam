using UnityEngine;

public class Artifact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Victory();          
        }
    }

}
