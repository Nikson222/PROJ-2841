using System;
using System.Collections.Generic;
using Zenject;

namespace _Scripts._Infrastructure.StateMachine
{
    public abstract class StateMachine
    {
        private Dictionary<Type, IExitableState> _states = new();
        private IExitableState _activeState;
        
        private readonly DiContainer _container;
        
        public void RegisterState(IExitableState state)
        {
            if (_states.ContainsKey(state.GetType()))
                _states.Remove(state.GetType());
            
            _states.Add(state.GetType(), state);
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            if(_activeState is TState) return;
            
            var state = ChangeState<TState>();
            state.Enter(payload); 
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
        
            TState state = GetState<TState>();
            _activeState = state;
        
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            if(_states.ContainsKey(typeof(TState)))
                return _states[typeof(TState)] as TState;
                
            throw new ArgumentException("No exist this state in dictionary");
        }
    }
}