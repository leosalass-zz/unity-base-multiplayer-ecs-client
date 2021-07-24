using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class LNL_ChatMessageSerializer
{
    public static void Write(Net_ChatMessage message, NetPeer server, ref NetDataWriter writer)
    {
        if (message.Message.Length == 0) return;

        writer.Reset();
        writer.Put((int)message.Code);
        writer.Put(message.Message);
    }

    public static string Read(ref NetPacketReader reader)
    {
        string message = reader.GetString();
        return message;
    }
}
