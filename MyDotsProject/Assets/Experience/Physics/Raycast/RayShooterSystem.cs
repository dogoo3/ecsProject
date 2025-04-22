using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Physics.Raycast
{
    partial struct RayShooterSystem : ISystem
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
                    // Debug.Log(tranform.ValueRO.Position);
                    // Debug.Log(tranform.ValueRO.Position + tranform.ValueRO.Forward * 10.0f);
                    Debug.DrawLine(tranform.ValueRO.Position, tranform.ValueRO.Position + tranform.ValueRO.Forward * 10.0f, Color.blue, 2.5f);
                    var ray = new RaycastInput()
                    {
                        Start = tranform.ValueRO.Position,
                        End = tranform.ValueRO.Position + tranform.ValueRO.Forward * 10.0f,
                        Filter = new CollisionFilter
                        {
                            GroupIndex = 0,

                            // 1u << 0는 Physics Category Names에서 0번째의 레이어마스크이다.
                            // 1u << 1는 Physics Category Names에서 1번째의 레이어마스크이다.
                            BelongsTo = 1u << 0,
                            CollidesWith = 1u << 1
                        }

                    };

                    // 레이캐스트 발사 후 hit가 존재하면 if 실행
                    if (physicsWorld.CastRay(ray, out var hit))
                    {
                        Debug.Log($"{hit.Position}");
                        Debug.Log($"{hit.ToString()}");
                        Debug.Log($"{hit.Entity.Index}");

                        if(entityManager.HasComponent<RayHitter>(hit.Entity)) // 충돌된 entity에 컴포넌트가 있을 경우
                        {
                            var com = entityManager.GetComponentData<RayHitter>(hit.Entity);
                            Debug.Log(com.value);
                            Debug.Log(com.AddReturn());
                        }
                        else
                        {
                            Debug.Log("해당 컴포넌트 없음");
                        }
                    }
                }
                elapsedTime = 0;
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }

            // state.Enabled = false;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
