using System;
using System.Collections.Generic;

namespace FSM
{
    public class StateMachine<TState, TEvent>
    {
        bool m_running;
        public bool Running
        {
            get { return m_running; }
        }

        Stack<State<TState, TEvent>> m_states;
        Dictionary<TState, State<TState, TEvent>> m_statesDict;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FSM.StateMachine"/> class.
        /// </summary>
        /// <param name="initState">Init state.</param>
        /// <param name="stateComparer">provide IEqualityComparer for state dictionary</param>
        public StateMachine(State<TState, TEvent> initState, IEqualityComparer<TState> stateComparer = null)
        {
            m_running = false;
            m_states = new Stack<State<TState, TEvent>>();
            m_states.Push(initState);
            m_statesDict = new Dictionary<TState, State<TState, TEvent>>(stateComparer);
        }

        /// <summary>
        /// Add state.
        /// </summary>
        /// <param name="state">State.</param>
        public void AddState(State<TState, TEvent> state)
        {
            m_statesDict.Add(state.Id, state);
        }

        /// <summary>
        /// Get state.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="stateId">State identifier.</param>

        public State<TState, TEvent> GetState(TState stateId)
        {
            State<TState, TEvent> state;
            if (m_statesDict.TryGetValue(stateId, out state))
            {
                return state;
            }

            return null;
        }

        /// <summary>
        /// Start the StateMachine
        /// </summary>
        public void Start()
        {
            if (!m_running)
            {
                var currentState = m_states.Peek();
                currentState.OnEnter();
                m_running = true;
            }
        }

        /// <summary>
        /// Stop the StateMachine.
        /// </summary>
        public void Stop()
        {
            while (m_states.Count != 0)
            {
                var state = m_states.Pop();
                state.OnExit();
            }
            m_running = false;
        }

        /// <summary>
        /// Update the current state.
        /// </summary>
        public void Update()
        {
            if (!m_running || m_states.Count == 0)
                return;

            var currentState = m_states.Peek();
            var transition = currentState.Update();
            DoTransition(transition);
        }

        /// <summary>
        /// Handle event and do transition based on current state
        /// </summary>
        /// <param name="e">E.</param>
        public void HandleEvent(TEvent e)
        {
            if (!m_running || m_states.Count == 0)
                return;

            var currentState = m_states.Peek();
            DoTransition(currentState.HandleEvent(e));
        }

        /// <summary>
        /// Handle event and do transition based on current state
        /// </summary>
        /// <param name="e">E.</param>
        /// <param name="args">Arguments.</param>
        public void HandleEvent(TEvent e, object args)
        {
            if (!m_running || m_states.Count == 0)
                return;

            var currentState = m_states.Peek();
            DoTransition(currentState.HandleEvent(e, args));
        }

        void DoTransition(Transition transition)
        {
            if (!m_running)
                return;

            switch (transition.Op)
            {
                case TransitionOperation.None:
                    break;
                case TransitionOperation.Pop:
                    Pop();
                    break;
                case TransitionOperation.Push:
                    {
                        var stateTransition = transition as StateTransition<TState>;
                        Push(stateTransition.State);
                    }
                    break;
                case TransitionOperation.Switch:
                    {
                        var stateTransition = transition as StateTransition<TState>;
                        Switch(stateTransition.State);
                    }
                    break;
                case TransitionOperation.Quit:
                    Stop();
                    break;
            }
        }

        void Pop()
        {
            if (m_states.Count != 0)
            {
                var currentState = m_states.Pop();
                currentState.OnExit();

                if (m_states.Count != 0)
                {
                    currentState = m_states.Peek();
                    currentState.OnResume();
                }
            }
        }

        void Push(TState stateId)
        {
            if (m_states.Count != 0)
            {
                m_states.Peek().OnPause();
            }

            var state = GetState(stateId);
            m_states.Push(state);
            state.OnEnter();
        }

        void Switch(TState stateId)
        {
            if (m_states.Count != 0)
            {
                var currentState = m_states.Pop();
                currentState.OnExit();
            }

            var state = GetState(stateId);
            m_states.Push(state);
            state.OnEnter();
        }
    }
}
