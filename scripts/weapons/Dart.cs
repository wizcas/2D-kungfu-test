using System;
using Godot;

public class Dart : Node2D, IWeapon, IProjectileLauncher
{
  [Export]
  public PackedScene Projectile;
  [Export]
  public float CD = .7f;
  [Export]
  public NodePath Muzzle;

  private Creature _owner;
  private Projectile _projectile;
  private Node2D _muzzle;

  public override void _Ready()
  {
    _muzzle = GetNode<Node2D>(Muzzle);
  }

  public void Equip(Creature owner)
  {
    _owner = owner;
    Prepare();
  }

  public void Prepare()
  {
    if (_projectile != null) return;
    _projectile = Projectile.Instance<Projectile>();
    AddChild(_projectile);
    _projectile.Position = _muzzle.Position;
  }

  public void Perform(Vector2 dir)
  {
    if (_projectile == null) return;
    _projectile.Shoot(this, dir);
    _projectile = null;
    Prepare();
  }

  public float GetCoolDown()
  {
    return CD;
  }
}
