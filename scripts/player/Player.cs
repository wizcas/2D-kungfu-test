using Godot;

public class Player : Creature
{
  [Export]
  public PlayerInput PlayerInput;

  public override void _Ready()
  {
    base._Ready();
    PlayerInput = GetNodeOrNull<PlayerInput>("PlayerInput");
    if (PlayerInput != null)
    {
      PlayerInput.Connect(nameof(PlayerInput.PlayerLookToDirection), this, nameof(OnPlayerLookToDirection));
    }

  }

  public override void _PhysicsProcess(float delta)
  {
    base._PhysicsProcess(delta);

    if (PlayerInput != null)
    {
      PlayerInput.Enabled = _state == Creature.State.Free;

      if (_state == Creature.State.Free)
      {
        _velocity = PlayerInput.MovingVector * WalkSpeed;
        MoveAndSlide(_velocity);
      }
    }
  }
}