using System;
using System.Collections;
using UnityEngine;

public enum PlayerState
{
    ground,
    air,
    swing,
    wall,
    dash
}


[RequireComponent(typeof(CharacterGrounding))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float groundMoveSpeed = 5f;
    [SerializeField] float airMoveSpeed = 2f;
    [SerializeField] float jumpPower = 1000f;
    [SerializeField] float swingMoveSpeed = 1f;
    [SerializeField] float defaultHorizontalMaxSpeed = 7f;
    [SerializeField] float dashHorizontalMaxSpeed = 10f;
    [SerializeField] Kunai kunai;
    [SerializeField] Transform hand;
    [SerializeField] float wallUpForce = 20f;
    [SerializeField] float dashForce = 20f;
    WallSensor wallSensor;
    Rigidbody2D rb2d;
    CharacterGrounding characterGrounding;
    PlayerInput playerInput;
    PlayerAnimation playerAnimation;
    Transform spriteTransform;

    [SerializeField] float wallGrabDelay = 0.2f;
    [SerializeField] float dashCooldown = 0.5f;
    float dashTimer = 0;
    bool allowInput = true;
    float horizontalMaxSpeed;
    PlayerState state = PlayerState.ground;
    float horizontal;
    bool directionIsLeft = false;
    bool jump;
    bool fire;
    bool dash;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        characterGrounding = GetComponent<CharacterGrounding>();
        playerInput = GetComponent<PlayerInput>();
        wallSensor = GetComponent<WallSensor>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
        spriteTransform = playerAnimation.transform;
        kunai.onHook += () => { ChangeState(PlayerState.swing); };
        horizontalMaxSpeed = defaultHorizontalMaxSpeed;
    }

    private void Update()
    {
        if (allowInput)
        {
            if (!jump)
                jump = playerInput.Jump;
            if (!fire)
                fire = playerInput.Fire;
            if (!dash)
                dash = playerInput.Dash;
            horizontal = playerInput.Horizontal;
            playerAnimation.SetSpeed(Mathf.Abs(horizontal));
            if (horizontal > 0)
            {
                directionIsLeft = false;
            }
            else if (horizontal < 0)
            {
                directionIsLeft = true;
            }
            FlipSprite();
        }
        switch (state)
        {
            case PlayerState.ground:
                if (!characterGrounding.IsGrounded)
                {
                    ChangeState(PlayerState.air);
                    playerAnimation.SetTrigger("Fall");
                }
                break;
            case PlayerState.air:
                if (characterGrounding.IsGrounded)
                {
                    ChangeState(PlayerState.ground);
                    playerAnimation.SetTrigger("Land");
                }
                else if (wallSensor.IsLatched)
                {
                    ChangeState(PlayerState.wall);
                    playerAnimation.SetTrigger("WallGrab");
                }
                else if(rb2d.velocity.y <0)
                {
                    playerAnimation.SetTrigger("Fall");
                }
                break;
            case PlayerState.swing:
                if (characterGrounding.IsGrounded)
                {
                    ChangeState(PlayerState.ground);
                    playerAnimation.SetTrigger("Land");
                }
                break;
            case PlayerState.wall:
                if (!wallSensor.IsLatched)
                {
                    ChangeState(PlayerState.air);
                    playerAnimation.SetTrigger("Fall");
                }
                if (characterGrounding.IsGrounded)
                {
                    ChangeState(PlayerState.ground);
                    playerAnimation.SetTrigger("Land");
                }
                break;
            case PlayerState.dash:
                if (wallSensor.IsLatched)
                {
                    ChangeState(PlayerState.wall);
                    playerAnimation.SetTrigger("WallGrab");
                    break;
                }
                dashTimer += Time.deltaTime;
                if(dashTimer>dashCooldown)
                {
                    horizontalMaxSpeed = defaultHorizontalMaxSpeed;
                    if (characterGrounding.IsGrounded)
                    {
                        ChangeState(PlayerState.ground);
                        playerAnimation.SetTrigger("Reset");
                    }
                    else
                    {
                        ChangeState(PlayerState.air);
                        playerAnimation.SetTrigger("Fall");
                    }
                }
                break;
        }
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.ground:
                rb2d.velocity += new Vector2(horizontal * groundMoveSpeed, 0);
                if (jump)
                {
                    StopInput();
                    playerAnimation.DelayJump(() =>
                    {
                        rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                        ChangeState(PlayerState.air);
                        allowInput = true;
                    });
                }
                if (dash)
                {
                    PerformDash();
                }
                break;
            case PlayerState.air:
                rb2d.velocity += new Vector2(horizontal * airMoveSpeed, 0);
                if (fire)
                {
                    StopInput();
                    playerAnimation.DelayThrow(() =>
                    {
                        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(kunai.transform.position);
                        Vector2 direction = ((Vector2)playerInput.MousePosition - positionOnScreen).normalized;
                        kunai.Throw(direction);
                        allowInput = true;
                    });
                }
                if(dash)
                {
                    PerformDash();
                }
                break;
            case PlayerState.swing:
                rb2d.AddForce(new Vector2(horizontal * swingMoveSpeed, 0));
                if (jump)
                {
                    kunai.Unhook();
                    rb2d.AddForce((rb2d.velocity + Vector2.up).normalized * jumpPower * 2f / 3f, ForceMode2D.Impulse);
                    ChangeState(PlayerState.air);
                    playerAnimation.SetTrigger("Jump");
                }
                if(fire)
                {
                    kunai.Unhook();
                    ChangeState(PlayerState.air);
                    playerAnimation.SetTrigger("Fall");
                }
                break;
            case PlayerState.wall:
                if(rb2d.velocity.y <0)
                    rb2d.velocity += Vector2.up * wallUpForce;
                if(jump)
                {
                    StartCoroutine(wallSensor.DisableWallSensor());
                    StartCoroutine(StopInputOnWallGrab());
                    rb2d.velocity = Vector2.zero;
                    rb2d.AddForce((wallSensor.WallDirection + Vector2.up).normalized * jumpPower, ForceMode2D.Impulse);
                    directionIsLeft = wallSensor.WallDirection.x < 0;
                    FlipSprite();
                    ChangeState(PlayerState.air);
                    playerAnimation.SetTrigger("WallJump");
                }
                break;
        }
        rb2d.velocity = Vector2.ClampMagnitude(new Vector2(rb2d.velocity.x, 0), horizontalMaxSpeed) + new Vector2(0, rb2d.velocity.y);
        fire = false;
        jump = false;
        dash = false;
    }

    void ChangeState(PlayerState newState)
    {
        state = newState;
        switch (state)
        {
            case PlayerState.ground:
                break;
            case PlayerState.air:
                break;
            case PlayerState.swing:
                playerAnimation.SetTrigger("Swing");
                break;
            case PlayerState.wall:                
                Debug.Log("latched");
                break;
            case PlayerState.dash:
                dashTimer = 0;
                horizontalMaxSpeed = dashHorizontalMaxSpeed;
                break;
        }
    }

    void PerformDash()
    {
        Vector2 dir = directionIsLeft ? -transform.right : transform.right;
        rb2d.AddForce(dir * dashForce, ForceMode2D.Impulse);
        ChangeState(PlayerState.dash);
        playerAnimation.SetTrigger("Dash");
    }

    void FlipSprite()
    {
        if(directionIsLeft)
        {
            spriteTransform.localScale = new Vector3(-Mathf.Abs(spriteTransform.localScale.x), spriteTransform.localScale.y, spriteTransform.localScale.z);
        }
        else
        {
            spriteTransform.localScale = new Vector3(Mathf.Abs(spriteTransform.localScale.x), spriteTransform.localScale.y, spriteTransform.localScale.z);
        }
    }

    IEnumerator StopInputOnWallGrab()
    {
        StopInput();
        yield return new WaitForSeconds(wallGrabDelay);
        allowInput = true;
    }

    public void ResetPlayer()
    {
        
    }

    private void StopInput()
    {
        fire = false;
        jump = false;
        dash = false;
        horizontal = 0;
        allowInput = false;
    }

}
