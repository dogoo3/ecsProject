namespace ExperienceTest.GetComponent
{
    using Unity.Entities;
    using UnityEngine;

    public class CubeAuthoring : MonoBehaviour
    {
        public GameObject sphereObj, cylinderObj;

        public class CubeBaker : Baker<CubeAuthoring>
        {
            public override void Bake(CubeAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                if(authoring.sphereObj != null && authoring.cylinderObj)
                {
                    AddComponent(entity, new CubeComponent{
                        sphereEntity = GetEntity(authoring.sphereObj, TransformUsageFlags.Dynamic),
                        cylinderEntity = GetEntity(authoring.cylinderObj, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }

    public struct CubeComponent : IComponentData
    {
        public Entity sphereEntity, cylinderEntity;
    }
}