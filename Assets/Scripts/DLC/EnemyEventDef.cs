using UnityEngine;
using Core.Conveyor;

[CreateAssetMenu(menuName="DLC/Enemy Event")]
public class EnemyEventDef : ScriptableObject
{
    public string id = "Saboteur";
    [TextArea] public string description;
    public Sprite icon;

    public ConveyorStateId targetState = ConveyorStateId.Reversed;
    public float duration = 8f;

    public bool affectAllConveyors = false;
    public int affectCount = 1;

    [Range(0f,1f)] public float weight = 1f;
}

