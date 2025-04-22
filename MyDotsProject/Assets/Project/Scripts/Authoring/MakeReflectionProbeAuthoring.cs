using Unity.Entities;
using UnityEngine;

public class MakeReflectionProbeAuthoring : MonoBehaviour {
    public GameObject _rp;
    public GameObject[] positions;

    private class Baker : Baker<MakeReflectionProbeAuthoring>
    {
        public override void Bake(MakeReflectionProbeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponentObject(entity, new MakeReflectionProbeComponent()
            {
                _rp = authoring._rp,
                positions = authoring.positions
            });
        }
    }
}

public class MakeReflectionProbeComponent : IComponentData
{
    public GameObject _rp;
    public GameObject[] positions;
}