namespace GLTFImporter.Player.FSM
{
    public sealed class RunPS : StandingSPS
    {
        public override void EnterState()
        {
            base.EnterState();
            Animator.Run();
        }

        public override void UpdateStanding()
        {
            if (!Inputs.IsMovePressed)
            {
                SwitchState<IdlePS>();
                return;
            }

            if (!Inputs.IsSprintPressed)
            {
                SwitchState<WalkPS>();
            }

            Physics.Move(Data.RunSpeed);
        }
    }
}