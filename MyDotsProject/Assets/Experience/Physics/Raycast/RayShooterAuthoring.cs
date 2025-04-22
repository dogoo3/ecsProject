using Unity.Entities;
using UnityEngine;

namespace Physics.Raycast
{
    public class RayShooterAuthoring : MonoBehaviour
    {
        
        public class RayShooterBaker : Baker<RayShooterAuthoring>
        {
            public override void Bake(RayShooterAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RayShooterComponent
                {
                    
                });
            }
        }

    }
    public struct RayShooterComponent : IComponentData
    {
        public Entity entity;
    }
}