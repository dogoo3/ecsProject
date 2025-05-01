using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics;

public class StatefulCollisionEventBufferAuthoring : MonoBehaviour
{
    [Tooltip("If selected, the details will be calculated in collision event dynamic buffer of this entity")]
    public bool CalculateDetails = false;

    class StatefulCollisionEventBufferBaker : Baker<StatefulCollisionEventBufferAuthoring>
    {
        public override void Bake(StatefulCollisionEventBufferAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            if (authoring.CalculateDetails)
            {
                var dynamicBufferTag = new StatefulCollisionEventDetails
                {
                    CalculateDetails = authoring.CalculateDetails
                };

                AddComponent(entity, dynamicBufferTag);
            }

            AddBuffer<StatefulCollisionEvent>(entity);
        }
    }
}

public struct StatefulCollisionEventDetails : IComponentData
{
    public bool CalculateDetails;
}

// Collision Event that can be stored inside a DynamicBuffer
// 다이내믹 버퍼에 저장할 수 있는 충돌 이벤트

// IComponentData와는 달리 엔티티당 여러 개의 요소를 가질 수 있는 버퍼를 정의한다. 예를 들어, 캐릭터의 인벤토리, Mesh의 Vertex 데이터, GameObject의 자식 LocalTransform 등을 저장하는 데 쓰임.
public struct StatefulCollisionEvent : IBufferElementData, IStatefulSimulationEvent<StatefulCollisionEvent>
{
    public Entity EntityA { get; set; }
    public Entity EntityB { get; set; }
    public int BodyIndexA { get; set; }
    public int BodyIndexB { get; set; }
    public ColliderKey ColliderKeyA { get; set; }
    public ColliderKey ColliderKeyB { get; set; }
    public StatefulEventState State { get; set; }
    // public float3 Normal;

    // Only if CalculateDetails is checked on PhysicsCollisionEventBuffer of selected entity,
    // this field will have valid value, otherwise it will be zero initialized
    // 선택된 엔티티의 PhysicsCollisionEventBuffer에서 CalculateDetails가 확인된 경우에만,
    // 이 필드는 유효한 값을 가지며, 그렇지 않으면 0으로 초기화됩니다
    // internal Details CollisionDetails;

    public StatefulCollisionEvent(CollisionEvent collisionEvent)
    {
        EntityA = collisionEvent.EntityA;
        EntityB = collisionEvent.EntityB;
        BodyIndexA = collisionEvent.BodyIndexA;
        BodyIndexB = collisionEvent.BodyIndexB;
        ColliderKeyA = collisionEvent.ColliderKeyA;
        ColliderKeyB = collisionEvent.ColliderKeyB;
        State = default;
        // Normal = collisionEvent.Normal;
        // CollisionDetails = default;
    }

    // This struct describes additional, optional, details about collision of 2 bodies
    // public struct Details
    // {
    //     internal bool IsValid;

    //     // If 1, then it is a vertex collision
    //     // If 2, then it is an edge collision
    //     // If 3 or more, then it is a face collision
    //     public int NumberOfContactPoints;

    //     // Estimated impulse applied
    //     public float EstimatedImpulse;
    //     // Average contact point position
    //     public float3 AverageContactPointPosition;

    //     public Details(int numContactPoints, float estimatedImpulse, float3 averageContactPosition)
    //     {
    //         IsValid = (0 < numContactPoints); // Should we add a max check?
    //         NumberOfContactPoints = numContactPoints;
    //         EstimatedImpulse = estimatedImpulse;
    //         AverageContactPointPosition = averageContactPosition;
    //     }
    // }

    // Returns the other entity in EntityPair, if provided with other one
    public Entity GetOtherEntity(Entity entity)
    {
        Assert.IsTrue((entity == EntityA) || (entity == EntityB));
        return entity == EntityA ? EntityB : EntityA;
    }

    // Returns the normal pointing from passed entity to the other one in pair
    // public float3 GetNormalFrom(Entity entity)
    // {
    //     Assert.IsTrue((entity == EntityA) || (entity == EntityB));
    //     return math.select(-Normal, Normal, entity == EntityB);
    // }

    // public bool TryGetDetails(out Details details)
    // {
    //     details = CollisionDetails;
    //     return CollisionDetails.IsValid;
    // }

    public int CompareTo(StatefulCollisionEvent other) => ISimulationEventUtilities.CompareEvents(this, other);
}

