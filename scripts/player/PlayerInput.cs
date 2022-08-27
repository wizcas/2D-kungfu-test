using Godot;

public class PlayerInput : Node2D
{
  [Signal]
  public delegate void LookToDirection(Vector2 dir);

  [Export]
  public bool Enabled = true;
  [Export]
  public bool FreeLook = true;
  private Creature _pc;
  private float _holdTime;
  private Vector2 _prevMouseScreenPos;

  private bool CanCompute
  {
    get { return Enabled && _holdTime <= 0; }
  }

  public override void _Ready()
  {
    _pc = GetNodeOrNull<Creature>("..");
    if (_pc == null)
    {
      Enabled = false;
    }
  }
  public override void _Process(float delta)
  {
    if (_holdTime > 0)
    {
      _holdTime -= delta;
    }
    UpdateFreeLook();
  }
  public Vector2 ComputeInput(float speed)
  {
    var velocity = Vector2.Zero;
    var stickVector = Input.GetVector(InputNames.MOVE_LEFT, InputNames.MOVE_RIGHT, InputNames.MOVE_UP, InputNames.MOVE_DOWN);
    if (CanCompute)
    {
      if (stickVector != Vector2.Zero)
      {
        velocity = stickVector * speed;
      }
      else
      {
        if (Input.IsActionPressed(InputNames.MOVE_UP))
        {
          velocity.y--;
        }
        if (Input.IsActionPressed(InputNames.MOVE_DOWN))
        {
          velocity.y++;
        }
        if (Input.IsActionPressed(InputNames.MOVE_LEFT))
        {
          velocity.x--;
        }
        if (Input.IsActionPressed(InputNames.MOVE_RIGHT))
        {
          velocity.x++;
        }
        velocity = velocity.Normalized() * speed;
      }
    }
    return velocity;
  }

  public void Hold(float time)
  {
    _holdTime = time;
  }

  public void UpdateFreeLook()
  {
    if (FreeLook)
    {
      var stickVector = Input.GetVector(InputNames.LOOK_LEFT, InputNames.LOOK_RIGHT, InputNames.LOOK_UP, InputNames.LOOK_DOWN);
      var mouseScreenPos = GetViewport().GetMousePosition();
      Vector2 dir = Vector2.Zero;
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

      if (dir != Vector2.Zero)
      {
        EmitSignal(nameof(LookToDirection), dir);
      }
    }
  }
}