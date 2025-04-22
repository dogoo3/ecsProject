using Unity.Collections;
using Unity.Entities;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace BUSH.Physics.Collider
{
    public class SphereAuthoring : MonoBehaviour
    {
        // 플레이어 ID
        public string playerName;
        public float moveSpeed;
        class Baker : Baker<SphereAuthoring>
        {
            public override void Bake(SphereAuthoring authoring)
            {
                var _entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(_entity, new SphereComponent 
                {
                    entity = _entity,
                    moveSpeed = authoring.moveSpeed,
                    playerName = authoring.playerName
                });
            }
        }
    }

    public struct SphereComponent : IComponentData
    {
        public Entity entity;
        public FixedString32Bytes playerName;
        public float moveSpeed;
    }
}