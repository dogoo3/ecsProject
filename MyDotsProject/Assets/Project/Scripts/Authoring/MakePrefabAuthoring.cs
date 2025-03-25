using Unity.Entities;
using UnityEngine;

public class MakePrefabAuthoring : MonoBehaviour
{
    public GameObject _prefab;

    private class Baker : Baker<MakePrefabAuthoring>
    {
        public override void Bake(MakePrefabAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            // 'AddComponentObject' 를 사용합니다. entity 레퍼런스도 받지 않습니다.
            AddComponentObject(entity, new MakePrefabData()
            {
                prefab = authoring._prefab
            });
        }
    }
}

public class MakePrefabData : IComponentData
{
    public GameObject prefab;
}
