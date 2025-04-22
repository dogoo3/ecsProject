using Unity.Entities;
using UnityEngine;

public class ShootRayAuthoring : MonoBehaviour
{
    public float length;
    public class ShootRayAuthoringBaker : Baker<ShootRayAuthoring>
    {
        public override void Bake(ShootRayAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new RayShooterComponent
            {
                entity = entity,
                length = authoring.length
            });
        }
    }
}

public struct RayShooterComponent : IComponentData
{
    public Entity entity;
    public float length;
}