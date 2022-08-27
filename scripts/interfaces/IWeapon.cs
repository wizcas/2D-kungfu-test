using Godot;

public interface IWeapon
{
  void Equip(Creature owner);
  void Perform(Vector2 dir);
}