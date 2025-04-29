using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct CubeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CubeComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new RotateAndScaleJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            elapsedTime = (float)SystemAPI.Time.ElapsedTime
        };
        job.Schedule();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}


[BurstCompile]
partial struct RotateAndScaleJob : IJobEntity
{
    public float deltaTime;
    public float elapsedTime;

    // 소스 생성에서 쿼리는 Execute()의 매개변수로부터 생성됩니다.
    // 여기서 쿼리는 LocalTransform, PostTransformMatrix 및 RotationSpeed 구성 요소를 가진 모든 엔티티와 일치합니다.
    // (Scene에서 루트 큐브는 비균일한 스케일을 가지므로 베이킹 시 PostTransformMatrix 성분이 부여됩니다.)
    void Execute(ref LocalTransform transform, in CubeComponent cube)
    {
        transform = transform.RotateY(cube.angle * deltaTime);
        // postTransform.Value = float4x4.Scale(1, math.sin(elapsedTime), 1);
    }
}