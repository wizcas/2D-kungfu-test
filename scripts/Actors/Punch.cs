using Godot;

public class Punch : Node2D, IWeapon
{
  [Export]
  public float power = 200;

  private AnimationPlayer _anim;

  public override void _Ready()
  {
    _anim = GetNode<AnimationPlayer>("Anim");
  }

  public void Play()
  {
    _anim.Play("hit");
  }

  public void OnBodyEntered(Node body)
  {
    if (body is IHittable)
    {
      DoHit(body as IHittable);
    }
  }

  private void DoHit(IHittable target)
  {
    target.OnHit(GlobalPosition, power);
  }
}