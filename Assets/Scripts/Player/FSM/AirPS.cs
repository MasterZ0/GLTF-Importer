using GLTFImporter.Shared;

namespace GLTFImporter.Player.FSM
{
    public class AirPS : PlayerState
    {
        private readonly Timer timer = new();

        private bool falling;

        private bool MinJumpApplied => timer.Counter > Data.JumpRangeDuration.x;
        private bool MaxJumpApplied => timer.Counter >= Data.JumpRangeDuration.y;

        public override void EnterState()
        {
            timer.Reset();
            falling = !StateMachine.IsJumping;

            if (falling)
            {
                Physics.SetGravityScale(Data.FallingGravity);
                Animator.Falling();
            }
            else
            {
                Animator.Jump();

                Physics.SetGravityScale(Data.JumpGravity);
            }
        }

        public override void UpdateState()
        {
            timer.FixedTick();
            Physics.Move(Data.WalkSpeed);

            if (StateMachine.IsJumping)
            {
                ApplyJump();
                return;
            }

            if (!falling && Physics.Velocity.y < 0) // Wait until start fall
            {
                falling = true;
                Physics.SetGravityScale(Data.FallingGravity);
                Animator.Falling();
            }

            if (Physics.CheckGround())
            {
                Animator.OverrideIdleAsLanding();
                SwitchState<IdlePS>();
            }
        }

        private void ApplyJump()
        {
            Physics.Jump(Data.JumpVelocity);

            if (MinJumpApplied && (!Inputs.IsJumpPressed || MaxJumpApplied))
            {
                StateMachine.IsJumping = false;
            }
        }
    }
}