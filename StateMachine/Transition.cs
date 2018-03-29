using System;
namespace FSM
{
    /// <summary>
    /// transition operation
    /// </summary>
    public enum TransitionOperation
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None,
        /// <summary>
        /// Remove the current state and resume the next state on the stack or stop if there is none
        /// </summary>
        Pop,
        /// <summary>
        /// Pause the current state and push a new state onto the stack
        /// </summary>
        Push,
        /// <summary>
        /// Remove the current state on the stack and push a new one
        /// </summary>
        Switch,
        /// <summary>
        /// Stop and remove all states
        /// </summary>
        Quit,
    }

    public class Transition
    {
        public static Transition None = new Transition { Op = TransitionOperation.None };
        public static Transition Pop = new Transition { Op = TransitionOperation.Pop };
        public static Transition Quit = new Transition { Op = TransitionOperation.Quit };

        public TransitionOperation Op;
    }

    public class StateTransition<TState>:Transition
    {
        public TState State;

        public StateTransition(TransitionOperation op, TState state)
        {
            this.Op = op;
            this.State = state;
        }
    }
}
