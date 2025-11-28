using UnityEditor;
using UnityEngine;

public static class RemoveMissing
{
    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    static void RemoveMissingInScene()
    {
        int count = 0;
        foreach (var go in GameObject.FindObjectsOfType<GameObject>())
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        Debug.Log($"Removed {count} missing script components.");
    }
}
