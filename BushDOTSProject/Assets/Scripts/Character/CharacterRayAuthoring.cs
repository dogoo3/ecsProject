using Unity.Entities;
using UnityEngine;

public class CharacterRayAuthoring : MonoBehaviour
{
    public float length;
    public class Baker : Baker<CharacterRayAuthoring>
    {
        public override void Bake(CharacterRayAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CharacterRayComponent
            {
                entity = entity,
                length = authoring.length
            });
        }
    }
}

public struct CharacterRayComponent : IComponentData
{
    public Entity entity;
    public float length;
}