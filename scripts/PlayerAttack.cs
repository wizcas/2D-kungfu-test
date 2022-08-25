using System.Collections.Generic;
using Godot;

public class PlayerAttack : Area2D
{
  [Signal]
  public delegate void Attacking(Vector2 origin, Vector2 dir, float power);

  private HashSet<ulong> _targets = new HashSet<ulong>();

  public override void _UnhandledInput(InputEvent e)
  {
    if (e.IsActionPressed("attack"))
    {
      Attack();
    }

  }

  private void Attack()
  {
    // EmitSignal(nameof(Attacking), GlobalPosition, Vector2.Zero, 256);
    GD.Print("attacking");
    foreach (var id in _targets)
    {
      var target = GD.InstanceFromId(id);
      if (target is IHittable)
      {
        (target as IHittable).OnHit(GlobalPosition, 256);
      }
    }
  }

  public void OnBodyEnter(Node body)
  {
    if (body is IHittable)
    {
      var id = body.GetInstanceId();
      if (!_targets.Contains(id))
      {
        _targets.Add(id);
        GD.Print($"{body.Name} entered the attack range");
      }
    }
  }

  public void OnBodyLeave(Node body)
  {
    var id = body.GetInstanceId();
    if (_targets.Contains(id))
    {
      _targets.Remove(id);
      GD.Print($"{body.Name} has left the attach range");
    }
  }
}