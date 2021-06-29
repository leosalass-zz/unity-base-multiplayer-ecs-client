using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class InputStoreSystem : SystemBase
{
    private PlayerActions playerActions;

    //Called when this system is created.
    protected override void OnCreate()
    {
        playerActions = new PlayerActions();
    }

    protected override void OnStartRunning()
    {
        playerActions.Enable();
        //playerActions.Grounded.Attack.performed += ctx => { AttackFunction(); };
        //playerActions.Grounded.Move.performed += ctx => { MoveFunction(ctx.ReadValue<Vector2>()); };
    }

    protected override void OnStopRunning()
    {
        playerActions.Disable();
    }

    protected override void OnUpdate()
    {
        Vector2 moveDirection = playerActions.Grounded.Move.ReadValue<Vector2>();

        if (moveDirection != Vector2.zero)
        {
            int tick = World.GetOrCreateSystem<WorldUpdateSystem>().GetTick();
            int status = (int)InputStateStatus.Pending;
            float2 convertedDirection = new float2(moveDirection.x, moveDirection.y);

            Entities.ForEach((DynamicBuffer<InputStateBuffer> buffer, ref PendingInputs pendingInputs) =>
            {
                DynamicBuffer<InputState> inputStateBuffer = buffer.Reinterpret<InputState>();
                InputState state = new InputState(tick, status, convertedDirection);
                inputStateBuffer.Add(state);

                pendingInputs.prediction++;
            }).Run();
        }
    }
}
