using Godot;

public class CreatureGUI : Control
{
  TextureProgress _hpBar;

  public override void _Ready()
  {
    _hpBar = GetNode<TextureProgress>("HpBar");
  }
  public void OnSelfHpChanged(int hp, int maxHp)
  {
    _hpBar.MaxValue = maxHp;
    _hpBar.Value = hp;
  }
}