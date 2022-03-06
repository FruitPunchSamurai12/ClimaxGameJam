using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Kunai : MonoBehaviour
{
    Rigidbody2D rb2d;
    [SerializeField] Chain chain;
    [SerializeField] float rotationSpeed = 300f;
    [SerializeField] float throwPower = 20f;
    BoxCollider2D col;

    public bool Flying { get; private set; }

    public event Action onHook;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        chain.SetKunai(this);       
    }

    public void Throw(Vector2 direction)
    {
        if (chain.state != ChainState.held)
            return;
        AudioManager.Instance.PlaySoundEffect("Throw");
        rb2d.velocity = direction * throwPower;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rb2d.velocity.normalized);
        rb2d.constraints = RigidbodyConstraints2D.None;
        Flying = true;
        col.enabled = true;
        chain.OnThrow();
    }

    public void StartPullKunai()
    {
        rb2d.constraints = RigidbodyConstraints2D.None;
        col.enabled = false;
    }

    public void PullKunai(Vector2 pullPosition, float pullForce)
    {
        Vector2 dir = (pullPosition - (Vector2)transform.position).normalized;
        var velocity = dir * pullForce;
        rb2d.velocity = velocity;
    }

    private void FixedUpdate()
    {
        if (Flying)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rb2d.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Hookable") && collision.WasTop())
        {
            rb2d.velocity = Vector2.zero;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            Flying = false;
            col.enabled = false;
            onHook?.Invoke();
            chain.OnHook();
            AudioManager.Instance.PlaySoundEffect("Hook");
        }
        else
        {
            Unhook();
        }
    }

    public void Unhook()
    {
        chain.OnUnhook();
    }
}
