using System;
using UnityEngine;

[RequireComponent(typeof(CharacterGrounding))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float groundMoveSpeed = 5f;
    [SerializeField] float airMoveSpeed = 2f;
    [SerializeField] float jumpPower = 1000f;
    [SerializeField] float swingMoveSpeed = 1f;
    [SerializeField] float maxHorizontalSpeed = 7f;
    [SerializeField] Kunai kunai;
    [SerializeField] Transform hand;
    Rigidbody2D rb2d;
    CharacterGrounding characterGrounding;
    PlayerInput playerInput;


    bool swinging = false;
    float horizontal;
    bool jump;
    bool fire;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        characterGrounding = GetComponent<CharacterGrounding>();
        playerInput = GetComponent<PlayerInput>();
        kunai.onHook += () => { swinging = true;};
    }


    private void Update()
    {
        if(!jump)
            jump = playerInput.Jump;
        if(!fire)
            fire = playerInput.Fire;
        horizontal = playerInput.Horizontal;
    }

    void FixedUpdate()
    {
        if (swinging)
        {
            rb2d.AddForce(new Vector2(horizontal * swingMoveSpeed, 0));
        }
        else if(!characterGrounding.IsGrounded)
        {
            rb2d.velocity += new Vector2(horizontal * airMoveSpeed, 0);
        }
        else
        {
            rb2d.velocity += new Vector2(horizontal * groundMoveSpeed, 0);
        }
        rb2d.velocity = Vector2.ClampMagnitude(new Vector2(rb2d.velocity.x, 0), maxHorizontalSpeed) + new Vector2(0, rb2d.velocity.y);

        if (jump)
        {
            jump = false;
            if (characterGrounding.IsGrounded)
                rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            else if (swinging)
            {
                Debug.Log("wtf1");
                swinging = false;
                kunai.Unhook();
                rb2d.AddForce((rb2d.velocity + Vector2.up).normalized * jumpPower*2f/3f, ForceMode2D.Impulse);
            }
        }

        if (fire)
        {
            fire = false;
            if(swinging)
            {
                Debug.Log("wtf2");
                swinging = false;
                kunai.Unhook();
            }
            else if(!characterGrounding.IsGrounded)
            {
                Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(kunai.transform.position);
                Vector2 direction = ((Vector2)playerInput.MousePosition - positionOnScreen).normalized;
                kunai.Throw(direction);
            }
        }
    }

}
