using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FollowObjectInstantiateSystem : ISystem
{
    float elapsedTime;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FollowObjectSpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (elapsedTime <= 0)
        {
            // Delete BulletTag Component
            EntityQuery spawnQuery = SystemAPI.QueryBuilder().WithAll<TempTag>().Build();
            state.EntityManager.RemoveComponent<TempTag>(spawnQuery);

            // Instantiate Bullet
            Entity prefab = SystemAPI.GetSingleton<FollowObjectSpawner>().prefab;
            state.EntityManager.Instantiate(prefab);

            new PositionJob
            {
                randomSeed = Random.CreateFromIndex(state.GlobalSystemVersion).NextInt2()
            }.ScheduleParallel();

            elapsedTime = 2f;
        }
        elapsedTime -= SystemAPI.Time.DeltaTime;
    }
}

[WithAll(typeof(TempTag))]
[BurstCompile]
partial struct PositionJob : IJobEntity
{
    public int2 randomSeed;
    public void Execute(ref LocalTransform transform)
    {
        transform.Position = new float3(randomSeed.x % 6 * 2, 3.0f, randomSeed.y % 6 * 2);
    }
}

