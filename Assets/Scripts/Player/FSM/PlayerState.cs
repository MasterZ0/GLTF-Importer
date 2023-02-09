using CharacterXYZ.Data;
using CharacterXYZ.Inputs;
using CharacterXYZ.StateMachine;

namespace CharacterXYZ.Player.FSM
{
    public abstract class PlayerState : State<PlayerFSM>
    {
        protected PlayerController Controller => StateMachine.Controller;
        protected PlayerData Data => Controller.Data;
        protected PlayerPhysics Physics => Controller.Physics;
        protected PlayerAnimator Animator => Controller.Animator;
        protected PlayerCamera Camera => Controller.Camera;
        protected PlayerInputs Inputs => Controller.Inputs;
    }
}