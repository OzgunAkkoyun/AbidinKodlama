using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathTools
{
    public static Vector3 ToVector3XZ(this Vector2Int value)
    {
        return new Vector3(value.x, 0f, value.y);
    }

    public static Vector3 ToVector3(this Vector2 value)
    {
        return new Vector3(value.x, value.y, 0f);
    }

    public static Vector3 Vector3toXZ(this Vector3 value)
    {
        return new Vector3(value.x, 0f, value.z);
    }

    public static Vector2 Vector3ToVector2(this Vector3 value)
    {
        return new Vector2(value.x, value.y);
    }

    public static Vector3 Vector2ToVector3(this Vector2 value)
    {
        return new Vector3(value.x, 0, value.y);
    }

    public static string ListPrint<T>(this List<T> list)
    {
        var str = "";

        for (int i = 0; i < list.Count; i++)
        {
            str += list[i].ToString() + " -- ";
        }

        return str.Remove(str.Length - 4, 4);
    }

    public static string ArrayPrint(this Array list)
    {
        var str = "";

        foreach (var l in list)
        {
            str += l.ToString() + " -- ";
        }

        return str.Remove(str.Length - 4, 4);
    }

    public static bool Equal(Vector2 _v1, Vector2 _v2, Vector2 _e)
    {
        return System.Math.Abs(_v1.x - _v2.x) <= _e.x &&
               System.Math.Abs(_v1.y - _v2.y) <= _e.y;
    }
}