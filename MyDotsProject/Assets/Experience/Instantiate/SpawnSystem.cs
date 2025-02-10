using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System.Diagnostics;

namespace Instantiate
{
    public partial struct SpawnSystem : ISystem
    {
        float elapsedTime;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnPoint>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (elapsedTime <= 0)
            {
                // Delete BulletTag Component
                EntityQuery spawnQuery = SystemAPI.QueryBuilder().WithAll<BulletTag>().Build();
                state.EntityManager.RemoveComponent<BulletTag>(spawnQuery);

                // Instantiate Bullet
                Entity prefab = SystemAPI.GetSingleton<SpawnPoint>().Prefab;
                state.EntityManager.Instantiate(prefab);

                new PositionJob
                {
                    randomSeed = Random.CreateFromIndex(state.GlobalSystemVersion).NextInt2()
                }.ScheduleParallel();

                elapsedTime = 0.5f;
            }
            elapsedTime -= SystemAPI.Time.DeltaTime;
        }
    }

    [WithAll(typeof(BulletTag))]
    [BurstCompile]
    partial struct PositionJob : IJobEntity
    {
        public int2 randomSeed;
        public void Execute(ref LocalTransform transform)
        {
            transform.Position = new float3(randomSeed.x % 6 * 2, 2.0f, randomSeed.y % 6 * 2);
        }
    }
}