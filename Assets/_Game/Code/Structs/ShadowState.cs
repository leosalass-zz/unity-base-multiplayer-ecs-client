using Unity.Mathematics;

public struct ShadowState
{
    public ShadowState(int tick, float3 position)
    {
        this.tick = tick;
        this.position = position;
    }

    public int tick;
    public float3 position;
}
