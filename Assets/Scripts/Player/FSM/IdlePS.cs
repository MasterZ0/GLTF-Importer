namespace GLTFImporter.Player.FSM
{
    public sealed class IdlePS : StandingSPS
    {
        public override void EnterState()
        {   
            base.EnterState();
            Animator.Idle();
            Physics.SetGravityScale(Data.GroundGravity);
        }

        public override void UpdateStanding()
        {
            if (Inputs.IsMovePressed)
            {
                SwitchState<WalkPS>();
            }
        }
    }
}