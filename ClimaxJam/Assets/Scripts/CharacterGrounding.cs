using UnityEngine;

public class CharacterGrounding : MonoBehaviour
{
    [SerializeField]
    Transform[] positions;

    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private LayerMask layerMask;

    private Transform groundedObject;
    private Vector3? groundedObjectLastPosition;
    public bool IsGrounded { get; private set; }
    public Vector2 GroundedDirection { get; private set; }

    void FixedUpdate()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            CheckFootForGrounding(positions[i]);
            if (IsGrounded)
                break;
        }
        StickToMovingObjects();
    }

    void StickToMovingObjects()
    {
        if (groundedObject != null)
        {
            if (groundedObjectLastPosition.HasValue && groundedObjectLastPosition.Value != groundedObject.position)
            {
                Vector3 delta = groundedObject.position - groundedObjectLastPosition.Value;
                transform.position += delta;
            }
            groundedObjectLastPosition = groundedObject.position;
        }
        else
        {
            groundedObjectLastPosition = null;
        }
    }

    private void CheckFootForGrounding(Transform foot)
    {
        var raycastHit = Physics2D.Raycast(foot.position, foot.forward, maxDistance, layerMask);
        if (raycastHit.collider != null)
        {
            if (groundedObject != raycastHit.collider.transform)
            {
                groundedObjectLastPosition = raycastHit.collider.transform.position;
            }
            IsGrounded = true;
            GroundedDirection = foot.forward;
            groundedObject = raycastHit.collider.transform;
        }
        else
        {
            groundedObject = null;
            IsGrounded = false;
        }
    }
}
