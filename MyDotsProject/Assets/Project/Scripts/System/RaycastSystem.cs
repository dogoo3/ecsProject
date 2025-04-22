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
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            foreach (var (player, tranform) in SystemAPI.Query<RefRO<RayShooterComponent>, RefRO<LocalToWorld>>())
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
                    if(entityManager.HasComponent<DoorComponent>(hit.Entity))
                    {
                        var door = entityManager.GetComponentData<DoorComponent>(hit.Entity);
                        var com = entityManager.GetComponentData<LocalTransform>(hit.Entity);
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
