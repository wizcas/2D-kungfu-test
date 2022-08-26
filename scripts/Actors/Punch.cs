using Godot;

public class Punch : Node2D, IWeapon
{
  [Export]
  public float power = 200;

  private AnimationPlayer _anim;
  private Node2D _owner;
  private Vector2 _dir;

  private PlayerMove pc
  {
    get
    {
      return _owner == null ? null :
      _owner.GetNode("..") as PlayerMove;
    }
  }

  public override void _Ready()
  {
    _anim = GetNode<AnimationPlayer>("Anim");
  }

  public void Equip(Node2D owner)
  {
    _owner = owner;
  }

  public void Play(Vector2 dir)
  {
    _dir = dir;
    _anim.Play("hit");
    pc?.Hold(_anim.GetAnimation("hit").Length);
  }

  public void OnBodyEntered(Node body)
  {
    if (body is IHittable)
    {
      (body as IHittable).OnHit(GlobalPosition, power);
    }
  }

  public void Dash()
  {
    if (_owner == null || _dir == Vector2.Zero)
    {
      return;
    }
    pc?.Dash(_dir * 6, .1f);
  }
}