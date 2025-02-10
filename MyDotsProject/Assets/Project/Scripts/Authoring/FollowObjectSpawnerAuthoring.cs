using UnityEngine;
using Unity.Entities;

public class FollowObjectSpawnerAuthoring : MonoBehaviour
{
    public GameObject instantiatePrefab;

    class Baker : Baker<FollowObjectSpawnerAuthoring>
    {
        public override void Bake(FollowObjectSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new FollowObjectSpawner
            {
                prefab = GetEntity(authoring.instantiatePrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct FollowObjectSpawner : IComponentData
{
    public Entity prefab;
}