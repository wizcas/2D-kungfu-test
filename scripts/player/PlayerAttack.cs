using System;
using System.Collections.Generic;
using Godot;

public class PlayerAttack : Node2D
{
  [Export]
  public NodePath Weapon;

  private Player _pc;
  private IWeapon _weapon;
  private ulong _nextAttackTime = 0;

  public override void _Ready()
  {
    _weapon = GetNode(Weapon) as IWeapon;

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
    if (OS.GetTicksMsec() >= _nextAttackTime && _weapon != null)
    {
      _nextAttackTime = OS.GetTicksMsec() + (ulong)Mathf.CeilToInt(_weapon.GetCoolDown() * 1000);
      _pc.PlayAnimation("punch");
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
