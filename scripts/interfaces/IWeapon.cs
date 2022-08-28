using System.Threading.Tasks;
using Godot;

public interface IWeapon
{
  float GetCoolDown();
  Task Equip(Creature owner);
  Task Remove();
  Task Perform(Vector2 dir);
}