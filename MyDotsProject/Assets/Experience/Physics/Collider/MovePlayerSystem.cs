using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BUSH.Physics.Collider
{
    public partial struct SphereSystem : ISystem
    {
        private Entity _inputEntity;
        private InputComponent _inputComponent;
        private EntityManager entityManager;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SphereComponent>();

        }

        public void OnUpdate(ref SystemState state)
        {
            entityManager = state.EntityManager;
            
            _inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
            _inputComponent = entityManager.GetComponentData<InputComponent>(_inputEntity);

            foreach(var (component, characterPos) in SystemAPI.Query<RefRO<SphereComponent>, RefRW<LocalTransform>>())
            {
                if(component.ValueRO.playerName == "A") // A Sphere의 이동을 관리
                {
                    characterPos.ValueRW.Position += new float3(new float3(0,0,1) * _inputComponent.player1Input.y * component.ValueRO.moveSpeed * Time.deltaTime);
                    characterPos.ValueRW.Position += new float3(new float3(1,0,0) * _inputComponent.player1Input.x * component.ValueRO.moveSpeed * Time.deltaTime);
                }
                else // B Sphere의 이동을 관리
                {
                    characterPos.ValueRW.Position += new float3(new float3(0,0,1) * _inputComponent.player2Input.y * component.ValueRO.moveSpeed * Time.deltaTime);
                    characterPos.ValueRW.Position += new float3(new float3(1,0,0) * _inputComponent.player2Input.x * component.ValueRO.moveSpeed * Time.deltaTime);
                }
            }
        }
    }
}