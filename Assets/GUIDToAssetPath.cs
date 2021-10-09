using UnityEditor;
using UnityEngine;

public class GUIDToAssetPath : MonoBehaviour
{
    [MenuItem("APIExamples/GUIDToAssetPath")]
    static void MaterialPathsInProject()
    {
        var allMaterials = AssetDatabase.FindAssets("t: GameObject");

        foreach (var guid in allMaterials)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(path);
        }
    }
}
