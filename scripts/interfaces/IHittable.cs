using Godot;

public interface IHittable
{
  void OnHit(Vector2 origin, float power);
}