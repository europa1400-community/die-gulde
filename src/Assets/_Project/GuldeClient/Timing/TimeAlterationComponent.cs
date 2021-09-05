using GuldeClient.Input;
using GuldeLib;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

namespace GuldeClient.Timing
{
    public class TimeAlterationComponent : SerializedMonoBehaviour
    {
        Controls Controls { get; set; }

        void Awake()
        {
            Controls = new Controls();

            Controls.DefaultMap.PauseAction.performed -= OnPauseActionPerformed;
            Controls.DefaultMap.PauseAction.performed += OnPauseActionPerformed;
            Controls.DefaultMap.Enable();
        }

        void OnPauseActionPerformed(InputAction.CallbackContext ctx)
        {
            Locator.Time.ToggleTime();
        }
    }
}