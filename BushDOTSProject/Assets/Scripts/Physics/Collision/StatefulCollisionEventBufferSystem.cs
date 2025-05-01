using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Physics;


// This system converts stream of CollisionEvents to StatefulCollisionEvents that can be stored in a Dynamic Buffer.
// In order for this conversion, it is required to:
//    1) Use the 'Collide Raise Collision Events' option of the 'Collision Response' property on a PhysicsShapeAuthoring component, and
//    2) Add a StatefulCollisionEventBufferAuthoring component to that entity (and select if details should be calculated or not)
// or, if this is desired on a Character Controller:
//    1) Tick the 'Raise Collision Events' flag on the CharacterControllerAuthoring component.

// 이 시스템은 CollisionEvents 스트림을 동적 버퍼에 저장할 수 있는 StatefulCollisionEvents로 변환합니다.
// 이 변환을 위해서는 다음이 필요합니다:
// 1) PhysicsShapeAuthoring 구성 요소에서 'Collision Response' 속성의 'Collide Raise Collision Events' 옵션을 사용합니다
// 2) 해당 엔티티에 StatefulCollisionEventBufferAuthorizing 구성 요소를 추가합니다(자세한 내용을 계산해야 하는지 여부를 선택합니다)
// 또는 Character Controller에서 원하는 경우:
// 1) 캐릭터 컨트롤러 저작 구성 요소에서 'Raise Collision Events' 플래그를 체크합니다.

// 특정 시스템을 지정된 ComponentSystemGroup에 포함시키는 역할을 합니다.
[UpdateInGroup(typeof(PhysicsSystemGroup))]
// 특정 시스템이 다른 시스템 다음에 실행되도록 명시적인 의존성을 설정하는 역할을 합니다.
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct StatefulCollisionEventBufferSystem : ISystem
{
    private StatefulSimulationEventBuffers<StatefulCollisionEvent> m_StateFulEventBuffers;
    private ComponentHandles m_Handles;

    // Component that does nothing. Made in order to use a generic job. See OnUpdate() method for details.
    internal struct DummyExcludeComponent : IComponentData { };

    struct ComponentHandles
    {
        public ComponentLookup<DummyExcludeComponent> EventExcludes;
        public ComponentLookup<StatefulCollisionEventDetails> EventDetails;
        public BufferLookup<StatefulCollisionEvent> EventBuffers;

        public ComponentHandles(ref SystemState systemState)
        {
            EventExcludes = systemState.GetComponentLookup<DummyExcludeComponent>(true);
            EventDetails = systemState.GetComponentLookup<StatefulCollisionEventDetails>(true);
            EventBuffers = systemState.GetBufferLookup<StatefulCollisionEvent>(false);
        }

        public void Update(ref SystemState systemState)
        {
            EventExcludes.Update(ref systemState);
            EventBuffers.Update(ref systemState);
            EventDetails.Update(ref systemState);
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_StateFulEventBuffers = new StatefulSimulationEventBuffers<StatefulCollisionEvent>();
        m_StateFulEventBuffers.AllocateBuffers();
        state.RequireForUpdate<StatefulCollisionEvent>();

        m_Handles = new ComponentHandles(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        m_StateFulEventBuffers.Dispose();
    }

    [BurstCompile]
    public partial struct ClearCollisionEventDynamicBufferJob : IJobEntity
    {
        public void Execute(ref DynamicBuffer<StatefulCollisionEvent> eventBuffer) => eventBuffer.Clear();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_Handles.Update(ref state);

        state.Dependency = new ClearCollisionEventDynamicBufferJob()
            .ScheduleParallel(state.Dependency);

        m_StateFulEventBuffers.SwapBuffers();

        var currentEvents = m_StateFulEventBuffers.Current;
        var previousEvents = m_StateFulEventBuffers.Previous;

        state.Dependency = new StatefulEventCollectionJobs.
            CollectCollisionEventsWithDetails
        {
            CollisionEvents = currentEvents,
            PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
            EventDetails = m_Handles.EventDetails
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);


        state.Dependency = new StatefulEventCollectionJobs.
            ConvertEventStreamToDynamicBufferJob<StatefulCollisionEvent, DummyExcludeComponent>
        {
            CurrentEvents = currentEvents,
            PreviousEvents = previousEvents,
            EventLookup = m_Handles.EventBuffers,

            UseExcludeComponent = false,
            EventExcludeLookup = m_Handles.EventExcludes
        }.Schedule(state.Dependency);
    }
}

