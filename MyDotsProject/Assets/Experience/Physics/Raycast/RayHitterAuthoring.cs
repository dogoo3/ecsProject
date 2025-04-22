using Unity.Entities;
using UnityEngine;


namespace Physics.Raycast
{
    public class RayHitterAuthoring : MonoBehaviour
    {
        public float value;

        class Baker : Baker<RayHitterAuthoring>
        {
            public override void Bake(RayHitterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RayHitter
                {
                    value = authoring.value
                });
            }
        }
    }

    public struct RayHitter : IComponentData
    {
        public float value;

        public string AddReturn()
        {
            return $"함수 반환 : {value+123f}";
        }
    }
}
