using UnityEngine;
using GLTFImporter.Player.FSM;
using GLTFImporter.Inputs;
using GLTFImporter.Data;

namespace GLTFImporter.Player
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

        public void OnChangeAnimation(Animation animation)
        {
            playerAnimator.ChangeAnimation(animation);
        }

        private void FixedUpdate()
        {
            stateMachine.Update();
            playerPhysics.Update();
            playerCamera.Update();
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