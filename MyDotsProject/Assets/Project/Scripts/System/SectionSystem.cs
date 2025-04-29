using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;

partial struct SectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Circle>();
        state.RequireForUpdate<StateManager>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // EntityManager 생성
        EntityManager entityManager = state.EntityManager;

        NativeHashSet<Entity> toLoad = new NativeHashSet<Entity>(1, Allocator.Temp);

        var sectionQuery = SystemAPI.QueryBuilder().WithAll<Circle, SceneSectionData>().Build();
        var sectionEntities = sectionQuery.ToEntityArray(Allocator.Temp);
        var circles = sectionQuery.ToComponentDataArray<Circle>(Allocator.Temp);

        // 게임 내에서의 상태들을 관리하고, 상태에 따라 로직을 변경하기 위해서 StateManager를 불러온다.
        Entity stateManager = SystemAPI.GetSingletonEntity<StateManager>();
        StateManager manager = entityManager.GetComponentData<StateManager>(stateManager);

        // Find all the sections that should be loaded based on the distances to the sphere
        foreach (var transform in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<SphereComponent>())
        {
            for (int index = 0; index < circles.Length; ++index)
            {
                float3 distance = transform.ValueRO.Position - circles[index].Center;
                distance.y = 0;
                float radiusSq = circles[index].Radius;
                Color debugColor = new Color(1f, 0f, 0f);
                if(manager.isEnterBuilding) // 섹션 설정 기능이 OFF일 경우
                {
                    if (math.abs(circles[index].Center.y - transform.ValueRO.Position.y) < 2.0f) // (Section의 중심점의 y좌표 - 캐릭터의 y좌표)가 제한 높이보다 작을 경우에만 section이 활성화되도록 설정
                    {
                        if (math.lengthsq(distance) < radiusSq * radiusSq) // 일정 거리만큼 떨어져 있을 경우
                        {
                            toLoad.Add(sectionEntities[index]); // 섹션 활성화
                            debugColor = new Color(0f, 0.5f, 0f);
                        }
                    }

                    DrawCircleXZ(circles[index].Center + new float3(0f, 0.2f, 0f),
                        circles[index].Radius, debugColor);
                }
                else // 섹션 설정 기능이 OFF일 경우
                {
                    // 모든 섹션 활성화
                    toLoad.Add(sectionEntities[index]); // 섹션 활성화
                }
            }
        }

        foreach (Entity sectionEntity in sectionEntities)
        {
            var sectionState = SceneSystem.GetSectionStreamingState(state.WorldUnmanaged, sectionEntity);
            if (toLoad.Contains(sectionEntity))
            {
                if (sectionState == SceneSystem.SectionStreamingState.Unloaded)
                {
                    // Load the section
                    state.EntityManager.AddComponent<RequestSceneLoaded>(sectionEntity);
                }
            }
            else
            {
                if (sectionState == SceneSystem.SectionStreamingState.Loaded)
                {
                    // Unload the section
                    state.EntityManager.RemoveComponent<RequestSceneLoaded>(sectionEntity);
                }
            }
        }
    }

    public static void DrawCircleXZ(float3 position, float radius, Color color, float divisions = 8f)
    {
        float angle = 0f;
        float step = math.PI / divisions;
        float PI2 = math.PI * 2f;
        while (angle < PI2)
        {
            float3 begin = new float3(math.sin(angle), 0f, math.cos(angle)) * radius + position;
            angle += step;
            float3 end = new float3(math.sin(angle), 0f, math.cos(angle)) * radius + position;
            Debug.DrawLine(begin, end, color);
        }
    }
}

