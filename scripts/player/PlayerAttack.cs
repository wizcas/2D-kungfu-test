using System;
using System.Collections.Generic;
using Godot;

public class PlayerAttack : AttackNode
{
  [Export]
  public NodePath Weapon;

  private ulong _nextAttackTime = 0;

  private Player PC => _owner as Player;

  public override void _Ready()
  {

  }
  public override void _Process(float delta)
  {
    if (PC == null)
    {
      var pc = GetNodeOrNull<Player>("..");
      if (pc != null && pc.PlayerInput != null)
      {
        pc.PlayerInput.Connect(nameof(PlayerInput.PlayerLookToDirection), this, nameof(OnPlayerLookToDirection));
        pc.PlayerInput.Connect(nameof(PlayerInput.PlayerAttack), this, nameof(Attack));
        Prepare(pc, GetNode(Weapon) as IWeapon);
      }
    }
  }

  public void OnPlayerLookToDirection(Vector2 dir)
  {
    var node = _weapon as Node2D;
    if (node == null) return;
    node.Rotation = dir.Angle();
  }
}
