using System;
using Godot;

public static class Directions
{
  public static class Suffix
  {
    public const string N = "n";
    public const string S = "s";
    public const string W = "w";
    public const string E = "e";
  }

  public static string ComputeSuffix(float rad, bool lazy = false)
  {
    var predicates = lazy ? LAZY_RANGES : GREEDY_RANGES;
    foreach (var predicate in predicates)
    {
      if (predicate.Item2(rad)) return predicate.Item1;
    }
    return null;
  }
  public static string ComputeSuffix(Vector2 dir, bool lazy = false)
  {
    if (dir.LengthSquared() == 0) return null;
    return ComputeSuffix(dir.Normalized().Angle(), lazy);
  }

  private struct DirectionRange
  {
    public float Min;
    public float Max;
    public DirectionRange(float min, float max)
    {
      Min = min;
      Max = max;
    }
  }


  private static float Pi(float factor)
  {
    return factor * Mathf.Pi;
  }

  private class RangePredicate : Tuple<string, Func<float, bool>>
  {
    public RangePredicate(string suffix, Func<float, bool> predicate) : base(suffix, predicate) { }
  }

  private static RangePredicate[] LAZY_RANGES = new[]{
    new RangePredicate(Suffix.E, (rad)=>rad >= Pi(-.2f) && rad <= Pi(.2f)),
    new RangePredicate(Suffix.N, (rad)=>rad >= Pi(-.7f) && rad <= Pi(-.3f)),
    new RangePredicate(Suffix.W, (rad)=>rad >= Pi(.8f) || rad <= Pi(-.8f)),
    new RangePredicate(Suffix.S, (rad)=>rad >= Pi(.3f) && rad <= Pi(.7f)),
  };

  private static RangePredicate[] GREEDY_RANGES = new[]{
    new RangePredicate(Suffix.E, (rad)=>rad >= Pi(-.25f) && rad < Pi(.25f)),
    new RangePredicate(Suffix.N, (rad)=>rad >= Pi(-.75f) && rad <= Pi(-.25f)),
    new RangePredicate(Suffix.W, (rad)=>rad >= Pi(.75f) || rad <= Pi(-.75f)),
    new RangePredicate(Suffix.S, (rad)=>rad >= Pi(.25f) && rad <= Pi(.75f)),
  };

}