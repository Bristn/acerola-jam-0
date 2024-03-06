using System;
using UnityEngine;

public static class UtilityExtensions
{
    // https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    public static float Remap(this float value, float prevFrom, float prevTo, float newFrom, float newTo)
    {
        return (value - prevFrom) / (prevTo - prevFrom) * (newTo - newFrom) + newFrom;
    }
}