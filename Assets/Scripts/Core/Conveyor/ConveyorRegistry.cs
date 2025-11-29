using System;
using System.Collections.Generic;

namespace Core.Conveyor
{
    public static class ConveyorRegistry
    {
        public static readonly List<ConveyorController> All = new List<ConveyorController>();
        public static event Action OnListChanged;

        internal static void Register(ConveyorController c)
        {
            if (c == null) return;
            if (!All.Contains(c))
            {
                All.Add(c);
                OnListChanged?.Invoke();
            }
        }

        internal static void Unregister(ConveyorController c)
        {
            if (c == null) return;
            if (All.Remove(c))
                OnListChanged?.Invoke();
        }
    }
}