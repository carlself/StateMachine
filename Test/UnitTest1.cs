using System;
using Xunit;
using FSM;

namespace Test
{
    enum PlayerState
    {
        Idle,
        Walk
    }

    enum InputEvent
    {
        Move,
    }

    class WalkState : State<PlayerState, InputEvent>
    {
        int m_ticks;
        public override PlayerState Id => PlayerState.Walk;
        public WalkState(int ticks)
        {
            m_ticks = ticks;
        }

        public override Transition Update()
        {
            m_ticks--;
            if (m_ticks < 0)
            {
                return Transition.Pop;
            }
            Console.WriteLine("Tick WalkState");
            return Transition.None;
        }

		public override void OnEnter()
		{
            Console.WriteLine("Enter WalkState");
            base.OnEnter();
		}

		public override void OnExit()
		{
            Console.WriteLine("Exit WalkState");
            base.OnExit();
		}
	}

    class IdleState : State<PlayerState, InputEvent>
    {
        public override PlayerState Id => PlayerState.Idle;

        public override Transition HandleEvent(InputEvent e)
        {
            switch (e)
            {
                case InputEvent.Move:
                    return new StateTransition<PlayerState>(TransitionOperation.Push, PlayerState.Walk);
            }
            return Transition.None;
        }

        public override void OnEnter()
        {
            Console.WriteLine("Enter IdleState");
            base.OnEnter();
        }

        public override void OnExit()
        {
            Console.WriteLine("Exit IdleState");
            base.OnExit();
        }

		public override void OnPause()
		{
            Console.WriteLine("Pause IdleState");
            base.OnPause();
		}

        public override void OnResume()
        {
            Console.WriteLine("Resume IdleState");
            base.OnResume();
        }
	}

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var walkState = new WalkState(3);
            var idleState = new IdleState();
            var sm = new StateMachine<PlayerState, InputEvent>(idleState);
            sm.AddState(walkState);
            sm.AddState(idleState);
            sm.Start();

            sm.HandleEvent(InputEvent.Move);

            for (int i = 0; i < 4; i++)
            {
                sm.Update();
            }
        }
    }
}