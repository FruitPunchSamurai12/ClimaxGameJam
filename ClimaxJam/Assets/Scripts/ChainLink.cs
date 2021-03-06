using UnityEngine;

public class ChainLink : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.PickedChainLink();
            AudioManager.Instance.PlaySoundEffect("Link");
            Destroy(gameObject);
        }
    }
}
