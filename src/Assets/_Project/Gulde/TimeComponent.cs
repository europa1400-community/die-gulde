using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gulde
{
    [ExecuteAlways]
    public class TimeComponent : SerializedMonoBehaviour
    {
        public event EventHandler Morning;
        public event EventHandler Evening;

        bool isBlocked;

        void Awake()
        {
            Locator.TimeComponent = this;
        }

        void Update()
        {
            if (Keyboard.current.digit1Key.isPressed && !isBlocked)
            {
                isBlocked = true;
                Morning?.Invoke(this, EventArgs.Empty);
            }

            if (Keyboard.current.digit2Key.isPressed && !isBlocked)
            {
                isBlocked = true;
                Evening?.Invoke(this, EventArgs.Empty);
            }

            if (Keyboard.current.bKey.isPressed && isBlocked) isBlocked = false;
        }

        #region OdinSerialize

        void OnValidate()
        {
            Locator.TimeComponent = this;
        }

        #endregion
    }
}