using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ResolutionUtility
{
    public static List<Vector2Int> GetUniqueResolutions()
    {
        return Screen.resolutions
        .Select(x => new Vector2Int(x.width, x.height))
        .Distinct()
        .ToList();
    }

    // use this method when you only need debug
    public static void SetResolution(int width, int height, bool fullscreen = true)
    {
        Screen.SetResolution(width, height, fullscreen);
    }

    public static void PrintResolutions()
    {
        foreach (var r in Screen.resolutions)
        {
            Debug.Log($"Resolution: {r.width}x{r.height} @ {r.refreshRateRatio}Hz");
        }
    }
}
