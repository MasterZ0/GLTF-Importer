namespace GLTFImporter.Player.FSM
{
    public abstract class StandingSPS : PlayerState
    {
        public override void EnterState()
        {
            Animator.Idle();

            Inputs.OnJumpPressed += OnJumpPressed;
        }

        public override void ExitState()
        {
            Inputs.OnJumpPressed -= OnJumpPressed;
        }

        public sealed override void UpdateState()
        {
            if (!Physics.CheckGround())
            {
                SwitchState<AirPS>();
                return;
            }

            UpdateStanding();
        }

        public virtual void UpdateStanding() { }

        private void OnJumpPressed()
        {
            StateMachine.IsJumping = true;
            SwitchState<AirPS>();
        }
    }
}