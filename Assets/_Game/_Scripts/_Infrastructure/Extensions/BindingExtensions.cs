using _Scripts._Infrastructure.StateMachine;
using Zenject;

namespace _Scripts._Infrastructure.Extensions
{
    public static class BindingExtensions
    {
        public static void BindAndRegisterState<TState, TStateMachine>(this DiContainer container) 
            where TState : IExitableState where TStateMachine : StateMachine.StateMachine
        {
            container.BindInterfacesAndSelfTo<TState>().AsSingle();
            var stateMachine = container.Resolve<TStateMachine>();
            stateMachine.RegisterState(container.Resolve<TState>());
        }
    }
}