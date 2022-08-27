using Godot;

public class Projectile : KinematicBody2D
{
  [Export] public float Power = 50;
  [Export] public float Speed = 300;

  private IProjectileLauncher _launcher;
  private Vector2 _velocity;

  public override void _Ready()
  {

  }

  public override void _PhysicsProcess(float delta)
  {
    if (_velocity != Vector2.Zero)
    {
      var c = MoveAndCollide(_velocity * delta);
      if (c != null)
      {
        Hit(c.Collider as Node);
      }
    }
  }

  public void Shoot(IProjectileLauncher launcher, Vector2 dir)
  {
    var t = GlobalTransform;
    var sceneRoot = GetTree().Root.GetChild(0); // todo: global projectile manager
    GetParent().RemoveChild(this);
    sceneRoot.AddChild(this);
    GlobalTransform = t;

    _launcher = launcher;
    _velocity = dir * Speed;
    GD.Print("Projectile shot", _velocity);
  }

  public void Hit(Node body)
  {
    if (body == null) return;
    if (body is IHittable && body.IsInGroup("enemy")) //todo: fix group
    {
      (body as IHittable).OnHit(GlobalPosition, Power);
    }
    QueueFree();
  }
}
