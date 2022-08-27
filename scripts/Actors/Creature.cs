using System;
using Godot;

public class Creature : KinematicBody2D, IHittable
{
  [Signal]
  public delegate void HpChanged(int hp, int maxHp);

  [Export]
  public int maxHp = 500;
  [Export]
  public int hp
  {
    get { return _hp; }
    set
    {
      _hp = value;
      EmitSignal(nameof(HpChanged), _hp, maxHp);
    }
  }
  [Export]
  public float walkSpeed = 64;
  public PlayerInput input;

  private int _hp = 0;
  private Vector2 _velocity = Vector2.Zero;
  private float _forceMoveTime = 0;
  private float _friction;
  private State _state = State.Free;
  private AnimatedSprite _animSprite;
  private string _currentDirSuffix = "s";


  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    hp = maxHp;
    input = GetNodeOrNull<PlayerInput>("PlayerInput");
    _animSprite = GetNodeOrNull<AnimatedSprite>("Body");
  }

  public override void _PhysicsProcess(float delta)
  {
    if (input != null)
    {
      input.Enabled = _state == State.Free;
    }
    ZIndex = (int)Position.y;

    if (_velocity.LengthSquared() <= 0)
    {
      if (hp <= 0)
      {
        Die();
      }
    }
    switch (_state)
    {
      case State.PassiveMoving:
        GD.Print($"passive moving: {_velocity} ({_velocity.LengthSquared()})");
        if (_velocity.LengthSquared() < 1)
        {
          _state = State.Free;
          _velocity = Vector2.Zero;
        }
        else
        {
          var c = MoveAndCollide(_velocity * delta);
          if (c != null)
          {
            if (CollideWithTile(c))
            {
              var speed = Mathf.Clamp(_velocity.Length() - 32, 4, 32); // TODO absorb amount
              _velocity = _velocity.Normalized().Bounce(c.Normal) * speed;
            }
          }
          else
          {
            _velocity = SlowDown(_velocity, delta);
          }

        }
        break;
      case State.ForceMoving:
        if (_forceMoveTime > 0)
        {
          _forceMoveTime -= delta;
          GD.Print($"force moving: {_velocity} in {_forceMoveTime}s");
          MoveAndCollide(_velocity * delta);
        }
        else
        {
          _state = State.Free;
          _velocity = Vector2.Zero;
        }
        break;
      case State.Free:
        if (input != null)
        {
          _velocity = input.ComputeInput(walkSpeed);
          MoveAndSlide(_velocity);
        }
        break;
    }
    UpdateCurrentDirSuffix();
  }
  public void ForceMove(Vector2 v, float distance, bool jump = false)
  {
    _state = State.ForceMoving;
    _forceMoveTime = distance / v.Length();
    _velocity = v / _forceMoveTime;
    GD.Print($"force move v {v} for {distance}px in {_forceMoveTime}s");
    if (jump)
    {
      Jump(_forceMoveTime);
    }
  }

  public void PassiveMove(Vector2 v)
  {
    _state = State.PassiveMoving;
    _friction = World.Friction;
    _velocity = v;
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

  private bool CollideWithTile(KinematicCollision2D collision)
  {
    var collider = collision.Collider;
    if (!(collider is TileMap)) return false;
    var tm = collider as TileMap;
    var pos = tm.ToLocal(collision.Position - collision.Normal * 8);
    var coord = tm.WorldToMap(pos);

    var cellId = tm.GetCellv(coord);
    var tileName = tm.TileSet.TileGetName(tm.GetCellv(coord));
    var subtileCoord = tm.GetCellAutotileCoord((int)coord.x, (int)coord.y);
    GD.Print($"{Name} hit cell {tileName}{subtileCoord} @ {coord}");
    tm.SetCellv(coord, -1);

    return cellId >= 0;
  }

  private Vector2 SlowDown(Vector2 velocity, float delta)
  {
    return velocity - velocity.Normalized() * _friction * delta;
  }

  public void OnHit(Vector2 origin, float power)
  {
    hp = Mathf.CeilToInt(hp - power);
    var to = GlobalPosition - origin;
    var v = to.Normalized() * power;
    PassiveMove(v);
    GD.Print($"{Name} is hit, power: {power}, dir: {to.Normalized()}, v: {v}");
  }

  public void Die()
  {
    QueueFree();
  }
  enum State
  {
    Free,
    Walk,
    ForceMoving,
    PassiveMoving,
  }

  private void UpdateCurrentDirSuffix()
  {
    var stopped = _velocity == Vector2.Zero;
    string suffix = _currentDirSuffix;
    string name;
    if (!stopped && _state == State.Free)
    {
      var rad = _velocity.Normalized().Angle();
      if (rad > -.25f * Mathf.Pi && rad <= .25f * Mathf.Pi)
      {
        suffix = "e";
      }
      else if (rad > -.75f * Mathf.Pi && rad <= -.25f * Mathf.Pi)
      {
        suffix = "n";
      }
      else if (rad > .75f * Mathf.Pi || rad <= -.75f * Mathf.Pi)
      {
        suffix = "w";
      }
      else
      {
        suffix = "s";
      }
      name = $"Walk-{suffix}";
    }
    else
    {
      name = $"Idle-{_currentDirSuffix}";
    }
    _currentDirSuffix = suffix;
    if (_animSprite != null && name != _animSprite.Animation)
    {
      GD.Print($"playing: {name}");
      _animSprite.Play(name);
    }
  }

  public void PlayAnimation(string name, bool withSuffix)
  {
    var anim = GetNode<AnimationPlayer>("Anim");
    if (anim == null) return;
    string animName = name + (withSuffix ? $"-{_currentDirSuffix}" : "");
    anim.Play(animName);
  }
}
