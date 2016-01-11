using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 资源打包配置
/// </summary>
public partial class AssetBundleBuilder : EditorWindow
{
    #region 配置

    /// <summary>
    /// <para>原始资源文件夹目录</para>
    /// <para>Assets下的一个文件夹名</para>
    /// <para>配置List有助于将来本地化管理</para>
    /// </summary>
    private List<string> m_listDataFolderName = new List<string>()
    {
        "Data",
    };

    /// <summary>
    /// 客户端地址
    /// </summary>
    private string m_strClientPath = "../explorer_client/";

    /// <summary>
    /// 游戏端地址
    /// </summary>
    private string m_strGamePath = "../explorer_game/";

    #endregion

    /// <summary>
    /// 当前打包的文件夹目录
    /// </summary>
    private string m_strCurrentDataFolderName;

    /// <summary>
    /// 获取当前打包的文件夹目录
    /// </summary>
    /// <returns>是否可打包</returns>
    private bool GetCurrentDataFolderName()
    {
        bool bRet = false;

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        if (selection == null
            || selection.Length <= 0)
        {
            return bRet;
        }

        List<string> listFolder = new List<string>();
        string path, dataPath;
        int idx;
        foreach (UnityEngine.Object obj in selection)
        {
            path = GetSelectedPath(obj);
            dataPath = path.Remove(0, 7);//删除"Assets/"
            idx = dataPath.IndexOf('/');
            if (idx != -1)
            {
                dataPath = dataPath.Substring(0, idx);
            }

            if (listFolder.Contains(dataPath) == false)
            {
                listFolder.Add(dataPath);
            }
        }

        int cnt = 0;
        foreach (string str in listFolder)
        {
            if (m_listDataFolderName.Contains(str) == true)
            {
                m_strCurrentDataFolderName = str;
                cnt++;
            }

            if (cnt > 1)
            {
                break;
            }
        }

        if (cnt == 0)
        {
            EditorUtility.DisplayDialog("没有选中资源目录", "请先选择打包资源文件夹，再打包资源！", "确定");
        }
        else if (cnt == 1)
        {
            bRet = true;
        }
        else
        {
            EditorUtility.DisplayDialog("选中了多个语言资源目录", "一次只能打包一种语言的资源", "确定");
        }

        return bRet;
    }

    /// <summary>
    /// 路径是否合法
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsPathLegal(string path)
    {
        if (path.Contains("Assets/" + m_strCurrentDataFolderName))
        {
            return true;
        }
        return false;
    }
}
