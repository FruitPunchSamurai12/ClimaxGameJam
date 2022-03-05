using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float Vertical => Input.GetAxis("Vertical");
    public float Horizontal => Input.GetAxisRaw("Horizontal");

    public bool Fire => Input.GetButtonDown("Fire1");
    public bool Dash => Input.GetButtonDown("Fire2");
    public bool Jump => Input.GetButtonDown("Jump");

    public bool PausePressed => Input.GetKeyDown(KeyCode.Escape);

    public Vector2 MousePosition => Input.mousePosition;

}
