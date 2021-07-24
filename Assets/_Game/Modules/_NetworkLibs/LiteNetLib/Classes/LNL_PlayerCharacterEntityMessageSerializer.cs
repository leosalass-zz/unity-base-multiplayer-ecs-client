using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Mathematics;

public static class LNL_PlayerCharacterEntityMessageSerializer
{
    public static void Write(ref NetDataWriter writer, PlayerCharacterEntityMessage playerCharacterEntityMessage) {
        writer.Reset();
        writer.Put((int)playerCharacterEntityMessage.Code);
        writer.Put(playerCharacterEntityMessage.Position.x);
        writer.Put(playerCharacterEntityMessage.Position.y);
        writer.Put(playerCharacterEntityMessage.Position.z);
    }

    public static PlayerCharacterEntityMessage Read(ref NetPacketReader reader) {
        float x = (float)reader.GetFloat();
        float y = (float)reader.GetFloat();
        float z = (float)reader.GetFloat();
        float3 position = new float3(x, y, z);

        PlayerCharacterEntityMessage playerCharacterEntityMessage = new PlayerCharacterEntityMessage(position);

        return playerCharacterEntityMessage;
    }
}
