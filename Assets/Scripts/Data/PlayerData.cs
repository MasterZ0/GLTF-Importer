using GLTFImporter.Shared;
using UnityEngine;

namespace GLTFImporter.Data
{

    [CreateAssetMenu(menuName = MenuPath.Data + "Player", fileName = "New" + nameof(PlayerData))]
    public class PlayerData : ScriptableObject
    {
        [Header("Physics")]
        [SerializeField] private float groundCheckRadius = 0.2f;

        [Header(" - Movement")]
        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 15f;

        [Header(" - Rotation")]
        [SerializeField] private float mouseSensitivity = 0.2f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private Vector2 cameraRangeRotation = new Vector2(-35f, 75f);

        [Header(" - Jump")]
        [SerializeField] private float jumpVelocity = 1f;
        [SerializeField] private Vector2 jumpRangeDuration = new Vector2(-0.04f, 0.25f);

        [Header(" - Gravity")]
        [SerializeField] private float groundGravity = 3f;
        [SerializeField] private float fallingGravity = 3f;
        [SerializeField] private float jumpGravity = 1f;
        [SerializeField] private float mass = 1f;
        [SerializeField] private float maxFallingVelocity = 11f;

        [Header(" - Visual")]
        [SerializeField] private float animationBlendDamp = 0.1f;

        public float GroundCheckRadius => groundCheckRadius;
        public float Mass => mass;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public Vector2 CameraRangeRotation => cameraRangeRotation;
        public float RotationSmoothTime => rotationSmoothTime;
        public float MaxFallingVelocity => maxFallingVelocity;

        public float MouseSensitivity => mouseSensitivity;
        public float FallingGravity => fallingGravity;
        public float JumpGravity => jumpGravity;
        public float JumpVelocity => jumpVelocity;
        public Vector2 JumpRangeDuration => jumpRangeDuration;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        public float GroundGravity => groundGravity;
        public float AnimationBlendDamp => animationBlendDamp;
    }
}