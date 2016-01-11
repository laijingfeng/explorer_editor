using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 资源打包_场景
/// </summary>
public partial class AssetBundleBuilder : EditorWindow
{
    /// <summary>
    /// 打包Scene
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <returns>是否是场景</returns>
    private bool ProcessScene(string path, UnityEngine.Object obj)
    {
        if (path.Contains("/Scene/") == false)
        {
            return false;
        }

        if (path.Contains(".prefab") == false)
        {
            return false;
        }

        GameObject go = obj as GameObject;

        if (go == null)
        {
            return true;
        }

        Scene scene = go.GetComponent<Scene>();

        if (scene == null)
        {
            return true;
        }

        Build(path, obj);
        return true;
    }
}
