namespace Core.Conveyor
{
    public interface IConveyorState
    {
        ConveyorStateId Id { get; }
        void OnEnter(ConveyorController ctx);
        void OnExit(ConveyorController ctx);
        void Update(ConveyorController ctx, float fixedDeltaTime);
        bool Allows(ConveyorStateId target);
    }
}