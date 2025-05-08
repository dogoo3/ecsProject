using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

// PhysicsSystemGroup 이후에 실행되도록 설정
// Collision 이벤트는 PhysicsSimulationGroup에서 생성되므로 그 이후에 처리해야 합니다.
// StatefulTriggerEventBufferSystem이 Collision 이벤트를 StatefulTriggerEvent로 변환하므로 그 이후에 실행합니다.
// 필요에 따라 UpdateBefore 또는 UpdateAfter 속성을 조정할 수 있습니다.
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
public partial struct TriggerLoggingSystem : ISystem
{
    // Burst 컴파일을 활성화하여 성능 향상
    // Burst는 struct가 unmanaged data만 포함할 때 작동합니다.
    // RefRW<DynamicBuffer<StatefulTriggerEvent>>는 unmanaged이므로 Burst에서 사용 가능합니다.
    // [BurstCompile] // SystemBase에서는 지원하지만 ISystem에서는 Job으로 처리해야 함 (아래 OnUpdate 참고)

    public void OnCreate(ref SystemState state)
    {
        // 이 시스템이 실행되려면 StatefulTriggerEvent 버퍼가 있는 엔티티가 최소 하나 이상 있어야 합니다.
        // state.RequireForUpdate<StatefulTriggerEvent>(); // IBufferElementData 자체로는 RequireForUpdate를 사용할 수 없습니다.
        // 대신 OnUpdate에서 필터링하거나 Job에서 사용합니다.
        state.RequireForUpdate<StateManager>();

        state.EntityManager.AddBuffer<TriggerBufferElement>(state.EntityManager.CreateEntity());
    }
    // [BurstCompile] // ISystem.OnUpdate는 BurstCompile을 직접 적용할 수 없습니다. Job을 사용하여 Burst를 활용합니다.
    public void OnUpdate(ref SystemState state)
    {
        // EntityManager 생성
        EntityManager entityManager = state.EntityManager;

        // 게임 내에서의 상태들을 관리하고, 상태에 따라 로직을 변경하기 위해서 StateManager를 불러온다.
        Entity stateManager = SystemAPI.GetSingletonEntity<StateManager>();

        // 트리거 충돌 상태를 관리합니다(엔티티에서 DynamicBuffer 가져오기)
        Entity triggerEntity = SystemAPI.GetSingletonEntity<TriggerBufferElement>();
        DynamicBuffer<TriggerBufferElement> entityArray = SystemAPI.GetBuffer<TriggerBufferElement>(triggerEntity);

        Debug.Log(entityArray.Length);

        // StatefulTriggerEvent 버퍼를 가진 모든 엔티티를 순회합니다.
        foreach (var (triggerBuffer, entity) in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>>().WithEntityAccess())
        {
            foreach (var triggerEvent in triggerBuffer)
            {
                Entity otherEntity = triggerEvent.GetOtherEntity(entity);

                switch (triggerEvent.State)
                {
                    case StatefulEventState.Enter:
                        if(!ContainsEntity(in entityArray, entity))
                        {
                            entityArray.Add(entity); // 암시적 변환 사용
                            Debug.Log($"Trigger Enter - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                            // 하기에 작업항목 구현
                            Debug.Log($"Trigger Enter - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                            // EnterBuilding tag가 붙은 컴포넌트의 경우에만 호출
                            bool isEnterBuilding = entityManager.HasComponent<EnterBuildingTag>(entity);
                            if (isEnterBuilding)
                            {
                                StateManager manager = entityManager.GetComponentData<StateManager>(stateManager);
                                manager.isEnterBuilding = !manager.isEnterBuilding;
                                entityManager.SetComponentData(stateManager, manager);
                            }
                        }
                        break;
                    case StatefulEventState.Stay:
                        // 충돌 중일 때 매 프레임 호출됩니다. 필요하다면 로그를 남기세요.
                        //Debug.Log($"Trigger Stay - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                        break;
                    case StatefulEventState.Exit:
                        // 하기에 작업항목 구현
                        if (ContainsEntity(in entityArray, entity))
                        {
                            RemoveEntityFromArray(ref entityArray, entity);
                            Debug.Log($"Trigger Exit - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                            Debug.Log("아웃풋");
                        }
                        break;
                    case StatefulEventState.Undefined:
                        Debug.LogWarning($"Trigger Event in Undefined state - Entity: {entity.Index}:{entity.Version}, Other Entity: {otherEntity.Index}:{otherEntity.Version}");
                        break;
                }

                if (triggerEvent.TryGetDetails(out var details) && details.IsValid)
                {
                    // 디테일 정보가 필요하다면 여기서 접근할 수 있습니다.
                    Debug.Log($"  - Number of Contact Points: {details.NumberOfContactPoints}");
                    Debug.Log($"  - Estimated Impulse: {details.EstimatedImpulse}");
                    Debug.Log($"  - Average Contact Point Position: {details.AverageContactPointPosition}");
                }
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
        Entity trigger = SystemAPI.GetSingletonEntity<TriggerBufferElement>();
        if(state.EntityManager.Exists(trigger))
        {
            state.EntityManager.DestroyEntity(trigger);
            Debug.Log("잘 파괴됨");
        }
    }

    private bool ContainsEntity(in DynamicBuffer<TriggerBufferElement> buffer, Entity entityToFind)
    {
        foreach (TriggerBufferElement element in buffer)
        {
            if (element.Value == entityToFind)
            {
                return true;
            }
        }
        return false;
    }

        // DynamicBuffer는 값 타입이므로, ref로 전달하여 직접 수정합니다.
    private void RemoveEntityFromArray(ref DynamicBuffer<TriggerBufferElement> buffer, Entity entityToRemove)
    {
        for (int i = buffer.Length - 1; i >= 0; i--)
        {
            if (buffer[i].Value == entityToRemove)
            {
                buffer.RemoveAt(i);
                return; // 하나만 삭제한다고 가정
            }
        }
    }
}

// Extension 메서드는 그대로 사용할 수 있습니다.
public static class StatefulTriggerEventExtensions
{
    public static Entity GetOtherEntity(this StatefulTriggerEvent collisionEvent, Entity entity)
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

