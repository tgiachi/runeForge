namespace Runeforge.Data.Entities.Tileset;

public class JsonTileData
{
    public string Id { get; set; }
    public string Foreground { get; set; }
    public string Background { get; set; }
    public string Description { get; set; }
    public string Symbol { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsTransparent { get; set; }
    public List<string> Tags { get; set; } = [];
    public string? AnimationId { get; set; }
}
