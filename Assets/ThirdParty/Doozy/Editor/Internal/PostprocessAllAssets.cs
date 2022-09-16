using UnityEditor;

namespace Doozy.Editor.Internal
{
    public class PostprocessAllAssets : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
//            DDebug.Log("OnPostprocessAllAssets: Running Doozy Assets Processor...");
        }
    }
}