using UnityEngine;
using CharacterXYZ.Player.FSM;
using CharacterXYZ.Inputs;
using CharacterXYZ.Data;

namespace CharacterXYZ.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Player Controller")]
        [SerializeField] private PlayerData data;
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private PlayerAnimator playerAnimator;
        [SerializeField] private PlayerCamera playerCamera;

        public PlayerData Data => data;
        public PlayerPhysics Physics => playerPhysics;
        public PlayerAnimator Animator => playerAnimator;
        public PlayerCamera Camera => playerCamera;
        public PlayerInputs Inputs => playerInputs;

        private PlayerInputs playerInputs;
        private PlayerFSM stateMachine;

        private void Awake()
        {
            playerPhysics.Init(this);
            playerAnimator.Init(this);
            playerCamera.Init(this);

            playerInputs = new PlayerInputs();
            stateMachine = PlayerFSM.Create<IdlePS>(this);
        }

        private void OnDestroy()
        {
            playerInputs.Dispose();
        }

        private void FixedUpdate()
        {
            stateMachine.Update();
            playerPhysics.Update();
            playerCamera.Update();
            playerAnimator.Update();
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            stateMachine.DrawGizmos();
            playerPhysics.DrawGizmos();
        }
    }
}