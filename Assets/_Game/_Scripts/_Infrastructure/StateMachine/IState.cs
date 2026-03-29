namespace _Scripts._Infrastructure.StateMachine
{
    public interface IState : IExitableState
    {
        void Enter();
    }

    public interface IPayloadState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }

    public interface IUpdatablePayloadState<TPayload> : IPayloadState<TPayload>
    {
        void Update(TPayload payload);
    }
    
    public interface IExitableState
    {
        void Exit();
    }
}