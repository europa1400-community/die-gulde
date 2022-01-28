using System;
using Sirenix.OdinInspector;

namespace GuldeLib
{
    public class ObjectSavedEventArgs : EventArgs
    {
        public SerializedScriptableObject Obj { get; }

        public ObjectSavedEventArgs(SerializedScriptableObject obj) => Obj = obj;
    }
}