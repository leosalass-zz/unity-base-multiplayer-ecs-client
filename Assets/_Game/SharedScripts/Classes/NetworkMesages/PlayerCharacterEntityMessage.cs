using LiteNetLib;
using Unity.Mathematics;

public class PlayerCharacterEntityMessage : NetMessage
{
    public float3 Position { set; get; }

    public PlayerCharacterEntityMessage(float3 position)
    {
        Code = MessageCode.CREATE_PLAYER_CHARACTER;
        Position = position;
    }
}
