using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct MakeReflectionProbeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MakeReflectionProbeComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var obj in SystemAPI.Query<MakeReflectionProbeComponent>())
        {
            Debug.Log("zxcv");
            for(int i=0;i<obj.positions.Length;i++)
            {
                Debug.Log("asjdklajsdkl");
                GameObject myGameObject = GameObject.Instantiate(obj._rp, obj.positions[i].transform.position, quaternion.identity);
            }
        }

        state.Enabled = false;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
