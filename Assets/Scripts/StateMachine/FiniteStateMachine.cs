using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace GLTFImporter.StateMachine
{
    public class FiniteStateMachine
    {
        public State CurrentState { get; protected set; }
        public State LastState { get; protected set; }
        public State NextState { get; protected set; }

        private readonly List<State> states = new();

        /// <summary>
        /// Ex: new FiniteStateMachine(typeof(BaseState));
        /// </summary>
        protected FiniteStateMachine(Type stateBaseType)
        {
            IEnumerable<Type> types = Assembly.GetAssembly(stateBaseType).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(stateBaseType));

            CreateAndSetInstances(types);
        }

        /// <summary>
        /// Ex: new FiniteStateMachine(typeof(State1), typeof(State2), etc...);
        /// </summary>
        public FiniteStateMachine(params Type[] types)
        {
            CreateAndSetInstances(types);
        }

        /// <summary>
        /// Ex: new FiniteStateMachine(stateList);
        /// </summary>
        public FiniteStateMachine(IEnumerable<State> states)
        {
            foreach (State state in states)
            {
                state.SetupState(this);
                this.states.Add(state);
            }
        }

        private void CreateAndSetInstances(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                State state = Activator.CreateInstance(type) as State;
                state.SetupState(this);
                states.Add(state);
            }
        }

        /// <typeparam name="TBase">Base State</typeparam>
        /// <typeparam name="TFirst">First State</typeparam>
        public static FiniteStateMachine Create<TBase, TFirst>() where TBase : State where TFirst : TBase, new()
        {
            FiniteStateMachine stateMachine = new FiniteStateMachine(typeof(TBase));
            stateMachine.SetFirstState<TFirst>();
            return stateMachine;
        }

        public void SetFirstState<T>() where T : State, new()
        {
            CurrentState = GetState<T>();
            CurrentState.EnterState();
        }

        public virtual void SwitchState<T>() where T : State, new()
        {
            NextState = GetState<T>();

            CurrentState.ExitState();

            LastState = CurrentState;
            CurrentState = NextState;
            NextState = null;

            CurrentState.EnterState();
        }

        public void Update() => CurrentState.UpdateState();

        public void DrawGizmos() => CurrentState.DrawGizmos();

        private State GetState<T>() where T : State, new()
        {
            return states.First(s => s.GetType() == typeof(T));
        }
    }
}