using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Companies
{
    /// <summary>
    /// Provides information for masters.
    /// </summary>
    public class MasterComponent : SerializedMonoBehaviour
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