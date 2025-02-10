using Unity.Entities;
using UnityEngine;

namespace Instantiate
{
    public class BulletAuthoring : MonoBehaviour
    {
        public class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Bullet>(entity);
                AddComponent<BulletTag>(entity);
            }
        }
    }

    public struct Bullet : IComponentData
    {

    }

    public struct BulletTag : IComponentData
    {

    }
}
