using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Names
{
    public class NamingComponent : SerializedMonoBehaviour
    {

        [ShowInInspector]
        public string FriendlyName { get; set; }

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