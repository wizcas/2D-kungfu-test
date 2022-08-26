using System;
using Godot;

public class PlayerMove : KinematicBody2D
{
  [Export]
  public float walkSpeed = 64;
  [Export]
  public float dashSpeed = 128;
  private Vector2 _dir = Vector2.Zero;

  private Vector2 _forceVelocity = Vector2.Zero;
  private float _forceTime = 0;
  private float _holdTime = 0;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
  }

  public override void _PhysicsProcess(float delta)
  {
    if (_forceTime > 0)
    {
      _forceTime -= delta;
      GD.Print($"force moving: {_forceVelocity} in {_forceTime}s");
      MoveAndCollide(_forceVelocity * delta);
    }
    if (_holdTime > 0)
    {
      _holdTime -= delta;
    }

    if (_forceTime <= 0 && _holdTime <= 0)
    {
      MoveAndSlide(ComputeWalkInput());
    }

    ZIndex = (int)Position.y;
  }

  public void Hold(float time)
  {
    _holdTime = time;
  }

  public void ForceMove(Vector2 v, float time, bool jump = false)
  {
    _forceVelocity = v / time;
    _forceTime = time;
    GD.Print($"dest: {_forceVelocity} in {time}s");
    if (jump)
    {
      Jump(time);
    }
  }

  private void Jump(float time)
  {
    var anim = GetNode<AnimationPlayer>("Anim");
    if (anim == null) return;
    var upTime = anim.GetAnimation("jump-up").Length;
    var downTime = anim.GetAnimation("jump-down").Length;
    if (time < upTime + downTime) return;

    anim.Play("jump-up");
    GetTree().CreateTimer(time - downTime).Connect("timeout", this, nameof(OnJumpDown));
  }
  private void OnJumpDown()
  {
    var anim = GetNode<AnimationPlayer>("Anim");
    if (anim == null) return;
    anim.Play("jump-down");
  }

  private Vector2 ComputeWalkInput()
  {
    var dir = Vector2.Zero;
    if (Input.IsActionPressed(InputNames.MOVE_UP))
    {
      dir.y--;
    }
    if (Input.IsActionPressed(InputNames.MOVE_DOWN))
    {
      dir.y++;
    }
    if (Input.IsActionPressed(InputNames.MOVE_LEFT))
    {
      dir.x--;
    }
    if (Input.IsActionPressed(InputNames.MOVE_RIGHT))
    {
      dir.x++;
    }
    dir = dir.Normalized() * walkSpeed;
    return dir;
  }
}

public static class InputNames
{
  public const string MOVE_UP = "move_up";
  public const string MOVE_DOWN = "move_down";
  public const string MOVE_LEFT = "move_left";
  public const string MOVE_RIGHT = "move_right";
}
