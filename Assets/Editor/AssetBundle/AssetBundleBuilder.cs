using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// 资源打包
/// </summary>
public partial class AssetBundleBuilder : EditorWindow
{
    [@MenuItem("Assets/AssetBundleBuilder")]
    private static void ShowAssetBundleBuilder()
    {
        EditorWindow.GetWindow(typeof(AssetBundleBuilder));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("打包"))
        {
            if (GetCurrentDataFolderName())
            {
                PackSelection();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 打包选中资源
    /// </summary>
    private void PackSelection()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        foreach (UnityEngine.Object obj in selection)
        {
            string path = GetSelectedPath(obj);

            //过滤掉文件夹
            if (!File.Exists(path))
            {
                continue;
            }

            if (IsPathLegal(path) == false)
            {
                continue;
            }

            try
            {
                if (ProcessLevel(path, obj))
                {
                    continue;
                }

                if (ProcessScene(path, obj))
                {
                    continue;
                }

                if (obj == null)
                {
                    UnityEngine.Debug.LogError("obj == null");
                    continue;
                }

                Build(path, obj);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error");
            }
        }
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="srcPath">是相对工程的地址，Assets/xxx</param>
    /// <param name="prefab"></param>
    private void Build(string srcPath, UnityEngine.Object prefab)
    {
        string bundlePath = srcPath.Replace("Assets/" + m_strCurrentDataFolderName + "/", "AssetBundles/");
        bundlePath = Path.GetDirectoryName(bundlePath);

        if (Directory.Exists(m_strGamePath) == false)
        {
            UnityEngine.Debug.LogError("游戏端不存在");
            return;
        }

        string prfabName = prefab.name;
        string gameDirectory = m_strGamePath + bundlePath;

        if (!Directory.Exists(gameDirectory))
        {
            Directory.CreateDirectory(gameDirectory);
        }

        string gamePath = gameDirectory + "/" +  prfabName + ".unity3d";

        if (srcPath.Contains(".mp4"))
        {
            File.Copy(srcPath, gamePath, true);
        }
        else
        {
#if UNITY_WEBPLAYER
            BuildTarget buildTarget = BuildTarget.WebPlayer;
#elif UNITY_IPHONE
            BuildTarget buildTarget = BuildTarget.iPhone;
#elif UNITY_ANDROID
            BuildTarget buildTarget = BuildTarget.Android;
#else
            BuildTarget buildTarget = BuildTarget.WebPlayer;
#endif

            if (!BuildPipeline.BuildAssetBundle(
                prefab,
                null,
                gamePath,
                BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle,
                buildTarget
                ))
            {
                UnityEngine.Debug.LogError("BuildFail name = " + prefab.name);
            }
        }

        if (Directory.Exists(m_strClientPath))
        {
            string clientDirectory = m_strClientPath + bundlePath;

            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string clientPath = clientDirectory + "/" + prfabName + ".unity3d";

            File.Copy(gamePath, clientPath, true);
        }
    }

    /// <summary>
    /// <para>获得选中物体的Assets路径</para>
    /// <para>路径格式：Assets/xxx[/xxx.xxx]</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private string GetSelectedPath(UnityEngine.Object obj)
    {
        if (obj == null && Selection.activeObject == null)
        {
            UnityEngine.Debug.LogError("Nothing Selected!");
            return string.Empty;
        }
        string path = AssetDatabase.GetAssetPath(obj == null ? Selection.activeObject : obj);
        return path;
    }
}
