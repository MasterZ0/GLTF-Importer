namespace GLTFImporter.Player.FSM
{
    public sealed class WalkPS : StandingSPS
    {
        public override void EnterState()
        {
            base.EnterState();
            Animator.Walk();
        }

        public override void UpdateStanding()
        {
            if (!Inputs.IsMovePressed)
            {
                SwitchState<IdlePS>();
                return;
            }

            if (Inputs.IsSprintPressed)
            {
                SwitchState<RunPS>();
            }

            Physics.Move(Data.WalkSpeed);
        }
    }
}