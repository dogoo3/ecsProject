using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TriggerHashAuthoring : MonoBehaviour
{
    // class Baker : Baker<TriggerHashAuthoring>
    // {
    //     public override void Bake(TriggerHashAuthoring authoring)
    //     {
    //         var entity = GetEntity(TransformUsageFlags.None);
    //         AddComponent(entity, new TriggerHashComponent{});
    //     }
    // }
}

public struct TriggerBufferElement : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(TriggerBufferElement e) { return e.Value; }
    public static implicit operator TriggerBufferElement(Entity e) { return new TriggerBufferElement { Value = e }; }
}

// public struct CollisionManager : IComponentData, IEnableableComponent
// {
//     public NativeHashSet<Entity> collisionEntities; // 물리적 충돌(collision)이 입력된 오브젝트를 set으로 관리
//     public NativeHashSet<Entity> triggerEntities; // 구조적 충돌(trigger)이 입력된 오브젝트를 set으로 관리

//     public void Dispose()
//     {
//         if(collisionEntities.IsCreated)
//             collisionEntities.Dispose();
//         if(triggerEntities.IsCreated)
//             triggerEntities.Dispose();
//     }
// }
