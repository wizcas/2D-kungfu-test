using Godot;

public interface IHittable
{
  void OnHit(Vector2 globalOrigin, float power);
}