using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite onSprite;
    SpriteRenderer spriteRenderer;
    bool turnedOn;

    public event Action<Checkpoint> onCheckpointTrigger;

    BoxCollider2D _col;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = offSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            onCheckpointTrigger?.Invoke(this);
            spriteRenderer.sprite = onSprite;
            if(!turnedOn)
            {
                turnedOn = true;
                AudioManager.Instance.PlaySoundEffect("Checkpoint");
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireCube(transform.position, _col.bounds.size);
    }
#endif

}
