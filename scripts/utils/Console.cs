using Godot;

public class Console
{
  public static Console Instance = new Console();
  public void Error(string message)
  {
    GD.PrintErr(message);
    throw new System.Exception(message);
  }
}