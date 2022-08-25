using Godot;
public class WallTileSet : TileMap
{
  public override void _Ready()
  {
    var ids = TileSet.GetTilesIds();
    foreach (int id in ids)
    {
      var occ = TileSet.TileGetLightOccluder(id);
      if (occ != null)
      {
        occ.CullMode = OccluderPolygon2D.CullModeEnum.Clockwise;
      }
    }
  }
}