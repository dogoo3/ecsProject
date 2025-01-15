using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;

// SystemBase에서는 BurstCompile을 사용할 수 없으므로 최적화가 덜 되지만, 여기서는 입력시스템만을 다룸.
// ISystem은 InputSystem을 사용할 수 없음.
public partial class InputSystem : SystemBase
{
    private SphereControls _sphereControls;

    protected override void OnCreate()
    {
        if(!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent input))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        _sphereControls = new SphereControls();
        _sphereControls.Enable();
    }
    protected override void OnUpdate()
    {
        Vector3 moveVector = _sphereControls.ActionMap.Movement.ReadValue<Vector3>();
        Vector2 mouseDelta = _sphereControls.ActionMap.MouseDelta.ReadValue<Vector2>();

        SystemAPI.SetSingleton(new InputComponent {movement = moveVector, mouseDelta = mouseDelta});
    }
}

public struct InputComponent : IComponentData
{
    public float3 movement;
    public float2 mouseDelta;
}
