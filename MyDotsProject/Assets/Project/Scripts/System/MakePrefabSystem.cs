using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct MakePrefabSystem : ISystem
{
    private int count;
    private float elapsedTime;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MakePrefabData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (elapsedTime > 5.0f)
        {
            foreach (var obj in SystemAPI.Query<MakePrefabData>())
            {
                GameObject myGameObject = GameObject.Instantiate(obj.prefab, Vector3.one * count, quaternion.identity);
                // foreach(var (obj, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<FollowObject>>())
                // {
                //     // 오브젝트가 캐릭터를 바라보는 방향벡터 계산
                //     float3 arrowVector = math.normalize(character.ValueRO.Position - obj.ValueRO.Position);
                //     obj.ValueRW.Position += arrowVector * speed.ValueRO.moveSpeed * Time.deltaTime;
                // }
            }
            elapsedTime = 0;
            count++;
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
