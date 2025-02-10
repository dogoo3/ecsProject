using Unity.Entities;
using UnityEngine;

public class FollowObjectAuthoring : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public class Baker : Baker<FollowObjectAuthoring>
    {
        public override void Bake(FollowObjectAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FollowObject{
                moveSpeed = authoring.moveSpeed
            });
            AddComponent<TempTag>(entity);
        }
    }
}

public struct FollowObject : IComponentData
{
    public float moveSpeed;
}

public struct TempTag : IComponentData
{

}
