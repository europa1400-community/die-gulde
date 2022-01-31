using System;
using GuldeLib.TypeObjects;
using Sirenix.OdinInspector;

namespace GuldeLib.Maps.Buildings
{
    public class BuildingComponent : SerializedMonoBehaviour
    {

        [ShowInInspector]
        [ReadOnly]
        public Building Building { get; set; }

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public class InitializedEventArgs : EventArgs
        {
        }
    }
}