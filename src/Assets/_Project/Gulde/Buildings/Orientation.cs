using UnityEngine;

namespace Gulde
{
    public enum Orientation
    {
        Right,
        Up,
        Left,
        Down,
    }

    public static class OrientationExtensions
    {
        public static Quaternion ToQuaternion(this Orientation orientation)
        {
            var eulerZ = orientation switch
            {
                Orientation.Right => 0f,
                Orientation.Up => 270f,
                Orientation.Left => 180f,
                Orientation.Down => 90f,
                _ => 0f,
            };

            var quaternion = Quaternion.Euler(0f, 0f, eulerZ);
            return quaternion;
        }
    }
}