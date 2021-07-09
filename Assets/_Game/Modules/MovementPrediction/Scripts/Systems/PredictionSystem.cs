using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PredictionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        float timestep = World.GetOrCreateSystem<WorldUpdateSystem>().Timestep();
        int currentTick = World.GetOrCreateSystem<WorldUpdateSystem>().GetTick();
        int predicionDelay = World.GetOrCreateSystem<WorldUpdateSystem>().GetPredictionDelay();
        float moveSpeed = 5;

        float3 position = new float3();///TODO: this is only for testing purposes, this must come from the server
        bool updateShadowPosition = false;///TODO: this is only for testing purposes, this must come from the server

        Entities
            .WithAll<PlayerTag>()
            .ForEach((DynamicBuffer<InputStateBuffer> buffer, ref PendingInputs pendingInputs, ref Translation translation) =>
        {
            DynamicBuffer<InputState> inputStateBuffer = buffer.Reinterpret<InputState>();
            if (inputStateBuffer.Length > 0 && inputStateBuffer[0].tick + predicionDelay <= currentTick)
            {
                float2 moveDirection = inputStateBuffer[0].moveDirection;
                translation.Value += moveSpeed * new float3(moveDirection.x, moveDirection.y, 0) * dt;
                inputStateBuffer.RemoveAt(0);
                pendingInputs.prediction--;

                position = translation.Value;///TODO: this is only for testing purposes, this must come from the server
                updateShadowPosition = true;///TODO: this is only for testing purposes, this must come from the server
            }
        }).Run();

        ///TODO: this is only for testing purposes, this must come from the server
        ///also this is not going to be perfect, I only want to add some positions to the shadow player to test the movement
        if (updateShadowPosition)
        {
            Entities
                .WithAll<ShadowPlayerTag>()
                .ForEach((DynamicBuffer<LocalShadowPlayerBuffer> buffer) =>
                {
                    DynamicBuffer<ShadowState> shadowStateBuffer = buffer.Reinterpret<ShadowState>();

                    ShadowState state = new ShadowState();
                    state.tick = currentTick;
                    state.position = position;

                    shadowStateBuffer.Add(state);
                }).Run();
        }
        ///
    }
}
