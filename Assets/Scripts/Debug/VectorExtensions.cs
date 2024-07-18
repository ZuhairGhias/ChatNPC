using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector3 vec3)
    {
        return new Vector3(vec3.x, vec3.y);
    }
}
