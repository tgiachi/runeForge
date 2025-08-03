namespace Runeforge.Data.Entities.MapPrefab;

public class JsonMapPrefabStair
{
    public string FromFloor { get; set; }
    public string ToFloor { get; set; }
    public int[] FromPosition { get; set; } = new int[2];
    public int[] ToPosition { get; set; } = new int[2];


}
