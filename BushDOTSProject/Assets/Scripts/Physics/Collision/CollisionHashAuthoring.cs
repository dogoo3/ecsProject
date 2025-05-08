using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CollisionHashAuthoring : MonoBehaviour
{
    class Baker : Baker<CollisionHashAuthoring>
    {
        public override void Bake(CollisionHashAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CollisionHashComponent{});
        }
    }
}

public struct CollisionHashComponent : IComponentData, IEnableableComponent
{
    // public NativeHashSet<Entity> collisionEntities; // 물리적 충돌(collision)이 입력된 오브젝트를 set으로 관리

    // public void CreateSet()
    // {
    //     if(!collisionEntities.IsCreated)
    //     {
    //         collisionEntities = new NativeHashSet<Entity>(16, Allocator.Persistent);
    //     }
    // }
    // public void DisposeSet()
    // {
    //     if(collisionEntities.IsCreated)
    //         collisionEntities.Dispose();
    // }
}