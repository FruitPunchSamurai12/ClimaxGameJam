using UnityEngine;

public static class Collision2DExtensions
{
    public static bool WasHitByPlayer(this Collision2D col)
    {
        return col.collider.GetComponent<PlayerController>() != null;
    }

    public static bool WasBottom(this Collision2D col)
    {
        return col.contacts[0].normal.y > 0.5f;
    }

    public static bool WasTop(this Collision2D col)
    {
        return col.contacts[0].normal.y < -0.5f;
    }
    public static bool WasSide(this Collision2D col)
    {
        return col.contacts[0].normal.x < -0.5f || col.contacts[0].normal.x > 0.5f;
    }

}

