using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// code generator가 만든 코드와 사용자가 만드는 코드를 분리하기 위해 partial을 사용한다.
// 컴파일러는 나중에 하나의 system으로 만든다?
public partial struct SphereSystem : ISystem
{
    private Entity _sphereEntity;
    private Entity _inputEntity;
    private EntityManager entityManager;
    private SphereComponent _sphereComponent;
    private InputComponent _inputComponent;

    private float2 rotateDeltaSum;
    private float sensitivity;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SphereComponent>();
        sensitivity = 0.1f;
    }
    
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;
        _sphereEntity = SystemAPI.GetSingletonEntity<SphereComponent>();
        _inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        _sphereComponent = entityManager.GetComponentData<SphereComponent>(_sphereEntity);
        _inputComponent = entityManager.GetComponentData<InputComponent>(_inputEntity);

        Move(ref state);

        CameraRotate(ref state);
    }

    // 참조전달, 매개변수의 전달 원본이 변경됨
    private void Move(ref SystemState state)
    {
        LocalTransform sphereTransform = entityManager.GetComponentData<LocalTransform>(_sphereEntity);

        sphereTransform.Position += new float3(_inputComponent.movement * _sphereComponent.moveSpeed * Time.deltaTime);

        entityManager.SetComponentData(_sphereEntity, sphereTransform);
    }

    private void CameraRotate(ref SystemState state)
    {
        Transform cameraTransform = Camera.main.transform;

        LocalTransform sphereTransform = entityManager.GetComponentData<LocalTransform>(_sphereEntity);

        // Debug.Log(sphereTransform);

        rotateDeltaSum += _inputComponent.mouseDelta;
        // sphereTransform.Rotation = quaternion.RotateY(math.radians(rotateDeltaSum.x));

        // 마우스 상하좌우 이동에 따른 머리 오브젝트 회전
        foreach(var localTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CharacterHeadComponent>()) 
        {
            localTransform.ValueRW.Rotation = math.mul(quaternion.RotateY(math.radians(rotateDeltaSum.x * sensitivity)), quaternion.RotateX(math.radians(rotateDeltaSum.y * sensitivity)));
            cameraTransform.rotation = Quaternion.Euler(rotateDeltaSum.y * sensitivity, rotateDeltaSum.x * sensitivity, 0);
            break;
        }

        // WithEntityAccess() : 매개변수가 있는 튜플을 검색
        // 눈이 위치한 오브젝트로의 카메라 위치 이동
        foreach(var (eyeEntity, worldtransform) in SystemAPI.Query<EyePositionComponent, LocalToWorld>())
        {            
            LocalToWorld temp = entityManager.GetComponentData<LocalToWorld>(eyeEntity.eyeEntity);
            cameraTransform.position = temp.Position;
            break; 
        }
    }
}
