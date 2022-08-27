using Godot;

public interface IWeapon
{
  float GetCoolDown();
  void Equip(Creature owner);
  void Perform(Vector2 dir);
}