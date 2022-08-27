using System.Collections.Generic;
using Godot;

public class PlayerAttack : Node2D
{
  [Signal]
  public delegate void Attacking(Vector2 origin, Vector2 dir, float power);

  [Export]
  public NodePath weapon;

  [Export]
  public float cd = .45f;

  private IWeapon _weapon;
  private ulong _nextAttackTime = 0;

  public override void _Ready()
  {
    Input.MouseMode = Input.MouseModeEnum.Confined;
    _weapon = GetNode(weapon) as IWeapon;
    if (_weapon != null) _weapon.Equip(this);
    GetNodeOrNull<PlayerInput>("../PlayerInput")?.Connect(nameof(PlayerInput.LookToDirection), this, nameof(OnLookToDirection));
  }
  public override void _Process(float delta)
  {
    var dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
    if (Input.IsActionPressed("attack"))
    {
      Attack(dir);
    }
  }

  private void Attack(Vector2 dir)
  {
    GetNode<Creature>("..").PlayAnimation("punch", true);
    if (OS.GetTicksMsec() >= _nextAttackTime)
    {
      _nextAttackTime = OS.GetTicksMsec() + (ulong)Mathf.CeilToInt(cd * 1000);
      _weapon.Perform(dir);
    }
  }

  public void OnLookToDirection(Vector2 dir)
  {
    var node = _weapon as Node2D;
    if (node == null) return;
    node.Rotation = dir.Angle();
  }
}
