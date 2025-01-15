namespace ExperienceTest.GetComponent
{
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    partial struct CubeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CubeComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (cubeComponent, entity) in SystemAPI.Query<CubeComponent>().WithEntityAccess())
            {
                // Sphere와 Cylinder의 LocalTransform 가져오기
                if (SystemAPI.HasComponent<LocalTransform>(cubeComponent.sphereEntity))
                {
                    LocalTransform sphereTransform = SystemAPI.GetComponent<LocalTransform>(cubeComponent.sphereEntity);
                    sphereTransform.Position = new Unity.Mathematics.float3(10,10,10);
                    SystemAPI.SetComponent(cubeComponent.sphereEntity, sphereTransform);
                    Debug.Log($"Sphere Position: {sphereTransform.Position}");
                }

                if (SystemAPI.HasComponent<LocalTransform>(cubeComponent.cylinderEntity))
                {
                    var cylinderTransform = SystemAPI.GetComponent<LocalTransform>(cubeComponent.cylinderEntity);
                    Debug.Log($"Cylinder Position: {cylinderTransform.Position}");
                }

                Debug.Log(SystemAPI.GetComponent<LocalToWorld>(entity).Position);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}

