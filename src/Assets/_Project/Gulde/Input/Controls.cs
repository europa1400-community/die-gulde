// GENERATED AUTOMATICALLY FROM 'Assets/_Project/Input/controls_default.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Gulde.Input
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""controls_default"",
    ""maps"": [
        {
            ""name"": ""DefaultMap"",
            ""id"": ""d8a2bc6e-97d1-448e-9bd9-a249542c8393"",
            ""actions"": [
                {
                    ""name"": ""PauseAction"",
                    ""type"": ""Button"",
                    ""id"": ""f1cc5cbf-f03f-4956-90a3-f803f8648bd9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MorningAction"",
                    ""type"": ""Button"",
                    ""id"": ""d902e4be-7ccd-4b81-807d-d4be7bfcb66e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EveningAction"",
                    ""type"": ""Button"",
                    ""id"": ""0746ee83-61ac-47be-b898-4666d53cb6ec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""efc5267f-0140-4908-9193-b4b10440bd53"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f67ae969-1977-445d-b5cd-7e8acb95737c"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MorningAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac44788f-7cda-4a20-89ca-896eaab77e09"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EveningAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // DefaultMap
            m_DefaultMap = asset.FindActionMap("DefaultMap", throwIfNotFound: true);
            m_DefaultMap_PauseAction = m_DefaultMap.FindAction("PauseAction", throwIfNotFound: true);
            m_DefaultMap_MorningAction = m_DefaultMap.FindAction("MorningAction", throwIfNotFound: true);
            m_DefaultMap_EveningAction = m_DefaultMap.FindAction("EveningAction", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // DefaultMap
        private readonly InputActionMap m_DefaultMap;
        private IDefaultMapActions m_DefaultMapActionsCallbackInterface;
        private readonly InputAction m_DefaultMap_PauseAction;
        private readonly InputAction m_DefaultMap_MorningAction;
        private readonly InputAction m_DefaultMap_EveningAction;
        public struct DefaultMapActions
        {
            private @Controls m_Wrapper;
            public DefaultMapActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @PauseAction => m_Wrapper.m_DefaultMap_PauseAction;
            public InputAction @MorningAction => m_Wrapper.m_DefaultMap_MorningAction;
            public InputAction @EveningAction => m_Wrapper.m_DefaultMap_EveningAction;
            public InputActionMap Get() { return m_Wrapper.m_DefaultMap; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DefaultMapActions set) { return set.Get(); }
            public void SetCallbacks(IDefaultMapActions instance)
            {
                if (m_Wrapper.m_DefaultMapActionsCallbackInterface != null)
                {
                    @PauseAction.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnPauseAction;
                    @PauseAction.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnPauseAction;
                    @PauseAction.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnPauseAction;
                    @MorningAction.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnMorningAction;
                    @MorningAction.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnMorningAction;
                    @MorningAction.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnMorningAction;
                    @EveningAction.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnEveningAction;
                    @EveningAction.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnEveningAction;
                    @EveningAction.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnEveningAction;
                }
                m_Wrapper.m_DefaultMapActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @PauseAction.started += instance.OnPauseAction;
                    @PauseAction.performed += instance.OnPauseAction;
                    @PauseAction.canceled += instance.OnPauseAction;
                    @MorningAction.started += instance.OnMorningAction;
                    @MorningAction.performed += instance.OnMorningAction;
                    @MorningAction.canceled += instance.OnMorningAction;
                    @EveningAction.started += instance.OnEveningAction;
                    @EveningAction.performed += instance.OnEveningAction;
                    @EveningAction.canceled += instance.OnEveningAction;
                }
            }
        }
        public DefaultMapActions @DefaultMap => new DefaultMapActions(this);
        public interface IDefaultMapActions
        {
            void OnPauseAction(InputAction.CallbackContext context);
            void OnMorningAction(InputAction.CallbackContext context);
            void OnEveningAction(InputAction.CallbackContext context);
        }
    }
}
