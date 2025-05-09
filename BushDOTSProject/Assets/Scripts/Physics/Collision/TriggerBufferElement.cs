using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct TriggerBufferElement : IBufferElementData
{
    public Entity Value;

    // 암시적으로 TriggerBufferElement 타입의 값이 들어오던, Entity 타입의 값이 들어오던, 모두 Entity 타입으로 변경하여 저장한다
    public static implicit operator Entity(TriggerBufferElement e) { return e.Value; }
    public static implicit operator TriggerBufferElement(Entity e) { return new TriggerBufferElement { Value = e }; }
}