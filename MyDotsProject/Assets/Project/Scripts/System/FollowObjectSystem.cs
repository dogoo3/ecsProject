using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct FollowObjectSystem : ISystem
{
    private float3 _characterPosition;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FollowObject>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach(var character in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<SphereComponent>())
        {
            foreach(var (obj, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<FollowObject>>())
            {
                // 오브젝트가 캐릭터를 바라보는 방향벡터 계산
                float3 arrowVector = math.normalize(character.ValueRO.Position - obj.ValueRO.Position);
                obj.ValueRW.Position += arrowVector * speed.ValueRO.moveSpeed * Time.deltaTime;

                Debug.Log(character.ValueRO.Position - obj.ValueRO.Position);
            }
        }
    }
}