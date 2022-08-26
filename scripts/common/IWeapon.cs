using Godot;

public interface IWeapon
{
  void Equip(Node2D owner);
  void Play(Vector2 dir);
}