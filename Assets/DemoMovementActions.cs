//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/SampleProjects/InputSystem/DemoMovementActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @DemoMovementActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @DemoMovementActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DemoMovementActions"",
    ""maps"": [
        {
            ""name"": ""DemoMap"",
            ""id"": ""2881e1a9-0837-4dab-b863-c71d3639d68f"",
            ""actions"": [
                {
                    ""name"": ""PlayerMovement"",
                    ""type"": ""Value"",
                    ""id"": ""df274a3a-e3aa-447f-9d94-3d658edbcc55"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PlayerJump"",
                    ""type"": ""Button"",
                    ""id"": ""74d5c7d6-43a5-4833-a816-73b5b647b6b7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""5155be56-c2b8-414b-9001-2ece243674de"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""15529a73-aff0-4686-8174-602703f72074"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8de24fa2-2e48-4a21-9d4e-c6922027e413"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f6eb713a-288a-41c3-8ebc-6af8d56edd2e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3dd21ec1-ee6d-4942-b925-6506d6faa622"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ArrowKeys"",
                    ""id"": ""81f9f6c7-83a1-4ca4-9be5-dbd081093698"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cfe90539-c3de-4af8-bdea-dacbc5744ff1"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8bb25708-ecb6-4b20-830b-6afe5d98cd33"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ea78b887-e740-4f32-a60f-4cead71ced4c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""af79769f-d9c0-4dee-b11d-921b1e783fe9"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3ebf6174-cf55-416a-b82a-487d7076b92f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // DemoMap
        m_DemoMap = asset.FindActionMap("DemoMap", throwIfNotFound: true);
        m_DemoMap_PlayerMovement = m_DemoMap.FindAction("PlayerMovement", throwIfNotFound: true);
        m_DemoMap_PlayerJump = m_DemoMap.FindAction("PlayerJump", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // DemoMap
    private readonly InputActionMap m_DemoMap;
    private List<IDemoMapActions> m_DemoMapActionsCallbackInterfaces = new List<IDemoMapActions>();
    private readonly InputAction m_DemoMap_PlayerMovement;
    private readonly InputAction m_DemoMap_PlayerJump;
    public struct DemoMapActions
    {
        private @DemoMovementActions m_Wrapper;
        public DemoMapActions(@DemoMovementActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @PlayerMovement => m_Wrapper.m_DemoMap_PlayerMovement;
        public InputAction @PlayerJump => m_Wrapper.m_DemoMap_PlayerJump;
        public InputActionMap Get() { return m_Wrapper.m_DemoMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DemoMapActions set) { return set.Get(); }
        public void AddCallbacks(IDemoMapActions instance)
        {
            if (instance == null || m_Wrapper.m_DemoMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DemoMapActionsCallbackInterfaces.Add(instance);
            @PlayerMovement.started += instance.OnPlayerMovement;
            @PlayerMovement.performed += instance.OnPlayerMovement;
            @PlayerMovement.canceled += instance.OnPlayerMovement;
            @PlayerJump.started += instance.OnPlayerJump;
            @PlayerJump.performed += instance.OnPlayerJump;
            @PlayerJump.canceled += instance.OnPlayerJump;
        }

        private void UnregisterCallbacks(IDemoMapActions instance)
        {
            @PlayerMovement.started -= instance.OnPlayerMovement;
            @PlayerMovement.performed -= instance.OnPlayerMovement;
            @PlayerMovement.canceled -= instance.OnPlayerMovement;
            @PlayerJump.started -= instance.OnPlayerJump;
            @PlayerJump.performed -= instance.OnPlayerJump;
            @PlayerJump.canceled -= instance.OnPlayerJump;
        }

        public void RemoveCallbacks(IDemoMapActions instance)
        {
            if (m_Wrapper.m_DemoMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDemoMapActions instance)
        {
            foreach (var item in m_Wrapper.m_DemoMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DemoMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DemoMapActions @DemoMap => new DemoMapActions(this);
    public interface IDemoMapActions
    {
        void OnPlayerMovement(InputAction.CallbackContext context);
        void OnPlayerJump(InputAction.CallbackContext context);
    }
}
