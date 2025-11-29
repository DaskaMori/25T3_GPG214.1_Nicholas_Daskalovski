namespace Core.Conveyor
{
    public class JammedState : IConveyorState
    {
        public ConveyorStateId Id => ConveyorStateId.Jammed;
        public void OnEnter(ConveyorController ctx) { }
        public void OnExit(ConveyorController ctx) { }
        public bool Allows(ConveyorStateId target) => target != ConveyorStateId.Jammed;

        public void Update(ConveyorController ctx, float dt)
        {
            foreach (var rb in ctx.Crates)
            {
                if (!rb) continue;
                ConveyorController.SetXZVelocity(rb, UnityEngine.Vector3.zero);
            }
        }
    }
}