using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class SendServerInputsStatesSystem : SystemBase
{
    Net_InputStateMessage inputStateMessage;
    int lastPackSendTick;

    protected override void OnCreate()
    {
        inputStateMessage = new Net_InputStateMessage();
        lastPackSendTick = 0;
    }

    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        float timestep = World.GetOrCreateSystem<WorldUpdateSystem>().Timestep();
        int currentTick = World.GetOrCreateSystem<WorldUpdateSystem>().GetTick();
        int sendTickDelay = 4;//send inputs every 4 frames

        Entities
            .WithAll<PlayerTag>()
            .ForEach((DynamicBuffer<ServerInputStateBuffer> buffer) =>
            {

                DynamicBuffer<InputState> inputStateBuffer = buffer.Reinterpret<InputState>();
                if (inputStateBuffer.Length > 0 && currentTick > lastPackSendTick + sendTickDelay)
                {
                    lastPackSendTick = currentTick;

                    //send inputs
                    inputStateMessage.Send(inputStateBuffer);

                    //clear server inputs
                    inputStateBuffer.Clear();
                }
            }).WithoutBurst().Run();
    }
}
