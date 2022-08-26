using System.Collections.Generic;
using Godot;

public class PlayerAttack : Node2D
{
  [Signal]
  public delegate void Attacking(Vector2 origin, Vector2 dir, float power);

  [Export]
  public NodePath attack;

  [Export]
  public float frequency = 1;

  private IAttack _attack;
  private ulong _nextAttackTime = 0;

  private HashSet<ulong> _targets = new HashSet<ulong>();

  public override void _Ready()
  {
    _attack = GetNode(attack) as IAttack;
    Input.MouseMode = Input.MouseModeEnum.Confined;
  }
  public override void _Process(float delta)
  {
    var dir = GetGlobalMousePosition() - GlobalPosition;
    UpdateAttackDir(dir);

    if (Input.IsActionPressed("attack"))
    {
      Attack();
    }
  }

  private void Attack()
  {
    if (!(_attack is IAttack))
    {
      GD.PrintErr($"not an IAttack", _attack, attack);
      return;
    }

    if (OS.GetTicksMsec() >= _nextAttackTime)
    {
      _nextAttackTime = OS.GetTicksMsec() + (ulong)Mathf.CeilToInt(1 / frequency * 1000);
      _attack.Play();
    }
  }

  private void UpdateAttackDir(Vector2 dir)
  {
    var node = _attack as Node2D;
    if (node == null) return;
    node.Rotation = dir.Angle() + Mathf.Pi / 2;
  }
}
