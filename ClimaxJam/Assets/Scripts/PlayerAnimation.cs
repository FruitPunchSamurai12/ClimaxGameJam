using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    float delayJumpTime = 0.1f;
    [SerializeField]
    float delayThrowTime = 0.1f;
    Animator animator;
    [SerializeField] float stepMinPitch = 0.8f;
    [SerializeField] float stepMaxPitch = 1.2f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void DelayJump(Action callback)
    {
        animator.SetTrigger("Jump");
        StartCoroutine(DelayCallback(callback,delayJumpTime));    
    }
    public void DelayThrow(Action callback)
    {
        animator.SetTrigger("Throw");
        StartCoroutine(DelayCallback(callback, delayThrowTime));
    }

    IEnumerator DelayCallback(Action callback,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        callback();
    }

    public void PlayFootStep()
    {
        float pitch = UnityEngine.Random.Range(stepMinPitch, stepMaxPitch);
        AudioManager.Instance.PlaySoundEffectInSpecificSource("Step", 8, pitch);
    }

    public void PlayLand()
    {
        //AudioManager.Instance.PlaySoundEffect("Land");
    }

    public void PlayJump()
    {
        AudioManager.Instance.PlaySoundEffect("Jump");
    }
}

