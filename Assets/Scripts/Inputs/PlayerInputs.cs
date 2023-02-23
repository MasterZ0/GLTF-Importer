using System;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace GLTFImporter.Inputs
{
    public class PlayerInputs : BaseInput
    {
        public event Action<Vector2> OnMoveCamera;
        public event Action OnJumpReleased;
        public event Action OnJumpPressed;

        public Vector2 Move => controls.Player.Move.ReadValue<Vector2>();
        public Vector2 Look => controls.Player.Look.ReadValue<Vector2>();

        public bool IsMovePressed => Move != Vector2.zero;
        public bool IsJumpPressed { get; private set; }
        public bool IsSprintPressed { get; private set; }

        public PlayerInputs(bool enable = true) : base(enable)
        {
            controls.Player.Jump.started += OnJumpDown;
            controls.Player.Jump.canceled += OnJumpUp;
            controls.Player.Sprint.started += OnSprintDown;
            controls.Player.Sprint.canceled += OnSprintUp;

            controls.Player.Look.started += OnLook;
        }

        private void OnLook(CallbackContext ctx) => OnMoveCamera?.Invoke(ctx.ReadValue<Vector2>());

        private void OnSprintUp(CallbackContext obj)
        {
            IsSprintPressed = false;
        }

        private void OnSprintDown(CallbackContext obj)
        {
            IsSprintPressed = true;
        }

        private void OnJumpDown(CallbackContext _)
        {
            IsJumpPressed = true;
            OnJumpPressed?.Invoke();
        }

        private void OnJumpUp(CallbackContext _)
        {
            IsJumpPressed = false;
            OnJumpReleased?.Invoke();
        }
    }
}