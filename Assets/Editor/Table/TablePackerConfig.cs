using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// 打表工具配置
/// </summary>
public partial class TablePacker : EditorWindow
{
    /// <summary>
    /// 表格相关内容所在目录
    /// </summary>
    private static string m_strTablePath = Application.dataPath + "/../table/";

    /// <summary>
    /// table所在文件夹名称
    /// </summary>
    private static string m_strTableFileName = "table";
}
