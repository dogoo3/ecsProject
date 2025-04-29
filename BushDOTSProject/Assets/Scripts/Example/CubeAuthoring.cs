using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CubeAuthoring : MonoBehaviour
{
    public float deg;

    public class Baker : Baker<CubeAuthoring>
    {
        public override void Bake(CubeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CubeComponent{
               angle = math.radians(authoring.deg)
            });
        }
    }
}

public struct CubeComponent : IComponentData
{
    public float angle;
}