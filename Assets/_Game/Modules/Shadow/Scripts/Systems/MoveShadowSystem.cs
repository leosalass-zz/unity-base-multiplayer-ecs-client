using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class MoveShadowSystem : SystemBase
{
    protected override void OnUpdate()
    {

        float currentTick = World.GetOrCreateSystem<WorldUpdateSystem>().GetTick();
        float timestep = World.GetOrCreateSystem<WorldUpdateSystem>().Timestep();
        float shadowDelay = World.GetOrCreateSystem<WorldUpdateSystem>().GetShadowDelay();

        Entities
            .WithAll<ShadowPlayerTag>()
            .ForEach((DynamicBuffer<LocalShadowPlayerBuffer> buffer, ref Translation translation) =>
            {
                DynamicBuffer<ShadowState> shadowStateBuffer = buffer.Reinterpret<ShadowState>();
                if (shadowStateBuffer.Length > 1 && shadowStateBuffer[0].tick + shadowDelay < currentTick)
                {
                    float3 startPos = shadowStateBuffer[0].position;
                    int startTick = shadowStateBuffer[0].tick;
                    float3 finalPos = shadowStateBuffer[1].position;
                    int finalTick = shadowStateBuffer[1].tick;
                    int elapsedTicks = finalTick - startTick;

                    Vector3 pos = Vector3.Lerp(startPos, finalPos, timestep * elapsedTicks);
                    translation.Value.x = pos.x;
                    translation.Value.y = pos.y;
                    translation.Value.z = pos.z;

                    shadowStateBuffer.RemoveAt(0);
                }
            }).Schedule();
    }
}
