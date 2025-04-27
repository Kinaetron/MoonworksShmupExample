using MoonTools.ECS;
using MoonWorks.Input;
using MoonWorksShumpExample.Components;

namespace MoonWorksShumpExample.Systems;

public struct InputState
{
    public ButtonState Left { get; set; }
    public ButtonState Right { get; set; }
    public ButtonState Up { get; set; }
    public ButtonState Down { get; set; }
    public ButtonState Shoot { get; set; }
}

public class ControlSet
{
    public VirtualButton Left { get; set; } = new EmptyButton();
    public VirtualButton Right { get; set; } = new EmptyButton();
    public VirtualButton Up { get; set; } = new EmptyButton();
    public VirtualButton Down { get; set; } = new EmptyButton();
    public VirtualButton Shoot { get; set; } = new EmptyButton();
}

public class Input : MoonTools.ECS.System
{
    private readonly Filter _playerFilter;

    private readonly ControlSet _gamepad = new();
    private readonly ControlSet _keyboard = new();

    public Input(World world, Inputs inputs)
        :base(world)
    {
        _playerFilter = FilterBuilder
            .Include<Player>()
            .Build();

        _keyboard.Up = inputs.Keyboard.Button(KeyCode.W);
        _keyboard.Down = inputs.Keyboard.Button(KeyCode.S);
        _keyboard.Left = inputs.Keyboard.Button(KeyCode.A);
        _keyboard.Right = inputs.Keyboard.Button(KeyCode.D);
        _keyboard.Shoot = inputs.Keyboard.Button(KeyCode.Space);

        _gamepad.Up = inputs.GetGamepad(0).LeftYDown;
        _gamepad.Down = inputs.GetGamepad(0).LeftYUp;
        _gamepad.Left = inputs.GetGamepad(0).LeftXLeft;
        _gamepad.Right = inputs.GetGamepad(0).LeftXRight;
        _gamepad.Shoot = inputs.GetGamepad(0).A;
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _playerFilter.Entities)
        {
            var inputState = InputState(_keyboard, _gamepad);
            Set(entity, inputState);
        }
    }

    private static InputState InputState(ControlSet keyboardSet, ControlSet gamepadSet)
    {
        return new InputState
        {
            Left = keyboardSet.Left.State | gamepadSet.Left.State,
            Right = keyboardSet.Right.State | gamepadSet.Left.State,
            Up = keyboardSet.Up.State | gamepadSet.Up.State,
            Down = keyboardSet.Down.State | gamepadSet.Down.State,
            Shoot = keyboardSet.Shoot.State | gamepadSet.Shoot.State
        };
    }
}
