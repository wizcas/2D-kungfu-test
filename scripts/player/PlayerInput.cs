using Godot;

public class PlayerInput : Node
{
  [Export]
  public bool Enabled = true;
  private Creature _pc;
  private float _holdTime;


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
  }
  public Vector2 ComputeInput(float speed)
  {
    var velocity = Vector2.Zero;
    if (CanCompute)
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
    return velocity;
  }

  public void Hold(float time)
  {
    _holdTime = time;
  }
}