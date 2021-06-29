using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WorldUpdateComponent : IComponentData
{
    public int tick;
    public float timestep;
    public int predicionDelay;
    public int shadowDelay;
}
