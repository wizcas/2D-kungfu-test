using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class AttackNode : Node2D
{
  protected Creature _owner;
  protected IWeapon _weapon;
  protected HashSet<IWeapon> _arsenal = new HashSet<IWeapon>();
  private ulong _nextAttackTime = 0;

  public override void _Ready()
  {

  }

  public async void Prepare(Creature owner, IWeapon weapon)
  {
    _owner = owner;
    _weapon = weapon;

    if (owner != null && weapon != null)
    {
      if (!_arsenal.Contains(weapon))
      {
        _arsenal.Add(weapon);
      }
      await weapon.Equip(_owner);
    }
  }

  protected void Attack(Vector2 dir)
  {
    if (OS.GetTicksMsec() >= _nextAttackTime && _weapon != null)
    {
      _nextAttackTime = OS.GetTicksMsec() + (ulong)Mathf.CeilToInt(_weapon.GetCoolDown() * 1000);
      _owner.PlayAnimation("punch");
      _weapon.Perform(dir);
    }
  }
}