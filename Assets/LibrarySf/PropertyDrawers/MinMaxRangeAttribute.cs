using System;

namespace LibrarySf
{
    /// <summary>
    /// Allows custom range bounds. ie [MinMaxRange(-100,100)]
    /// Works with Editor/RangedFloatDrawer.cs
    /// </summary>
    public class MinMaxRangeAttribute : Attribute // UnityEngine.PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

    [Serializable]
    public struct MinMaxRange
    {
        public float MinValue;
        public float MaxValue;
    }
}
