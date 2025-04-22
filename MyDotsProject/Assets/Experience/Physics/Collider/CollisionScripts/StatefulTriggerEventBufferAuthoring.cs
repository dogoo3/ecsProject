using Unity.Assertions;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

namespace BUSH.Physics.Collider
{
    public class StatefulTriggerEventBufferAuthoring : MonoBehaviour
    {
        class Baker : Baker<StatefulTriggerEventBufferAuthoring>
        {
            public override void Bake(StatefulTriggerEventBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<StatefulTriggerEvent>(entity);
            }
        }
    }

    // Trigger Event that can be stored inside a DynamicBuffer
    public struct StatefulTriggerEvent : IBufferElementData, IStatefulSimulationEvent<StatefulTriggerEvent>
    {
        public Entity EntityA { get; set; }
        public Entity EntityB { get; set; }
        public int BodyIndexA { get; set; }
        public int BodyIndexB { get; set; }
        public ColliderKey ColliderKeyA { get; set; }
        public ColliderKey ColliderKeyB { get; set; }
        public StatefulEventState State { get; set; }
        internal Details TriggerDetails;

        public StatefulTriggerEvent(TriggerEvent triggerEvent)
        {
            EntityA = triggerEvent.EntityA;
            EntityB = triggerEvent.EntityB;
            BodyIndexA = triggerEvent.BodyIndexA;
            BodyIndexB = triggerEvent.BodyIndexB;
            ColliderKeyA = triggerEvent.ColliderKeyA;
            ColliderKeyB = triggerEvent.ColliderKeyB;
            State = default;
            TriggerDetails = default;
        }

        public struct Details
        {
            internal bool IsValid;

            // If 1, then it is a vertex collision
            // If 2, then it is an edge collision
            // If 3 or more, then it is a face collision
            public int NumberOfContactPoints;

            // Estimated impulse applied
            public float EstimatedImpulse;
            // Average contact point position
            public float3 AverageContactPointPosition;

            public Details(int numContactPoints, float estimatedImpulse, float3 averageContactPosition)
            {
                IsValid = (0 < numContactPoints); // Should we add a max check?
                NumberOfContactPoints = numContactPoints;
                EstimatedImpulse = estimatedImpulse;
                AverageContactPointPosition = averageContactPosition;
            }
        }

        // Returns other entity in EntityPair, if provided with one
        public Entity GetOtherEntity(Entity entity)
        {
            Assert.IsTrue((entity == EntityA) || (entity == EntityB));
            return (entity == EntityA) ? EntityB : EntityA;
        }
        

        public bool TryGetDetails(out Details details)
        {
            details = TriggerDetails;
            return TriggerDetails.IsValid;
        }

        public int CompareTo(StatefulTriggerEvent other) => ISimulationEventUtilities.CompareEvents(this, other);
    }

    // If this component is added to an entity, trigger events won't be added to a dynamic buffer
    // of that entity by the StatefulTriggerEventBufferSystem. This component is by default added to
    // CharacterController entity, so that CharacterControllerSystem can add trigger events to
    // CharacterController on its own, without StatefulTriggerEventBufferSystem interference.
    public struct StatefulTriggerEventExclude : IComponentData {}
}
