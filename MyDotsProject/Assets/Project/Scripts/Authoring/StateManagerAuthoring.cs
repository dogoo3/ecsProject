using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public class StateManagerAuthoring : MonoBehaviour
{
    class Baker : Baker<StateManagerAuthoring>
    {
        public override void Bake(StateManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new StateManager{
                isEnterBuilding = false
            });
        }
    }
}

public struct StateManager : IComponentData
{
    public bool isEnterBuilding;
}
