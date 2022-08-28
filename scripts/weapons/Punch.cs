using System.Threading.Tasks;
using Godot;

public class Punch : Node2D, IWeapon
{
  [Export]
  public float Power = 200;
  [Export]
  public float CD = .45f;

  private AnimationPlayer _anim;
  private Creature _owner;
  private Vector2 _dir;

  public override void _Ready()
  {
    _anim = GetNode<AnimationPlayer>("Anim");
  }

  public Task Equip(Creature owner)
  {
    _owner = owner;
    return Task.CompletedTask;
  }

  public Task Remove()
  {
    QueueFree();
    return Task.CompletedTask;
  }

  public Task Perform(Vector2 dir)
  {
    _dir = dir;
    _anim.Play("hit");
    _owner.Hold(_anim.GetAnimation("hit").Length);
    return Task.CompletedTask;
  }

  public void OnBodyEntered(Node body)
  {
    GD.Print($"body {body.Name}, owner {_owner.Name}");
    if (body is IHittable && body != _owner && body.IsInGroup("enemy")) // todo: fix group
    {
      (body as IHittable).OnHit(GlobalPosition, Power);
    }
    else if (body is TileMap)
    {
      _owner?.ForceMove(-_dir * 2, .05f);
    }
  }

  public float GetCoolDown()
  {
    return CD;
  }
}