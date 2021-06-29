using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class WorldUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        float timestep = Timestep();

        World.GetOrCreateSystem<FixedStepSimulationSystemGroup>().Timestep = timestep;

        Entities
            .WithAll<WorldUpdateTag>()
            .ForEach((ref WorldUpdateComponent worldUpdateComponent) =>
        {
            worldUpdateComponent.timestep = timestep;
        }).Run();

    }

    protected override void OnUpdate()
    {
        Entities
            .WithAll<WorldUpdateTag>()
            .ForEach((ref WorldUpdateComponent worldUpdateComponent) =>
            {
                worldUpdateComponent.tick++;
            }).Run();
    }

    public int GetTick()
    {
        int data = 0;

        Entities.WithAll<WorldUpdateTag>().ForEach((in WorldUpdateComponent worldUpdateComponent) =>
        {
            data = worldUpdateComponent.tick;
        }).Run();

        return data;
    }

    public int GetPredictionDelay()
    {
        int data = 0;

        Entities.WithAll<WorldUpdateTag>().ForEach((in WorldUpdateComponent worldUpdateComponent) =>
        {
            data = worldUpdateComponent.predicionDelay;
        }).Run();

        return data;
    }
    public int GetShadowDelay()
    {
        int data = 0;

        Entities.WithAll<WorldUpdateTag>().ForEach((in WorldUpdateComponent worldUpdateComponent) =>
        {
            data = worldUpdateComponent.shadowDelay;
        }).Run();

        return data;
    }

    public float Timestep()
    {
        float minute = 1;
        float tickRate = 60;

        return minute / tickRate;
    }
}
