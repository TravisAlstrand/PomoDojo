using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private InputSystem_Actions inputActions;
    private InputAction move;
    private InputAction jump;
    private InputAction attack;
    private InputAction submit;
    private InputAction cheat;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        move = inputActions.Player.Move;
        jump = inputActions.Player.Jump;
        attack = inputActions.Player.Attack;
        submit = inputActions.UI.Submit;
        cheat = inputActions.Player.Cheat;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        FrameInput = GatherInput();
    }

    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            Move = move.ReadValue<Vector2>(),
            Jump = jump.WasPressedThisFrame(),
            JumpRelease = jump.WasReleasedThisFrame(),
            Attack = attack.WasPressedThisFrame(),
            SubmitRelease = submit.WasReleasedThisFrame(),
            Cheated = cheat.WasPressedThisFrame()
        };
    }

    public void SwitchToUIMap()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
    }

    public void SwitchToGameplayMap()
    {
        inputActions.UI.Disable();
        inputActions.Player.Enable();
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool JumpRelease;
    public bool Attack;
    public bool SubmitRelease;
    public bool Cheated;
}
