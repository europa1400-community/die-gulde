using GuldeLib.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeClient
{
    public class EntityHandlerComponent : SerializedMonoBehaviour
    {
        public void OnInitialized(object sender, EntityComponentInitializedEventArgs e)
        {
            transform.position = e.Position;
        }

        public void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            transform.position = e.Position;
        }
    }
}