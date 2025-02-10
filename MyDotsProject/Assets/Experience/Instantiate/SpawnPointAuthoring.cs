using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Instantiate
{
    public class SpawnPointAuthoring : MonoBehaviour
    {
        public GameObject instantiatePrefab;

        class Baker : Baker<SpawnPointAuthoring>
        {
            public override void Bake(SpawnPointAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SpawnPoint
                {
                    Prefab = GetEntity(authoring.instantiatePrefab, TransformUsageFlags.Dynamic)    
                });
            }
        }
    }

    public struct SpawnPoint : IComponentData
    {
        public Entity Prefab;
    }
}
