using Unity.Entities;
using UnityEngine;

public class TagAuthoring : MonoBehaviour
{
    public enum TagList {EnterBuilding, tag2, tag3};

    public TagList tagList;

    class Baker : Baker<TagAuthoring>
    {
        public override void Bake(TagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            switch(authoring.tagList)
            {
                case TagList.EnterBuilding:
                    AddComponent(entity, new EnterBuildingTag{ });
                    break;
            }
        }
    }
}

public struct EnterBuildingTag : IComponentData {}