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

        ProcessTrigger(level);

        Transform playerPos = Util.FindCo<Transform>(level.transform, "Player");
        if (playerPos != null)
        {
            level.m_PlayerPos = playerPos.position;
            Util.DestroyAllChildrenImmediate(playerPos.gameObject);
        }
    }

    /// <summary>
    /// 处理触发器
    /// </summary>
    /// <param name="level">关卡</param>
    private void ProcessTrigger(Level level)
    {
        level.m_listTrigger.Clear();

        int uniqueID = 0;

        TriggerBase[] triggers = level.GetComponentsInChildren<TriggerBase>(true);

        foreach (TriggerBase tmp in triggers)
        {
            if (tmp is TriggerRange)
            {
                ProcessTriggerRange(tmp as TriggerRange);
            }
            else if (tmp is TriggerBoss)
            {
                ProcessTriggerBoss(tmp as TriggerBoss);
            }

            if (tmp.transform.childCount > 0)
            {
                Util.DestroyAllChildrenImmediate(tmp.transform.gameObject);
            }
            tmp.m_iUniqueID = ++uniqueID;
            tmp.enabled = false;
            level.m_listTrigger.Add(tmp);
        }
    }

    /// <summary>
    /// 处理范围触发器
    /// </summary>
    /// <param name="tmp"></param>
    private void ProcessTriggerRange(TriggerRange tmp)
    {
        BoxCollider2D bcd = tmp.transform.GetComponent<BoxCollider2D>();

        if (bcd == null)
        {
            UnityEngine.Debug.LogWarning("TriggerRange需要BoxCollider2D " + tmp.transform.name);
        }
        else
        {
            tmp.m_bcTriggerBox = bcd;
        }

        if (tmp.transform.childCount > 0)
        {
            tmp.m_strItemName = tmp.transform.GetChild(0).name;

            if (tmp.transform.childCount > 1)
            {
                UnityEngine.Debug.LogWarning("可视TriggerRange只能添加一个item子结点 " + tmp.transform.name);
            }

            if (tmp.m_strItemName.Contains("item_") == false)
            {
                UnityEngine.Debug.LogError("可视TriggerRange子结点需要是item " + tmp.transform.name);
            }
        }
    }

    /// <summary>
    /// 处理Boss触发器
    /// </summary>
    /// <param name="tmp"></param>
    private void ProcessTriggerBoss(TriggerBoss tmp)
    {
        if (tmp.transform.childCount > 0)
        {
            tmp.m_strBossName = tmp.transform.GetChild(0).name;

            if (tmp.transform.childCount > 1)
            {
                UnityEngine.Debug.LogWarning("TriggerBoss只能添加一个boss子结点 " + tmp.transform.name);
            }

            if (tmp.m_strBossName.Contains("boss_") == false)
            {
                UnityEngine.Debug.LogError("TriggerBoss子结点需要是boss " + tmp.transform.name);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("TriggerBoss需要添加boss子结点 " + tmp.transform.name);
        }
    }
}
