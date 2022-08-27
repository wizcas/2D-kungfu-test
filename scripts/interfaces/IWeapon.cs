using Godot;

public interface IWeapon
{
  void Equip(Node2D owner);
  void Perform(Vector2 dir);
}