using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Text;

/// <summary>
/// 打表的Excel信息
/// </summary>
[System.Serializable]
public class PackExcelInfo
{
    /// <summary>
    /// 表格名
    /// </summary>
    public string m_strExcelName;
};

[System.Serializable]
public class PackExcelStoreInfo
{
    /// <summary>
    /// 表格名
    /// </summary>
    public string m_strExcelName;
};

/// <summary>
/// 打表工具
/// </summary>
public partial class TablePacker : EditorWindow
{
    static public string dir;
    static string tablePath;
    static string table_outputPath;
    static string table_toolsPath;
    static string protoPath;
    static string table_outputStreamingPath;

    static bool CallProcess(string processName, string param)
    {
        ProcessStartInfo process = new ProcessStartInfo
        {
            CreateNoWindow = false,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = processName,
            Arguments = param,
        };

        UnityEngine.Debug.Log(processName + " " + param);

        Process p = Process.Start(process);
        p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        string error = p.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError(processName + " " + param + "  ERROR! " + "\n" + error);

            string output = p.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                UnityEngine.Debug.Log(output);
            }
            return false;
        }
        return true;
    }

    // no postfix (.cs or .proto)
    static bool ProcessMsgProto(string name)
    {
        string param = string.Format("-i:{0}.proto -o:{0}.cs -p:detectMissing", name);
        if (CallProcess("protogen.exe", param))
        {
            File.Copy(@".\" + name + ".cs", dir + @"\Assets\Scripts\Table\proto_gen\" + name + ".cs", true);

            File.Delete(@".\" + name + ".cs");
            return true;
        }

        return false;
    }

    // no postfix (.cs or .proto)
    static bool ProcessTableProto(string name)
    {
        string param = string.Format("-i:{0}.proto -o:{0}.cs -p:detectMissing", name);
        if (CallProcess("protogen.exe", param))
        {
            File.Copy(@".\" + name + ".cs", dir + @"\Assets\Scripts\Table\proto_gen\" + name + ".cs", true);

            File.Delete(@".\" + name + ".cs");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 生成Python文件
    /// </summary>
    /// <param name="name"></param>
    static void GeneratePythonFile(string name)
    {
        string param = string.Format("-I. --python_out=. {0}.proto", name);
        CallProcess("protoc.exe", param);
    }

    #region 新增

    [MenuItem("HotTools/PackSomeTables")]
    public static void PackSomeTables()
    {
        Rect rect = new Rect(0, 0, 500, 500);
        TablePacker window = EditorWindow.GetWindowWithRect(typeof(TablePacker), rect, true, "打表工具") as TablePacker;
        window.Show();
    }

    /// <summary>
    /// <para>要打表的Excel的数量</para>
    /// <para>0表示All</para>
    /// </summary>
    private int m_iPackTablesCount = 0;

    /// <summary>
    /// 打表为二进制
    /// </summary>
    private static bool m_bPackTableToBinary = false;

    /// <summary>
    /// 打表为二进制统计
    /// </summary>
    private static int m_iPackTableToBinaryCnt = 0;

    /// <summary>
    /// 编译表格Proto文件
    /// </summary>
    private static bool m_bCompileTableProtoFile = false;

    /// <summary>
    /// 编译表格Proto文件统计
    /// </summary>
    private static int m_iCompileTableProtoFileCnt = 0;

    /// <summary>
    /// 编译公共Proto文件
    /// </summary>
    private static bool m_bCompileCommonProtoFile = false;

    /// <summary>
    /// 编译公共Proto文件统计
    /// </summary>
    private static int m_iCompileCommonProtoFileCnt = 0;

    /// <summary>
    /// 要打表的Excel信息列表
    /// </summary>
    private static List<PackExcelInfo> m_listPackExcelInfo = new List<PackExcelInfo>();

    /// <summary>
    /// 存储信息
    /// </summary>
    private List<PackExcelStoreInfo> m_listPackStoreInfo = new List<PackExcelStoreInfo>();

    /// <summary>
    /// 存储数量
    /// </summary>
    private const int m_iPackStoreInfoCnt = 5;

    /// <summary>
    /// <para>0-普通模式</para>
    /// <para>1-存储模式</para>
    /// <para>2-获取模式</para>
    /// </summary>
    private int m_iMod = 0;

    /// <summary>
    /// 正在操作的
    /// </summary>
    private int m_iIndexOfControlItem = -1;

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("要打表的Excel数量（0表示All）:");
        m_iPackTablesCount = EditorGUILayout.IntField(m_iPackTablesCount);

        EditorGUILayout.EndHorizontal();

        if (m_iPackTablesCount < 0)
        {
            m_iPackTablesCount = 0;
        }

        if (m_iPackTablesCount > 10)//手动就不要太多了
        {
            m_iPackTablesCount = 10;
        }

        if (m_iPackTablesCount < m_listPackExcelInfo.Count)
        {
            while (m_listPackExcelInfo.Count > m_iPackTablesCount)
            {
                m_listPackExcelInfo.RemoveAt(m_listPackExcelInfo.Count - 1);
            }
        }
        else if (m_iPackTablesCount > m_listPackExcelInfo.Count)
        {
            for (int i = m_listPackExcelInfo.Count; i < m_iPackTablesCount; i++)
            {
                m_listPackExcelInfo.Add(new PackExcelInfo()
                {
                    m_strExcelName = string.Empty,
                });
            }
        }

        int index = 0;
        foreach (PackExcelInfo packInfo in m_listPackExcelInfo)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Excel" + index + "名称（忽略大小写）：");
            packInfo.m_strExcelName = EditorGUILayout.TextField(packInfo.m_strExcelName, GUILayout.Width(155));
            if (GUILayout.Button("文件选择"))
            {
                string strTmp = EditorUtility.OpenFilePanel("选择Excel表", "", "");
                if (!string.IsNullOrEmpty(strTmp))
                {
                    packInfo.m_strExcelName = Path.GetFileNameWithoutExtension(strTmp);
                }
            }

            if (m_iMod == 2 && m_iIndexOfControlItem == index)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("取消获取"))
                {
                    m_iIndexOfControlItem = -1;
                    m_iMod = 0;
                }
                GUI.color = Color.white;
            }
            else if (m_listPackStoreInfo.Exists((x) => !string.IsNullOrEmpty(x.m_strExcelName)))
            {
                if (GUILayout.Button("存储获取"))
                {
                    m_iIndexOfControlItem = index;
                    m_iMod = 2;
                }
            }
            else
            {
                GUILayout.Box("存储为空");
            }

            if (!string.IsNullOrEmpty(packInfo.m_strExcelName))
            {
                if (m_iMod == 1 && m_iIndexOfControlItem == index)
                {
                    GUI.color = Color.red;
                    if (GUILayout.Button("取消存储"))
                    {
                        m_iIndexOfControlItem = -1;
                        m_iMod = 0;
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    if (GUILayout.Button("存储这个"))
                    {
                        m_iIndexOfControlItem = index;
                        m_iMod = 1;
                    }
                }
            }
            else
            {
                GUILayout.Box("内容为空");
            }

            EditorGUILayout.EndHorizontal();

            index++;
        }

        m_bPackTableToBinary = EditorGUILayout.ToggleLeft("PackTableToBinary", m_bPackTableToBinary);
        m_bCompileTableProtoFile = EditorGUILayout.ToggleLeft("CompileTableProtoFile", m_bCompileTableProtoFile);
        m_bCompileCommonProtoFile = EditorGUILayout.ToggleLeft("CompileCommonProtoFile", m_bCompileCommonProtoFile);

        if (GUILayout.Button("开始打表"))
        {
            m_iPackTableToBinaryCnt = 0;
            m_iCompileTableProtoFileCnt = 0;
            m_iCompileCommonProtoFileCnt = 0;

            if (m_bPackTableToBinary)
            {
                PackTables();
            }

            if (m_bCompileTableProtoFile || m_bCompileCommonProtoFile)
            {
                DoComplieTableProtoFile(true);
            }
        }

        if (m_iPackTablesCount > 0)
        {
            if (m_listPackStoreInfo.Count == 0)
            {
                for (int i = 0; i < m_iPackStoreInfoCnt; i++)
                {
                    m_listPackStoreInfo.Add(new PackExcelStoreInfo()
                    {
                        m_strExcelName = PlayerPrefs.GetString("EditorTablePackerExcelNameStore" + i, string.Empty),
                    });
                }
            }

            GUILayout.Label("存储的Excel名称:");

            for (int i = 0; i < m_iPackStoreInfoCnt; i++)
            {
                PackExcelStoreInfo info = m_listPackStoreInfo[i];

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("存储" + i + "：" + info.m_strExcelName, GUILayout.MinWidth(300));

                if (m_iMod == 1)
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("存这里"))
                    {
                        if (m_iIndexOfControlItem >= 0 && m_iIndexOfControlItem < m_listPackExcelInfo.Count)
                        {
                            m_listPackStoreInfo[i].m_strExcelName = m_listPackExcelInfo[m_iIndexOfControlItem].m_strExcelName;
                            PlayerPrefs.SetString("EditorTablePackerExcelNameStore" + i, m_listPackStoreInfo[i].m_strExcelName);
                        }

                        m_iIndexOfControlItem = -1;
                        m_iMod = 0;
                    }
                    GUI.color = Color.white;
                }
                else if (m_iMod == 2)
                {
                    if (!string.IsNullOrEmpty(m_listPackStoreInfo[i].m_strExcelName))
                    {
                        GUI.color = Color.green;
                        if (GUILayout.Button("拿这个"))
                        {
                            if (m_iIndexOfControlItem >= 0 && m_iIndexOfControlItem < m_listPackExcelInfo.Count)
                            {
                                m_listPackExcelInfo[m_iIndexOfControlItem].m_strExcelName = m_listPackStoreInfo[i].m_strExcelName;
                            }

                            m_iIndexOfControlItem = -1;
                            m_iMod = 0;
                        }
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUILayout.Button("空内容");
                    }
                }

                EditorGUILayout.EndHorizontal();

                index++;
            }
        }

        GUILayout.Label("打表统计:");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("PackTableToBinary:" + m_iPackTableToBinaryCnt);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("CompileTableProtoFile:" + m_iCompileTableProtoFileCnt);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("CompileCommonProtoFile:" + m_iCompileCommonProtoFileCnt);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// string的ToLower似乎要求纯字母，于是自己写了一个
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string MyToLower(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder(str);

        for (int i = 0, imax = sb.Length; i < imax; i++)
        {
            if (sb[i] >= 'A' && sb[i] <= 'Z')
            {
                sb[i] = (char)('a' + (sb[i] - 'A'));
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 检查名字
    /// </summary>
    /// <param name="strName"></param>
    /// <param name="bProto"></param>
    /// <returns></returns>
    private static bool CheckName(string strName, bool bProto = false)
    {
        if (m_listPackExcelInfo == null || m_listPackExcelInfo.Count == 0)
        {
            return true;
        }

        PackExcelInfo info = null;
        info = m_listPackExcelInfo.Find((x) => (MyToLower(bProto ? "c_table_" + x.m_strExcelName : x.m_strExcelName)).Equals(MyToLower(strName)));
        if (info == null)
        {
            return false;
        }

        return true;
    }

    #endregion

    [MenuItem("Assets/Pack All Tables To Binary")]
    public static void PackTables()
    {
        string strLocalizeTableWorldCollectPath = m_strTablePath + "table_output/chinese.txt";
        if (File.Exists(strLocalizeTableWorldCollectPath))
        {
            File.Delete(strLocalizeTableWorldCollectPath);
        }

        dir = Directory.GetCurrentDirectory();

        tablePath = dir + string.Format(@"\{0}\table\", m_strTableFileName);
        table_outputPath = dir + string.Format(@"\{0}\table_output\", m_strTableFileName);
        table_toolsPath = dir + string.Format(@"\{0}\table_tools\", m_strTableFileName);
        protoPath = dir + string.Format(@"\{0}\proto\", m_strTableFileName);

#if UNITY_WEBPLAYER
        table_outputStreamingPath = dir + @"\AssetBundles\Table\";
#elif UNITY_ANDROID
        table_outputStreamingPath = dir + @"\Assets\StreamingAssets\Table\";
#elif UNITY_IPHONE
        table_outputStreamingPath = dir + @"\Assets\StreamingAssets\Table\";
#endif

        if (!Directory.Exists(table_outputStreamingPath))
        {
            Directory.CreateDirectory(table_outputStreamingPath);
        }

#if DEBUG_CODE
        if (/*!dir.Contains("branches") &&*/!dir.Contains("tags"))
        {
            Directory.SetCurrentDirectory(tablePath);
            Process p = Process.Start("TortoiseProc.exe", @"/command:update /path:"".\"" /closeonend:1");
            p.WaitForExit();

            Directory.SetCurrentDirectory(table_toolsPath);
            p = Process.Start("TortoiseProc.exe", @"/command:update /path:"".\"" /closeonend:1");
            p.WaitForExit();

            Directory.SetCurrentDirectory(protoPath);
            p = Process.Start("TortoiseProc.exe", @"/command:update /path:"".\"" /closeonend:1");
            p.WaitForExit();
        }
#endif

        try
        {
            Directory.SetCurrentDirectory(protoPath);

            GeneratePythonFile("c_table_*");

            GeneratePythonFile("common_*");

            Directory.SetCurrentDirectory(table_toolsPath);

            Environment.SetEnvironmentVariable("DEFAULT_PROTO_PREFIX", "c_table_");

            Environment.SetEnvironmentVariable("DEFAULT_OUTPUT_PREFIX", "");

            foreach (TableLoader.TableDesc desc in TableLoader.Instance.tableDescList)
            {
                if (false == CheckName(desc.excelName))
                {
                    continue;
                }

                m_iPackTableToBinaryCnt++;

                string param = "";
                string bytesName = "";
                if (string.IsNullOrEmpty(desc.outFileName))
                {
                    param = string.Format(@"table_writer.py ""{0}""", desc.tableName);
                    bytesName = desc.tableName.ToLower();
                }
                else
                {
                    param = string.Format(@"table_writer.py -s {0} -m {1} -o {2} {3}", desc.sheetIndex, desc.proto_message_name, desc.outFileName, desc.excelName);
                    bytesName = desc.outFileName;
                }

                if (CallProcess("python.exe", param))
                {
                    if (CallProcess("Encryptor.exe", table_outputPath + bytesName + ".tbl"))
                    {
                        File.Copy(table_outputPath + bytesName + ".tbl", table_outputStreamingPath + bytesName + ".bytes", true);
                        File.Delete(table_outputPath + bytesName + ".tbl");
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("encryptor table error !!! ");
                        Directory.SetCurrentDirectory(dir);
                        return;
                    }
                }
                else
                {
                    Directory.SetCurrentDirectory(dir);
                    return;
                }
            }

            Directory.SetCurrentDirectory(dir);

            UnityEngine.Debug.Log("(.)  Success  (.)");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
            Directory.SetCurrentDirectory(dir);
        }
    }

    [MenuItem("Assets/Compile Table Proto File")]
    public static void ComplieTableProtoFile()
    {
        DoComplieTableProtoFile();
    }

    /// <summary>
    /// 编译proto文件
    /// </summary>
    /// <param name="bFromWindow">是否来自窗体操作</param>
    private static void DoComplieTableProtoFile(bool bFromWindow = false)
    {
        dir = Directory.GetCurrentDirectory();
        protoPath = dir + string.Format(@"\{0}\proto\", m_strTableFileName);

        Directory.SetCurrentDirectory(protoPath);

        try
        {
#if DEBUG_CODE
            if (!dir.Contains("branches") && !dir.Contains("tags"))
            {
                Process p = Process.Start("TortoiseProc.exe", @"/command:update /path:"".\"" /closeonend:1");
                p.WaitForExit();
            }
#endif
            string[] fileNames = Directory.GetFiles(@".\");

            bool bCompileCommonProto = true;
            if (bFromWindow)
            {
                bCompileCommonProto = m_bCompileCommonProtoFile;
            }

            if (bCompileCommonProto)
            {
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains("common_") && fileName.Contains(".proto"))
                    {
                        string name = fileName.Substring(0, fileName.LastIndexOf('.'));
                        name = name.Replace(".\\", "");
                        m_iCompileCommonProtoFileCnt++;
                        if (ProcessMsgProto(name) == false)
                        {
                            Directory.SetCurrentDirectory(dir);
                            return;
                        }
                    }
                }
            }

            foreach (string fileName in fileNames)
            {
                if (fileName.Contains("c_table_") && fileName.Contains(".proto") && CheckName(Path.GetFileNameWithoutExtension(fileName), true))
                {
                    string name = fileName.Substring(0, fileName.LastIndexOf('.'));
                    name = name.Replace(".\\", "");
                    m_iCompileTableProtoFileCnt++;
                    if (ProcessTableProto(name) == false)
                    {
                        Directory.SetCurrentDirectory(dir);
                        return;
                    }
                }
            }

            CallProcess("python.exe", protoPath + "XMLDeleter.py " + dir + @"\Assets\Scripts\Table\proto_gen\");
            //CallProcess("python.exe", protoPath + "XMLDeleter.py " + dir + @"\Assets\Code\Network\proto_gen\");
            Directory.SetCurrentDirectory(dir);

            UnityEngine.Debug.Log("(.)   Success   (.)");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
            Directory.SetCurrentDirectory(dir);
        }
    }

    /*
    [MenuItem("Assets/Complie MSG Proto File")]
    public static void ComplieMSGProtoFile()
    {
        dir = Directory.GetCurrentDirectory();
        protoPath = dir + @"\common\proto\";

        Directory.SetCurrentDirectory(protoPath);

        try
        {
            if (!dir.Contains("branches") && !dir.Contains("tags"))
            {
                Process p = Process.Start("TortoiseProc.exe", @"/command:update /path:"".\"" /closeonend:1");
                p.WaitForExit();
            }

            ProcessMsgProto("dir");

            string[] fileNames = Directory.GetFiles(@".\");
            foreach (string fileName in fileNames)
            {
                // 只读command_user_  common_
                if ((fileName.Contains("command_user") && fileName.Contains(".proto"))
                    || (fileName.Contains("common_") && fileName.Contains(".proto"))

                    )
                {
                    string name = fileName.Substring(0, fileName.LastIndexOf('.'));

                    ProcessMsgProto(name);
                }
            }
            CallProcess("python.exe", protoPath + "XMLDeleter.py " + dir + @"\Assets\Code\Network\proto_gen\");
            Directory.SetCurrentDirectory(dir);
            UnityEngine.Debug.Log("(.)   Success  (.)");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);

            Directory.SetCurrentDirectory(dir);
        }
    }
    */

    private static string getSelectedPath(UnityEngine.Object obj)
    {
        if (obj == null && Selection.activeObject == null)
        {
            UnityEngine.Debug.LogError("Nothing selected!");
            return "";
        }
        string path = AssetDatabase.GetAssetPath(obj == null ? Selection.activeObject : obj);
        return path;
    }

    //[@MenuItem("Assets/InitControl")]
    //public static void InitControl ()
    //{
    //    UnityEngine.Object[] selection = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.DeepAssets );

    //    string strPath = string.Empty;

    //    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile( @"Library/ScriptAssemblies/Assembly-CSharp.dll" );

    //    int itNum = 0;

    //    foreach ( UnityEngine.Object obj in selection )
    //    {
    //        strPath = getSelectedPath( obj );

    //        if ( strPath.Contains( "Assets/Data/UI/Resources/Windows" ) == false )
    //        {
    //            break;
    //        }

    //        GameObject inst = obj as GameObject;

    //        if ( inst == null )
    //        {
    //            UnityEngine.Debug.LogError("GameObject is null");
    //            continue;
    //        }

    //        GameObject go = UnityEngine.Object.Instantiate( inst ) as GameObject;
    //        go.name = inst.name;

    //        UnityEngine.Debug.LogError( "GameObjectName:" + go.name );

    //        Component component = go.GetComponent( go.name );

    //        if ( component != null )
    //        {
    //            System.Type t = assembly.GetType( go.name );

    //            if ( t == null )
    //            {
    //                //UnityEngine.Debug.LogError( "    Type is null" );
    //                continue;
    //            }

    //            System.Reflection.MethodInfo method = t.GetMethod( "InitControl" );

    //            if ( method != null)
    //            {
    //                itNum++;
    //                UnityEngine.Debug.LogError( "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++Method is ok " + itNum );
    //                method.Invoke( component , null );

    //                PrefabUtility.ReplacePrefab( go , inst , ReplacePrefabOptions.ConnectToPrefab );
    //                UnityEngine.Object.DestroyImmediate( go );
    //            }
    //            else
    //            {
    //                //UnityEngine.Debug.LogError( "    Method is null" );
    //                UnityEngine.Object.DestroyImmediate( go );
    //            }
    //        }
    //        else
    //        {
    //            //UnityEngine.Debug.LogError( "    Component is null" );
    //            UnityEngine.Object.DestroyImmediate( go );
    //        }
    //    }
    //    AssetDatabase.Refresh();
    //    AssetDatabase.SaveAssets();
    //}
}