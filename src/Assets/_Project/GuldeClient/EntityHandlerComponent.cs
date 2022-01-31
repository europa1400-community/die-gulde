using GuldeLib.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeClient
{
    public class EntityHandlerComponent : SerializedMonoBehaviour
    {
        public void OnInitialized(object sender, EntityComponent.InitializedEventArgs e)
        {
            transform.position = e.Position;
        }

        public void OnPositionChanged(object sender, EntityComponent.PositionChangedEventArgs e)
        {
            transform.position = e.Position;
        }
    }
}