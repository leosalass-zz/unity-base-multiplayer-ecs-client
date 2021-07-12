using LiteNetLib.Utils;
using Unity.Entities;

public class LNL_InputStateSerializer
{
    public static void Write(ref NetDataWriter writer, Net_InputStateMessage message)
    {
        DynamicBuffer<InputState> inputStateBuffer = message.Buffer;

        if (inputStateBuffer.Length > 0)
        {
            writer.Reset();
            writer.Put((int)message.Code);
            writer.Put(inputStateBuffer.Length);

            foreach (InputState inputState in inputStateBuffer)
            {
                writer.Put(inputState.tick);
                writer.Put(inputState.status);
                writer.Put(inputState.moveDirection.x);
                writer.Put(inputState.moveDirection.y);
            }
        }
    }

    public static void Deserialize(ref NetDataReader reader) { }
}
