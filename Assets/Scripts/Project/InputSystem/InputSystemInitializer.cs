using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.InputSystem{

    [RequireComponent(typeof(PlayerInput))]
    public class InputSystemInitializer : MonoBehaviour
    {
        private PlayerInput playerInput;
        [SerializeField] InputActionAsset inputActionAsset;
        [SerializeField] InputActionReference reference;
        [SerializeField] InputSystemEventStorage inputSystemEventStorage;
        

        void Awake(){
            playerInput = GetComponent<PlayerInput>();
            playerInput.actions = inputActionAsset;
            playerInput.currentActionMap.Enable();

            inputSystemEventStorage.Initialize(ref playerInput);
        }

        // public void RegisterCallbackAt(int index, UnityAction<InputAction.CallbackContext> callback){
        //     if (!m_callbacks.TryGetValue(index, out InputActionCallbackWrapper wrapper)){
        //         wrapper = new InputActionCallbackWrapper(playerInput.actionEvents[index]);
        //         m_callbacks.Add(index, wrapper);
        //     }
        //     wrapper.AddListener(callback);
        // }

        public void RegisterCallbackAt(int index, Action<InputAction.CallbackContext> callback) => inputSystemEventStorage.RegisterCallbackAt(index, callback);

        public void UnregisterCallbackAt(int index, Action<InputAction.CallbackContext> callback) => inputSystemEventStorage.UnregisterCallbackAt(index, callback);

        // public void UnregisterCallbackAt(int index, UnityAction<InputAction.CallbackContext> callback){
        //     if (m_callbacks.TryGetValue(index, out InputActionCallbackWrapper wrapper)){
        //         wrapper.RemoveListener(callback);
        //     }
        // }

    }
}
