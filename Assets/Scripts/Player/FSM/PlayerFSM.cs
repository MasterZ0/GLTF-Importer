using CharacterXYZ.StateMachine;
using UnityEngine;

namespace CharacterXYZ.Player.FSM
{
    public class PlayerFSM : FiniteStateMachine
    {
        public PlayerController Controller { get; private set; }
        public bool IsJumping { get; set; }

        protected PlayerFSM(PlayerController controller) : base(typeof(PlayerState))
        {
            Controller = controller;
        }

        /// <typeparam name="T">First State</typeparam>
        public static PlayerFSM Create<T>(PlayerController controller) where T : PlayerState, new()
        {
            PlayerFSM stateMachine = new PlayerFSM(controller);
            stateMachine.SetFirstState<T>();
            return stateMachine;
        }
    }
}