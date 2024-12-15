using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private InputSystem_Actions inputActions;
    private InputAction move;
    private InputAction jump;
    private InputAction attack;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        move = inputActions.Player.Move;
        jump = inputActions.Player.Jump;
        attack = inputActions.Player.Attack;
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
            Attack = attack.WasPressedThisFrame()
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool JumpRelease;
    public bool Attack;
}
