using Godot;

public class PlayerInput : Node2D
{
  [Signal]
  public delegate void PlayerMoving(Vector2 vector);
  [Signal]
  public delegate void PlayerLookToDirection(Vector2 dir);
  [Signal]
  public delegate void PlayerAttack(Vector2 dir);
  [Signal]
  public delegate void PlayerCycleWeapon(int delta);

  [Export]
  public bool Enabled = true;
  [Export]
  public bool FreeLook = true;
  public Vector2 LookDirection;
  public Vector2 MovingVector;

  private Player _pc;
  private Vector2 _prevMouseScreenPos;

  public override void _Ready()
  {
    Input.MouseMode = Input.MouseModeEnum.Confined;
    _pc = GetNodeOrNull<Player>("..");
    if (_pc == null)
    {
      Enabled = false;
    }
  }
  public override void _Process(float delta)
  {
    if (Enabled)
    {
      UpdateMove();
      UpdateFreeLook();
      if (Input.IsActionJustPressed(InputNames.PREV_WEAPON))
      {
        EmitSignal(nameof(PlayerCycleWeapon), -1);
      }
      if (Input.IsActionJustPressed(InputNames.NEXT_WEAPON))
      {
        EmitSignal(nameof(PlayerCycleWeapon), 1);
      }
      if (Input.IsActionPressed(InputNames.ATTACK))
      {
        EmitSignal(nameof(PlayerAttack), LookDirection);
      }
    }
  }

  public override void _UnhandledInput(InputEvent e)
  {
    if (e is InputEventMouseButton)
    {
      var mouseEvent = e as InputEventMouseButton;
      if (mouseEvent.IsPressed())
      {
        if (mouseEvent.ButtonIndex == (int)ButtonList.WheelUp)
        {
          EmitSignal(nameof(PlayerCycleWeapon), -1);
        }
        if (mouseEvent.ButtonIndex == (int)ButtonList.WheelDown)
        {
          EmitSignal(nameof(PlayerCycleWeapon), 1);
        }
      }
    }
  }

  public void UpdateMove()
  {
    var vector = Input.GetVector(InputNames.MOVE_LEFT, InputNames.MOVE_RIGHT, InputNames.MOVE_UP, InputNames.MOVE_DOWN);
    if (vector == Vector2.Zero)
    {
      if (Input.IsActionPressed(InputNames.MOVE_UP))
      {
        vector.y--;
      }
      if (Input.IsActionPressed(InputNames.MOVE_DOWN))
      {
        vector.y++;
      }
      if (Input.IsActionPressed(InputNames.MOVE_LEFT))
      {
        vector.x--;
      }
      if (Input.IsActionPressed(InputNames.MOVE_RIGHT))
      {
        vector.x++;
      }
      vector = vector.Normalized();
    }
    MovingVector = vector;
    EmitSignal(nameof(PlayerMoving), vector);
  }

  public void UpdateFreeLook()
  {
    if (!FreeLook)
    {
      LookDirection = Vector2.Zero;
      return;
    };

    var stickVector = Input.GetVector(InputNames.LOOK_LEFT, InputNames.LOOK_RIGHT, InputNames.LOOK_UP, InputNames.LOOK_DOWN);
    var mouseScreenPos = GetViewport().GetMousePosition();
    Vector2 dir = LookDirection;
    if (stickVector != Vector2.Zero)
    {
      dir = stickVector.Normalized();
    }
    else if (mouseScreenPos != _prevMouseScreenPos)
    {
      // update mouse looking dir when mouse is moved so that
      // it won't override game-pad looking when there's no input
      dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
      _prevMouseScreenPos = mouseScreenPos;
    }

    LookDirection = dir;
    EmitSignal(nameof(PlayerLookToDirection), LookDirection);
  }
}