using UnityEngine;
using System;

namespace CharacterXYZ.Player
{
    /// <summary>
    /// Handles player physics
    /// </summary>
    [Serializable]
    public sealed class PlayerPhysics : PlayerControllerComponent
    {
        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask invisibleLayer;

        [Header("Components")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CapsuleCollider bodyTrigger;

        [Header("Points")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private Transform interactableCheckPoint;
        [SerializeField] private Transform standUpCheckPoint;

        public Vector3 Velocity => characterController.velocity;
        public Transform Transform => characterController.transform;

        private float Weight => Data.Mass * Physics.gravity.y;
        private float EulerYCamera => Controller.Camera.CameraTarget.eulerAngles.y;

        private Vector3 velocity;
        private float gravityScale;
        private float moveSpeed;
        private float verticalVelocity;
        private float targetYRotation;
        private float rotationVelocity;

        private bool acceleration;

        internal bool CheckGround() => Physics.CheckSphere(groundCheckPoint.position, Data.GroundCheckRadius, groundLayer);

        public void Jump(float jumpHeight)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Weight);
        }

        public void SetGravityScale(float gravityScale) => this.gravityScale = gravityScale;

        internal void Move(float speed)
        {
            Vector2 direction = Controller.Inputs.Move;

            if (direction == Vector2.zero)
                return;

            moveSpeed = speed;
            if (direction.magnitude > 1)
            {
                direction = direction.normalized;
            }

            targetYRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + EulerYCamera;

            // Rotation
            float rotation = Mathf.SmoothDampAngle(Transform.eulerAngles.y, targetYRotation, ref rotationVelocity, Data.RotationSmoothTime);
            Transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        internal void Update()
        {
            UpdateAccelerationSpeed();
            UpdateVerticalVelocity();
            characterController.Move(velocity * Time.fixedDeltaTime);
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheckPoint.position, Data.GroundCheckRadius);
            Gizmos.DrawWireSphere(standUpCheckPoint.position, Data.GroundCheckRadius);
            Gizmos.DrawWireSphere(interactableCheckPoint.position, Data.InteractCheckRadius);
        }

        #region Private Methods
        /// <summary> Gravity and Jump Velocity </summary>
        private void UpdateVerticalVelocity()
        {
            if (CheckGround() && verticalVelocity < 0f) // Slope force?
            {
                verticalVelocity = -2f;
            }

            float maxFallingVelocity = -Data.MaxFallingVelocity;

            verticalVelocity += Weight * gravityScale * Time.fixedDeltaTime;
            if (verticalVelocity < maxFallingVelocity)
            {
                verticalVelocity = maxFallingVelocity;
            }

            velocity.y = verticalVelocity;
        }

        private void UpdateAccelerationSpeed()
        {
            // Reset state speed
            float speed;

            if (acceleration)
            {
                // Get the player's speed in a plane
                float currentHorizontalSpeed = new Vector3(Velocity.x, 0f, Velocity.z).magnitude;

                // Get Acceleration or Deceleration transition
                float transition = moveSpeed > currentHorizontalSpeed ? Data.Acceleration : Data.Deceleration;

                // Interpolate between current horizontal speed and target speed
                speed = Mathf.Lerp(currentHorizontalSpeed, moveSpeed, transition * Time.fixedDeltaTime);
                speed = Mathf.Min(moveSpeed, speed);
            }
            else
            {
                speed = moveSpeed;
                acceleration = true;
            }

            moveSpeed = 0f;

            // Update forward velocity
            Vector3 targetDirection = Quaternion.Euler(0f, targetYRotation, 0f) * Vector3.forward;
            velocity = targetDirection.normalized * speed;
        }
        #endregion
    }
}