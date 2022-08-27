using System;
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

  private Player _pc;
  private IWeapon _weapon;
  private ulong _nextAttackTime = 0;

  public override void _Ready()
  {
    _weapon = GetNode(weapon) as IWeapon;

  }
  public override void _Process(float delta)
  {
    if (_pc == null)
    {
      _pc = GetNodeOrNull<Player>("..");
      if (_pc != null && _pc.PlayerInput != null)
      {
        _pc.PlayerInput.Connect(nameof(PlayerInput.PlayerLookToDirection), this, nameof(OnPlayerLookToDirection));
        _pc.PlayerInput.Connect(nameof(PlayerInput.PlayerAttack), this, nameof(Attack));
        _weapon?.Equip(_pc);
      }
    }
  }

  private void Attack(Vector2 dir)
  {
    _pc.PlayAnimation("punch");
    if (OS.GetTicksMsec() >= _nextAttackTime)
    {
      _nextAttackTime = OS.GetTicksMsec() + (ulong)Mathf.CeilToInt(cd * 1000);
      _weapon.Perform(dir);
    }
  }

  public void OnPlayerLookToDirection(Vector2 dir)
  {
    var node = _weapon as Node2D;
    if (node == null) return;
    node.Rotation = dir.Angle();
  }
  private void OnPlayerAttack(Vector2 dir)
  {
    Attack(dir);
  }

}
