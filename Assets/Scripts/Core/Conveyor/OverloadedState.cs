namespace Core.Conveyor
{
    public class OverloadedState : IConveyorState
    {
        public ConveyorStateId Id => ConveyorStateId.Overloaded;
        public void OnEnter(ConveyorController ctx) { }
        public void OnExit(ConveyorController ctx) { }
        public bool Allows(ConveyorStateId target) => true;

        public void Update(ConveyorController ctx, float dt)
        {
            var dir = ctx.DirNorm;
            foreach (var rb in ctx.Crates)
            {
                if (!rb) continue;
                float speed = 0.5f * (ctx.baseSpeed / ConveyorController.GetWeight(rb));
                ConveyorController.SetXZVelocity(rb, dir * speed);
            }
        }
    }
}