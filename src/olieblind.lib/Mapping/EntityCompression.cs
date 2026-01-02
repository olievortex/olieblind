using olieblind.data.Entities;

namespace olieblind.lib.Mapping;

public class EntityCompression
{
    public static void Compress(RadarInventoryEntity entity)
    {
        if (entity.FileList.Count < 2) return;

        var parts = entity.FileList[0].Split('_');
        if (parts.Length != 3) return;

        var prefix = $"{parts[0]}_";
        var suffix = $"_{parts[2]}";

        for (var i = 1; i < entity.FileList.Count; i++)
        {
            entity.FileList[i] = entity.FileList[i].Replace(prefix, "^").Replace(suffix, "$");
        }
    }

    public static void Decompress(RadarInventoryEntity entity)
    {
        if (entity.FileList.Count < 2) return;

        var parts = entity.FileList[0].Split('_');
        if (parts.Length != 3) return;

        var prefix = $"{parts[0]}_";
        var suffix = $"_{parts[2]}";

        for (var i = 1; i < entity.FileList.Count; i++)
        {
            entity.FileList[i] = entity.FileList[i].Replace("^", prefix).Replace("$", suffix);
        }
    }
}
