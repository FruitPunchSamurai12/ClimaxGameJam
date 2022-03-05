using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChainState
{
    held,
    fly,
    swing,
    pull
}

[RequireComponent(typeof(LineRenderer))]
public class Chain : MonoBehaviour
{
    [SerializeField] float chainLength = 10f;
    [SerializeField] float chainLengthHeld = .5f;
    [SerializeField] float pullPower = .5f;
    [SerializeField] float minumumSwingLength = 3f;
    LineRenderer lineRend;
    SpringJoint2D distanceJoint;
    CharacterGrounding characterGrounding;
    Kunai kunai;
    float distanceFromKunai;
    float currentLength;
    bool pullingKunai;
    public ChainState state { get; private set; } = ChainState.held;

    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 2;
        lineRend.enabled = false;
        distanceJoint = GetComponentInParent<SpringJoint2D>();        
        characterGrounding = GetComponentInParent<CharacterGrounding>();
    }

    public void SetKunai(Kunai kunai)
    {
        this.kunai = kunai;
        lineRend.enabled = true;
        distanceJoint.enabled = true;
        distanceJoint.connectedBody = kunai.GetComponent<Rigidbody2D>();
        ChangeState(ChainState.held);
    }

    void UnSetKunai()
    {
        kunai = null;
    }

    void Update()
    {
        lineRend.SetPosition(0, transform.position);
        lineRend.SetPosition(1, kunai.transform.position);
        distanceFromKunai = Vector2.Distance(kunai.transform.position, transform.position);
        switch (state)
        {
            case ChainState.held:               
                break;
            case ChainState.fly:
                if(distanceFromKunai>chainLength || characterGrounding.IsGrounded)
                {
                    ChangeState(ChainState.pull);
                }
                break;
            case ChainState.swing:
                if (characterGrounding.IsGrounded)
                {
                    ChangeState(ChainState.pull);
                }
                break;
            case ChainState.pull:
                kunai.PullKunai(transform.position, pullPower);
                if (distanceFromKunai < chainLengthHeld)
                {
                    ChangeState(ChainState.held);
                }
                break;
        }
    }

    public void OnThrow()
    {
        ChangeState(ChainState.fly);
    }

    public void OnHook()
    {
        ChangeState(ChainState.swing);
    }

    public void OnUnhook()
    {
        ChangeState(ChainState.pull);
    }

    void ChangeState(ChainState newState)
    {
        state = newState;
        switch (state)
        {
            case ChainState.held:
                distanceJoint.enabled = true;
                distanceJoint.distance = chainLengthHeld;
                break;
            case ChainState.fly:
                distanceJoint.enabled = false;
                break;
            case ChainState.swing:
                distanceJoint.distance = Mathf.Max(distanceFromKunai, minumumSwingLength);
                distanceJoint.enabled = true;
                break;
            case ChainState.pull:
                distanceJoint.enabled = false;
                kunai.StartPullKunai();
                break;
        }
    }
}