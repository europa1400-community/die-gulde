using Newtonsoft.Json;
using UnityEngine;

namespace Gulde.Client.Model.Common
{
    internal class ElementTransform
    {
        [JsonProperty("position")]
        public Vector3 Position { get; set; }

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; }

        public Matrix4x4 GetMatrix()
        {
            // Initialize an identity matrix
            var matrix = Matrix4x4.identity;

            // Apply translation
            matrix.m03 = -Position.x;
            matrix.m13 = Position.y;
            matrix.m23 = -Position.z;

            // Apply scaling
            matrix.m00 = Rotation.x;
            matrix.m11 = 1.0f;  // Set to 1.0 for pure scaling
            matrix.m22 = Rotation.z;

            // Apply rotation
            var cosTheta = Mathf.Cos(Rotation.y);
            var sinTheta = Mathf.Sin(Rotation.y);
            matrix.m00 = cosTheta;
            matrix.m02 = -sinTheta;
            matrix.m20 = sinTheta;
            matrix.m22 = cosTheta;

            // Apply shear
            matrix.m01 = 0;
            matrix.m10 = 0;
            
            return matrix;
        }
    }
}