using Unity.Entities;
using UnityEngine;

// Authoring class to mark an entity as relevant. This is used in samples where the position of an entity
// (e.g. the player or camera) indicates which scene/sections to load.
public class SphereAuthoring : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public GameObject headObj, eyeObj;
    public class SphereBaker : Baker<SphereAuthoring>
    {
        public override void Bake(SphereAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            if (authoring.headObj != null && authoring.eyeObj != null)
            {
                AddComponent(entity, new SphereComponent
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

public struct SphereComponent : IComponentData
{
    public float moveSpeed;
    public Entity headEntity;
    public Entity eyeEntity;
}

