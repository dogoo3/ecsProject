using Unity.Entities;
using UnityEngine;

public class CharacterHeadAuthoring : MonoBehaviour
{
    public GameObject eyeObject;
    public class CharacterHeadBaker : Baker<CharacterHeadAuthoring>
    {
        public override void Bake(CharacterHeadAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new CharacterHeadComponent{});
            if(authoring.eyeObject != null)
            {
                AddComponent(entity, new EyePositionComponent
                {
                    eyeEntity = GetEntity(authoring.eyeObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}

public struct CharacterHeadComponent : IComponentData
{

}

public struct EyePositionComponent : IComponentData
{
    public Entity eyeEntity;
}