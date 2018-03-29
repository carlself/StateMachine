using System;
namespace FSM
{
    public abstract class State<TState, TEvent>
    {
        public virtual TState Id { get; }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public virtual Transition HandleEvent(TEvent e) { return Transition.None; }
        public virtual Transition HandleEvent(TEvent e, object args) { return Transition.None; }
        public virtual Transition Update() { return Transition.None; }
    }
}
