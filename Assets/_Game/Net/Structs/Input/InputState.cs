using Unity.Mathematics;

public struct InputState
{
    public InputState(int tick, int status, float2 moveDirection)
    {
        this.tick = tick;
        this.status = status;
        this.moveDirection = moveDirection;
    }

    public int tick;
    public int status;
    public float2 moveDirection;
}
