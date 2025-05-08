using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

// PhysicsSystemGroup 이후에 실행되도록 설정
// Collision 이벤트는 PhysicsSimulationGroup에서 생성되므로 그 이후에 처리해야 합니다.
// StatefulCollisionEventBufferSystem이 Collision 이벤트를 StatefulCollisionEvent로 변환하므로 그 이후에 실행합니다.
// 필요에 따라 UpdateBefore 또는 UpdateAfter 속성을 조정할 수 있습니다.
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateAfter(typeof(StatefulCollisionEventBufferSystem))]
public partial struct CollisionLoggingSystem : ISystem
{
    // Burst 컴파일을 활성화하여 성능 향상
    // Burst는 struct가 unmanaged data만 포함할 때 작동합니다.
    // RefRW<DynamicBuffer<StatefulCollisionEvent>>는 unmanaged이므로 Burst에서 사용 가능합니다.

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // 이 시스템이 실행되려면 StatefulCollisionEvent 버퍼가 있는 엔티티가 최소 하나 이상 있어야 합니다.
        // state.RequireForUpdate<StatefulCollisionEvent>(); // IBufferElementData 자체로는 RequireForUpdate를 사용할 수 없습니다.
        // 대신 OnUpdate에서 필터링하거나 Job에서 사용합니다.

        // StatefulCollisionEvent 버퍼를 가진 entity 중 CollisionManager 컴포넌트가 없는 엔티티를 찾아 초기화함
        // var query = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<StatefulCollisionEvent>(), ComponentType.Exclude<CollisionManager>());
        
        // NativeHashSet : https://docs.unity3d.com/Packages/com.unity.collections@0.11/api/Unity.Collections.NativeHashSet-1.html
        
        // 충돌된 entity를 저장할 수 있는 NativeHashSet 선언/정의
        // Allocator : Native Container(Set, Array 등)를 new로 할당할 때, 할당된 container가 메모리에 유지되는 시간을 설정 가능
        // Allocator.Temp : 가장 빠르게 할당되나, 1프레임을 초과하면 자동 할당 해제.
        // Allocator.TempJob : Temp보다는 할당이 느리나, Persistent보다 빠름(중간). 
        // Allocator.Persistent : 할당은 제일 느리지만 지속 유지됨. 그래서 Dispose()를 반드시 OnDestroy()에서 호출해 주어야 memory leak가 일어나지 않음.
        // https://everyday-devup.tistory.com/98
        // foreach (var entity in query.ToEntityArray(Allocator.Temp))
        // {
        //     state.EntityManager.AddComponent<CollisionManager>(entity);
        //     state.EntityManager.SetComponentEnabled<CollisionManager>(entity, true);
        //     state.EntityManager.SetComponentData(entity, new CollisionManager { collisionEntities = new NativeHashSet<Entity>(16, Allocator.Persistent) });
        // }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // ISystem에서는 직접적으로 foreach를 사용하기보다 Job을 스케줄링하는 것이 일반적입니다.
        // 하지만 간단한 로깅의 경우, Main Thread에서 실행해도 큰 부담이 없다면 SystemAPI를 사용할 수 있습니다.

        EntityManager entityManager = state.EntityManager;

        // Collision과 관련된 Singleton을 생성합니다.
        // Entity collisionManager = SystemAPI.GetSingletonEntity<CollisionManagerComponent>();
        // CollisionManagerComponent cmc = entityManager.GetComponentData<CollisionManagerComponent>(collisionManager);
        // cmc.collisionEntities = new NativeHashSet<Entity>(16, Allocator.Persistent);

        // StatefulCollisionEvent 버퍼를 가진 모든 엔티티를 순회합니다.
        foreach (var (collisionBuffer, entity) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
        {
            foreach (var collisionEvent in collisionBuffer)
            {
                Entity otherEntity = collisionEvent.GetOtherEntity(entity);

                switch (collisionEvent.State)
                {
                    case StatefulEventState.Enter:
                        // 하기에 작업항목 구현
                        Debug.Log($"Collision Enter - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                        break;
                    case StatefulEventState.Stay:
                        // 충돌 중일 때 매 프레임 호출됩니다. 필요하다면 로그를 남기세요.
                        // Debug.Log($"Collision Stay - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                        break;
                    case StatefulEventState.Exit:
                        // 하기에 작업항목 구현
                        Debug.Log($"Collision Exit - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                        break;
                    case StatefulEventState.Undefined:
                        Debug.LogWarning($"Collision Event in Undefined state - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                        break;
                }

                // if (collisionEvent.TryGetDetails(out var details) && details.IsValid)
                // {
                //     // 디테일 정보가 필요하다면 여기서 접근할 수 있습니다.
                //     Debug.Log($"  - Number of Contact Points: {details.NumberOfContactPoints}");
                //     Debug.Log($"  - Estimated Impulse: {details.EstimatedImpulse}");
                //     Debug.Log($"  - Average Contact Point Position: {details.AverageContactPointPosition}");
                // }
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // var query = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<CollisionManager>());
        // foreach (var entity in query.ToEntityArray(Allocator.Temp))
        // {
        //     state.EntityManager.GetComponentData<CollisionManager>(entity).Dispose();
        // }
    }
}

// Extension 메서드는 그대로 사용할 수 있습니다.
public static class StatefulCollisionEventExtensions
{
    public static Entity GetOtherEntity(this StatefulCollisionEvent collisionEvent, Entity entity)
    {
        if (collisionEvent.EntityA == entity)
        {
            return collisionEvent.EntityB;
        }
        else if (collisionEvent.EntityB == entity)
        {
            return collisionEvent.EntityA;
        }
        else
        {
            Debug.LogError("Provided entity is not part of this collision event.");
            return Entity.Null;
        }
    }
}

