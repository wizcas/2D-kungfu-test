using System;
using Godot;

public class PlayerMove : KinematicBody2D
{
  [Export]
  public float speed = 64;
  private Vector2 _dir = Vector2.Zero;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
  }

  public override void _PhysicsProcess(float delta)
  {
    MoveAndSlide(ComputeVelocityByInput());
  }

  private Vector2 ComputeVelocityByInput()
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
    dir = dir.Normalized() * speed;
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
