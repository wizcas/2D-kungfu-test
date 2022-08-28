using System;
using Godot;

public class Creature : KinematicBody2D, IHittable
{
  public enum State
  {
    Free,
    ForceMoving,
    PassiveMoving,
    Holding,
    Dead,
  }

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
  public float WalkSpeed = 64;
  public Vector2 LookDir = Vector2.Zero;

  protected int _hp = 0;
  protected State _state = State.Free;
  protected Vector2 _velocity = Vector2.Zero;
  private float _forceMoveTime = 0;
  private float _friction;
  private AnimatedSprite _animSprite;
  private string _dirSuffix = Directions.Suffix.S;
  private string _spriteAnimName = "Idle";
  private float _holdTime = 0;

  private AnimationPlayer _animator
  {
    get
    {
      return GetNodeOrNull<AnimationPlayer>("Anim");
    }
  }


  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    hp = maxHp;
    _animSprite = GetNodeOrNull<AnimatedSprite>("Body");
  }

  public override void _PhysicsProcess(float delta)
  {
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
      case State.Holding:
        if (_holdTime > 0)
        {
          _velocity = Vector2.Zero;
          _holdTime -= delta;
        }
        else
        {
          _state = State.Free;
          _holdTime = 0;
        }
        break;
      case State.PassiveMoving:
        GD.Print($"passive moving: {_velocity} ({_velocity.LengthSquared()})");
        if (_velocity.LengthSquared() < 2)
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
              GD.Print("collided ", _velocity);
            }
          }
          else
          {
            _velocity = SlowDown(_velocity, delta);
            GD.Print("slowed down: ", _velocity);
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
    }

    if (_velocity == Vector2.Zero)
    {
      _spriteAnimName = "Idle";
    }
    else
    {
      _spriteAnimName = "Walk";
    }

    PlaySpriteAnimation(_velocity == Vector2.Zero ? "Idle" : "Walk", GetDirectionSuffix());
  }

  public void Hold(float time)
  {
    _holdTime = time;
    _state = State.Holding;
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

  private async void Jump(float time)
  {
    var anim = _animator;
    if (anim == null) return;

    var upTime = anim.GetAnimation("jump-up").Length;
    var downTime = anim.GetAnimation("jump-down").Length;
    if (time < upTime + downTime) return;

    anim.Play("jump-up");
    await ToSignal(GetTree().CreateTimer(time - downTime), "timeout");
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

  public void OnHit(Vector2 globalOrigin, float power)
  {
    hp = Mathf.CeilToInt(hp - power);
    var to = GlobalPosition - globalOrigin;
    var v = to.Normalized() * power;
    PassiveMove(v);
    GD.Print($"{Name} is hit, power: {power}, dir: {to.Normalized()}, v: {v}");
  }

  public void Die()
  {
    _state = State.Dead;
    QueueFree();
  }
  protected string GetDirectionSuffix()
  {
    var looking = LookDir != Vector2.Zero;
    var dir = looking ? LookDir : _velocity.Normalized();
    var stopped = dir == Vector2.Zero;
    var suffix = _dirSuffix;
    if (!stopped && _state == State.Free)
    {
      var rad = dir.Normalized().Angle();
      suffix = Directions.ComputeSuffix(dir, !looking);
    }
    return suffix ?? _dirSuffix;
  }

  protected void PlaySpriteAnimation(string name, string suffix)
  {
    var fullName = GetAnimationFullName(name, suffix);
    _spriteAnimName = name;
    _dirSuffix = suffix;
    if (_animSprite != null && fullName != _animSprite.Animation)
    {
      GD.Print($"playing: {fullName}");
      _animSprite.Play(fullName);
    }
  }

  protected void PlaySelfAnimation(string name, string suffix)
  {
    var fullName = GetAnimationFullName(name, suffix);
    _animator.Play(fullName);
  }

  public void PlaySprite(string spriteAnimationName, bool inheritSuffix = true)
  {
    PlaySprite(spriteAnimationName, null, inheritSuffix);
  }
  public void PlaySprite(string spriteAnimationName, string suffix, bool inheritSuffix)
  {
    PlaySpriteAnimation(spriteAnimationName, inheritSuffix ? _dirSuffix : suffix);
  }
  public void PlayAnimation(string animation, bool inheritSuffix = true)
  {
    PlayAnimation(animation, null, inheritSuffix);
  }
  public void PlayAnimation(string animation, string suffix, bool inheritSuffix)
  {
    PlaySelfAnimation(animation, inheritSuffix ? _dirSuffix : suffix);
  }
  public void OnPlayerLookToDirection(Vector2 dir)
  {
    LookDir = dir;
  }

  private string GetAnimationFullName(string name, string suffix)
  {

    return name + (String.IsNullOrEmpty(suffix) ? "" : $"-{suffix}");
  }
}
