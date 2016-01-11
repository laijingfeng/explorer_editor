using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 资源打包_关卡
/// </summary>
public partial class AssetBundleBuilder : EditorWindow
{
    /// <summary>
    /// 打包Level
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <returns>是否是关卡</returns>
    private bool ProcessLevel(string path, UnityEngine.Object obj)
    {
        if (path.Contains("/Scene/") == false)
        {
            return false;
        }

        if (path.Contains(".unity") == false)
        {
            return false;
        }

        if (EditorApplication.OpenScene(path) == false)
        {
            UnityEngine.Debug.LogError(path + " 打不开");
            return true;
        }

        if (Level.Instance == null)
        {
            return true;
        }

        GameObject levelGO = Level.Instance.gameObject;

        GameObject inst = Object.Instantiate(levelGO) as GameObject;

        ProcessLevel(inst);

        string prefabPath = path.Replace(".unity", ".prefab");

        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(prefabPath);

        PrefabUtility.ReplacePrefab(inst, tempPrefab, ReplacePrefabOptions.ConnectToPrefab);

        Object.DestroyImmediate(inst);

        tempPrefab = AssetDatabase.LoadMainAssetAtPath(prefabPath);

        Build(prefabPath, tempPrefab);

        AssetDatabase.DeleteAsset(prefabPath);

        return true;
    }

    /// <summary>
    /// 处理关卡
    /// </summary>
    /// <param name="go"></param>
    private void ProcessLevel(GameObject go)
    {
        Level level = go.GetComponent<Level>();

        if (level == null)
        {
            return;
        }

        GameObject goTrigger = Util.FindGo(level.gameObject, "Trigger");

        if (goTrigger == null)
        {
            return;
        }

        level.m_listTrigger.Clear();

        int id = 0;

        TriggerTimer[] timers = goTrigger.GetComponentsInChildren<TriggerTimer>(true);

        foreach (TriggerTimer tmp in timers)
        {
            tmp.m_iUniqueID = id++;
            level.m_listTrigger.Add(tmp);
        }

        TriggerRange[] rangs = goTrigger.GetComponentsInChildren<TriggerRange>(true);

        foreach (TriggerRange tmp in rangs)
        {
            tmp.m_iUniqueID = id++;
            if (tmp.transform.childCount > 0)
            {
                tmp.m_strItemName = tmp.transform.GetChild(0).name;
                Util.DestroyAllChildrenImmediate(tmp.transform.gameObject);
            }
            //tmp.m_bcTriggerBox = tmp.transform.GetComponent<BoxCollider2D>();
            //level.m_listTrigger.Add(tmp);
        }

        TriggerBoss[] bosses = goTrigger.GetComponentsInChildren<TriggerBoss>(true);

        foreach (TriggerBoss tmp in bosses)
        {
            //tmp.m_iUniqueID = id++;
            tmp.m_strBossName = tmp.transform.GetChild(0).name;
            //level.m_listTrigger.Add(tmp);
            Util.DestroyAllChildrenImmediate(tmp.transform.gameObject);
        }

       // Util.DestroyAllChildrenImmediate(goTrigger);

        foreach (Transform t in level.transform)
        {
            if (t.name.Equals("Player"))
            {
                level.m_PlayerPos = t.position;

                Util.DestroyAllChildrenImmediate(t.gameObject);
            }
        }
    }
}
