using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
    //Frequency at which the item will move up and down
    public float verticalBobFrequency = 1f;
    //Distance the item will move up and down
    public float bobbingAmount = 1f;
    public float rotatingSpeed = 360f;
    Vector3 m_StartPosition;


    // Start is called before the first frame update
    void Start()
    {
        // Remember start position for animation
        m_StartPosition = transform.position;
    }


    private void Update()
    {
        // Handle bobbing
        float bobbingAnimationPhase = ((Mathf.Sin(Time.time * verticalBobFrequency) * 0.5f) + 0.5f) * bobbingAmount;
        transform.position = m_StartPosition + Vector3.up * bobbingAnimationPhase;

        // Handle rotating
        transform.Rotate(Vector3.forward, rotatingSpeed * Time.deltaTime, Space.Self);
    }
}
