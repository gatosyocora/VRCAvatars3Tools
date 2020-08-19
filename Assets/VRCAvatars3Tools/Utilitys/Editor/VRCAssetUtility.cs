using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gatosyocora.VRCAvatars3Tools.Utilitys
{
    public static class VRCAssetUtility
    {
        public static string GetVRCAssetPathForSearch(string filter)
            => AssetDatabase.FindAssets(filter)
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Where(p => p.Contains("/VRCSDK/") || p.Contains("\\VRCSDK\\"))
                .OrderBy(p => Path.GetFileName(p).Count())
                .FirstOrDefault();
    }
}
