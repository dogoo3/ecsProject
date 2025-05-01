using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct RaycastSystem : ISystem
{
    private float elapsedTime;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (elapsedTime > 0.5f)
        {
            // 레이캐스트를 사용하기 위해 PhysicsWorldSingleton이 필요하다.
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            // 게임 내에서의 상태들을 관리하고, 상태에 따라 로직을 변경하기 위해서 StateManager를 불러온다.
            Entity stateManager = SystemAPI.GetSingletonEntity<StateManager>();

            // EntityManager 생성
            EntityManager entityManager = state.EntityManager;

            foreach (var (player, tranform) in SystemAPI.Query<RefRO<CharacterRayComponent>, RefRO<LocalToWorld>>())
            {
                Debug.DrawLine(tranform.ValueRO.Position, tranform.ValueRO.Position + tranform.ValueRO.Forward * player.ValueRO.length, Color.blue, 0.5f);
                var ray = new RaycastInput()
                {
                    Start = tranform.ValueRO.Position,
                    End = tranform.ValueRO.Position + tranform.ValueRO.Forward * player.ValueRO.length,
                    Filter = new CollisionFilter
                    {
                        GroupIndex = 0,

                        // 1u << 0는 Physics Category Names에서 0번째의 레이어마스크이다.
                        // 1u << 1는 Physics Category Names에서 1번째의 레이어마스크이다.
                        BelongsTo = 1u,
                        CollidesWith = 1u
                    }
                };
                

                if(physicsWorld.CastRay(ray, out var hit))
                {
                    if(entityManager.HasComponent<DoorComponent>(hit.Entity)) // 문 오브젝트와 충돌했을 때
                    {
                        var door = entityManager.GetComponentData<DoorComponent>(hit.Entity);
                        var com = entityManager.GetComponentData<LocalTransform>(hit.Entity);

                        // Entity의 값 수정하는 방법
                        // 충돌하게 되면 빌딩에 들어간 것으로 간주한다
                        // StateManager manager = entityManager.GetComponentData<StateManager>(stateManager);
                        // manager.isEnterBuilding = !manager.isEnterBuilding;
                        // entityManager.SetComponentData(stateManager, manager);

                        // Door의 속성 변경하기
                        com = com.WithRotation(quaternion.Euler(0, math.radians(door.openAngle), 0));
                        entityManager.SetComponentData(hit.Entity, com);
                    }
                }
            }
            elapsedTime = 0;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
