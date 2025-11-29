namespace Core.Conveyor
{
    public class PoweredState : IConveyorState
    {
        public ConveyorStateId Id => ConveyorStateId.Powered;
        public void OnEnter(ConveyorController ctx) { }
        public void OnExit(ConveyorController ctx) { }
        public bool Allows(ConveyorStateId t) => true;

        public void Update(ConveyorController ctx, float dt)
        {
            var dir = ctx.DirNorm;
            foreach (var rb in ctx.Crates)
            {
                if (!rb) continue;
                float speed = ctx.baseSpeed / ConveyorController.GetWeight(rb);
                ConveyorController.SetXZVelocity(rb, dir * speed);
            }
        }
    }

}