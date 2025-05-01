using Unity.Entities;
using UnityEngine;

// Authoring class to mark an entity as relevant. This is used in samples where the position of an entity
// (e.g. the player or camera) indicates which scene/sections to load.
public class CharacterAuthoring : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public GameObject headObj, eyeObj;
    public class Baker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            if (authoring.headObj != null && authoring.eyeObj != null)
            {
                AddComponent(entity, new CharacterComponent
                {
                    moveSpeed = authoring.moveSpeed,
                    headEntity = GetEntity(authoring.headObj, TransformUsageFlags.Dynamic),
                    eyeEntity = GetEntity(authoring.eyeObj, TransformUsageFlags.Dynamic)
                });
            }
            else
            {
                Debug.Log("No include GameObject!");
            }
        }
    }
}

public struct CharacterComponent : IComponentData
{
    public float moveSpeed;
    public Entity headEntity;
    public Entity eyeEntity;
}

