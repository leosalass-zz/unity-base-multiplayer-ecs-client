using Unity.Entities;

public class Net_InputStateMessage : NetMessage
{
    public DynamicBuffer<InputState> Buffer { set; get; }

    public Net_InputStateMessage()
    {
        Code = MessageCode.INPUT_STATE;
    }

    public override void Send(DynamicBuffer<InputState> buffer)
    {
        Buffer = buffer;
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<LNL_ClientSystem>().SendInputStatesToServer(this);
    }
}
