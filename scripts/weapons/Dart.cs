using System;
using System.Threading.Tasks;
using Godot;

public class Dart : Node2D, IWeapon, IProjectileLauncher
{
  [Export]
  public PackedScene Projectile;
  [Export]
  public float CD = .7f;

  private Creature _owner;
  private Projectile _projectile;
  private Node2D _muzzle;
  private RayCast2D _indicator;

  private float _distance = -1;
  [Export]
  public float Distance
  {
    get { return _distance < 0 ? GetViewport().Size.x : _distance; }
    set
    {
      _distance = value;
      if (_indicator != null)
      {
        _indicator.CastTo = new Vector2(Distance, 0);
      }
    }
  }

  public override void _Ready()
  {
    _muzzle = GetNode<Node2D>("Muzzle");
    _indicator = _muzzle.GetNode<RayCast2D>("Indicator");
    Distance = Distance; // make sure the distance is valid
  }

  public override void _Process(float delta)
  {
    UpdateIndicator();
  }

  public Task Equip(Creature owner)
  {
    _owner = owner;
    Prepare();
    return Task.CompletedTask;
  }

  public Task Remove()
  {
    QueueFree();
    return Task.CompletedTask;
  }

  public void Prepare()
  {
    if (_projectile != null) return;
    _projectile = Projectile.Instance<Projectile>();
    AddChild(_projectile);
    _projectile.Position = _muzzle.Position;
  }

  public async Task Perform(Vector2 dir)
  {
    if (_projectile == null) return;
    _projectile.Shoot(this, dir);
    _projectile = null;
    await ToSignal(GetTree().CreateTimer(.5f), "timeout");
    Prepare();
  }

  public float GetCoolDown()
  {
    return CD;
  }

  public void UpdateIndicator()
  {
    if (_indicator == null) return;
    var line = _indicator.GetNode<Line2D>("Line");
    if (line == null) return;

    var endPos = line.ToLocal(_indicator.ToGlobal(_indicator.CastTo));
    if (_indicator.IsColliding())
    {
      endPos = line.ToLocal(_indicator.GetCollisionPoint());
    }
    line.Points = new Vector2[] { Vector2.Zero, endPos };
  }
}
