using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Players.Professions
{
    public class ProfessionComponent : SerializedMonoBehaviour
    {

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