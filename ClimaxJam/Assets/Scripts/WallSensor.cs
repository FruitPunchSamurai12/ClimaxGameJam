using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour
{
    [SerializeField]
    Transform[] positions;

    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    float disableTime = 0.5f;
    private Transform wallObject;
    private Vector3? wallLastPosition;
    public bool IsLatched { get; private set; }
    public Vector2 WallDirection { get; private set; }

    bool disabled = false;
    void FixedUpdate()
    {
        if (disabled)
        {
            IsLatched = false;
            return;
        }
        bool latched = true;
        for (int i = 0; i < positions.Length; i++)
        {
            latched &= CheckSideForWall(positions[i],true);           
        }
        if(latched)
        {
            IsLatched = latched;
            return;
        }
        latched = true;
        for (int i = 0; i < positions.Length; i++)
        {
            latched &= CheckSideForWall(positions[i], false);
        }
        IsLatched = latched;
    }

    private bool CheckSideForWall(Transform point,bool leftSide)
    {
        var raycastHit = Physics2D.Raycast(point.position,leftSide?point.forward:-point.forward, maxDistance, layerMask);
        if (raycastHit.collider != null)
        {
            if (wallObject != raycastHit.collider.transform)
            {
                wallLastPosition = raycastHit.collider.transform.position;
            }
            WallDirection = point.forward;
            wallObject = raycastHit.collider.transform;
            return true;
        }
        else
        {
            wallObject = null;
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < positions.Length; i++)
        {
            Debug.DrawRay(positions[i].position, positions[i].forward);
        }
    }

    public IEnumerator DisableWallSensor()
    {
        disabled = true;
        Debug.Log("cvalled");
        yield return new WaitForSeconds(disableTime);
        disabled = false;
    }
}
