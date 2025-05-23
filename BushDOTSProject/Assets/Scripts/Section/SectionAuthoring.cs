using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SectionAuthoring : MonoBehaviour
{
    public float radius;
    public float height = 2.0f;

    class Baker : Baker<SectionAuthoring>
    {
        public override void Bake(SectionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Section
            {
                Radius = authoring.radius,
                Center = GetComponent<Transform>().position,
                height = authoring.height
            });
        }
    }
}

public struct Section : IComponentData
{
    public float Radius; // Proximity radius within which to consider loading a section
    public float3 Center;
    public float height;
}

