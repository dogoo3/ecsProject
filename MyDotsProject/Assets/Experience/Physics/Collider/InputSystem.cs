using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BUSH.Physics.Collider
{
    public partial class InputSystem : SystemBase
    {
        private PhysicsObjectInputSystem physicsObjectInputSystem;

        protected override void OnCreate()
        {
            if(!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent input))
            {
                EntityManager.CreateEntity(typeof(InputComponent));
            }

            physicsObjectInputSystem = new PhysicsObjectInputSystem();
            physicsObjectInputSystem.Enable();
        }

        protected override void OnUpdate()
        {
            Vector2 player1Input = physicsObjectInputSystem.Player1.Move.ReadValue<Vector2>();
            Vector2 player2Input = physicsObjectInputSystem.Player2.Move.ReadValue<Vector2>();
            
            SystemAPI.SetSingleton(new InputComponent {player1Input = player1Input, player2Input = player2Input});
        }
    }

    public struct InputComponent : IComponentData
    {
        public float2 player1Input, player2Input;
    }

}