using System.Collections.Generic;
using System.Linq;
using Gulde.Client.Model.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Gulde.Client.Model.Scenes
{
    internal abstract class TransformElement
    {
        [JsonProperty("transform")]
        public ElementTransform Transform { get; set; }

        [JsonProperty("transforms")]
        public List<ElementTransform> Transforms { get; set; }
        
        public ElementTransform GetSecondTransform() => Transforms.FirstOrDefault(transform =>
            transform.Position != Vector3.zero || transform.Rotation != Vector3.zero);

        public Matrix4x4 GetCombinedMatrix()
        {
            var firstMatrix = Transform.GetMatrix();
            var secondTransform = GetSecondTransform();
            
            if (secondTransform is null)
            {
                return firstMatrix;
            }
            
            var secondMatrix = secondTransform.GetMatrix();
            return secondMatrix * firstMatrix;
        }
    }
}