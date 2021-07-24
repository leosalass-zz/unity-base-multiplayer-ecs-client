using LiteNetLib;
using Unity.Mathematics;

public class PlayerCharacterEntityMessage : NetMessage
{
    public float3 Position { set; get; }

    public PlayerCharacterEntityMessage(float3 position)
    {
        Code = MessageCode.SPAWN_PLAYER_CHARACTER_ENTITY;
        Position = position;
    }
}
