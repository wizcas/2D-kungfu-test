using System;
using Godot;

public class Creature : KinematicBody2D, IHittable
{
  private Vector2 _dir = Vector2.Zero;
  private float _speed = 0;
  private float _friction = 80;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    // GetNode("../PC/Attack").Connect("Attacking", this, "OnPlayerAttacking");
  }

  public override void _PhysicsProcess(float delta)
  {
    if (_speed > 0)
    {
      var c = MoveAndCollide(_dir * _speed * delta);
      if (c != null)
      {
        CollideWithTile(c);

        _dir = _dir.Bounce(c.Normal);
        _speed = Mathf.Clamp(_speed - 32, 4, 32);
      }
      else
      {
        _speed = DecaySpeed(_speed, delta);
      }
      if (Mathf.Abs(_speed - 0) < 1)
      {
        _speed = 0;
      }
    }
  }

  private void CollideWithTile(KinematicCollision2D collision)
  {
    var collider = collision.Collider;
    if (!(collider is TileMap)) return;
    var tm = collider as TileMap;
    var pos = tm.ToLocal(collision.Position - collision.Normal * 8);
    var coord = tm.WorldToMap(pos);

    var cellId = tm.GetCellv(coord);
    var tileName = tm.TileSet.TileGetName(tm.GetCellv(coord));
    var subtileCoord = tm.GetCellAutotileCoord((int)coord.x, (int)coord.y);
    GD.Print($"{Name} hit cell {tileName}{subtileCoord} @ {coord}");

    tm.SetCellv(coord, -1);
  }

  private float DecaySpeed(float speed, float delta)
  {
    return speed - _friction * delta;
  }

  public void OnPlayerAttacking(Vector2 origin, Vector2 dir, float power)
  {
    var to = GlobalPosition - origin;
    var dist = to.Length();
    GD.Print($"attacked, dist: {dist}");
    if (dist < 32)
    {
      _dir = to.Normalized();
      _speed = power;
      GD.Print($"dir: {_dir}, power: {_speed}");
    }
  }

  public void OnHit(Vector2 origin, float power)
  {
    var to = GlobalPosition - origin;
    var dist = to.Length();
    GD.Print($"{Name} on hit called, distance: {dist}, power: {power}");
    if (dist < 32)
    {
      _dir = to.Normalized();
      _speed = power;
      GD.Print($"{Name} is hit, power: {power}, dir: {_dir}, speed: {_speed}");
    }
  }
}
