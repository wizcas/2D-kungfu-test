using Godot;

public class PlayerAttack : AttackNode
{
  [Export]
  public PackedScene[] Arsenal = new PackedScene[] { };

  private ulong _nextAttackTime = 0;
  private int _weaponIndex = 0;

  private Player PC => _owner as Player;

  public override void _Ready()
  {
  }
  public override void _Process(float delta)
  {
    if (PC == null)
    {
      var pc = GetNodeOrNull<Player>("..");
      if (pc != null && pc.PlayerInput != null)
      {
        pc.PlayerInput.Connect(nameof(PlayerInput.PlayerLookToDirection), this, nameof(OnPlayerLookToDirection));
        pc.PlayerInput.Connect(nameof(PlayerInput.PlayerAttack), this, nameof(Attack));
        pc.PlayerInput.Connect(nameof(PlayerInput.PlayerCycleWeapon), this, nameof(OnPlayerCycleWeapon));
        Prepare(pc, GetActiveWeapon());
      }
    }
  }

  public void OnPlayerLookToDirection(Vector2 dir)
  {
    var node = _weapon as Node2D;
    if (node == null) return;
    node.Rotation = dir.Angle();
  }

  public async void OnPlayerCycleWeapon(int delta)
  {
    if (_weapon != null)
    {
      await _weapon.Remove();
    }
    _weaponIndex += delta;
    if (_weaponIndex >= Arsenal.Length) _weaponIndex = 0;
    if (_weaponIndex < 0) _weaponIndex = Arsenal.Length - 1;
    Prepare(PC, GetActiveWeapon());
  }

  private IWeapon GetActiveWeapon()
  {
    GD.Print($"active weapon @ {_weaponIndex}");
    if (Arsenal == null || Arsenal.Length == 0) return null;
    if (_weaponIndex >= Arsenal.Length) _weaponIndex = 0;
    var weapon = Arsenal[_weaponIndex].Instance();
    AddChild(weapon);
    return weapon as IWeapon;
  }
}
