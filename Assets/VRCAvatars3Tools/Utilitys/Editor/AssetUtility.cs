using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gatosyocora.VRCAvatars3Tools.Utilitys
{
    public static class AssetUtility
    {
        public static string GetAssetPathForSearch(string filter)
            => AssetDatabase.FindAssets(filter)
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .OrderBy(p => Path.GetFileName(p).Count())
                .FirstOrDefault();
    }
}