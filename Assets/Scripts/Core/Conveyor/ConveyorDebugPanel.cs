using UnityEngine;

namespace Core.Conveyor
{
    public class ConveyorDebugPanel : MonoBehaviour
    {
        public ConveyorController controller;

        private void Reset()
        {
            if (!controller) controller = FindObjectOfType<ConveyorController>();
        }

        private void OnGUI()
        {
            if (!controller) return;

            GUILayout.BeginArea(new Rect(10, 10, 220, 190), GUI.skin.window);
            GUILayout.Label("Conveyor State: " + controller.GetState());

            if (GUILayout.Button("Powered"))    controller.SetState(ConveyorStateId.Powered,  true); // force
            if (GUILayout.Button("Paused"))     controller.SetState(ConveyorStateId.Paused,   true);
            if (GUILayout.Button("Reversed"))   controller.SetState(ConveyorStateId.Reversed, true);
            if (GUILayout.Button("Jammed"))     controller.SetState(ConveyorStateId.Jammed,   true);
            if (GUILayout.Button("Overloaded")) controller.SetState(ConveyorStateId.Overloaded, true);

            GUILayout.EndArea();
        }
    }
}