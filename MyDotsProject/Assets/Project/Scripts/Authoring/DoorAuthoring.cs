using Unity.Entities;
using UnityEngine;

public class DoorAuthoring : MonoBehaviour
{
    public float openangle = 0;
    public class DoorAuthoringBaker : Baker<DoorAuthoring>
    {
        public override void Bake(DoorAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new DoorComponent {
                entity = entity,
                openAngle = authoring.openangle
            });
        }
    }
}

public struct DoorComponent : IComponentData
{
    public Entity entity;
    public float openAngle;
}