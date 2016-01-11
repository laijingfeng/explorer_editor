using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景背景编辑器
/// </summary>
public class SceneBkgEditor : EditorWindow
{
    /// <summary>
    /// 相等评判
    /// </summary>
    private const float EPS_EQUAL = 0.5f;

    /// <summary>
    /// 重叠量
    /// </summary>
    private const float EPS_OVERLAP = 0.01f;

    /// <summary>
    /// 显示
    /// </summary>
    [MenuItem("SceneEditor/EditorBkgScene")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneBkgEditor));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("拼接选中的场景"))
        {
            Process(Selection.transforms);
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 场景背景列
    /// </summary>
    private List<SceneBkgCell> m_listSceneBkgCell = new List<SceneBkgCell>();

    /// <summary>
    /// 进行处理
    /// </summary>
    /// <param name="trans"></param>
    private void Process(Transform[] trans)
    {
        if (trans == null
            || trans.Length <= 0)
        {
            EditorUtility.DisplayDialog("提示", "没有选中物体", "确定");
            return;
        }

        List<SpriteRenderer> sprites = new List<SpriteRenderer>();

        foreach (Transform tf in trans)
        {
            sprites.AddRange(tf.GetComponentsInChildren<SpriteRenderer>(true));
        }

        if (sprites.Count <= 0)
        {
            EditorUtility.DisplayDialog("提示", "没有选中场景", "确定");
            return;
        }

        m_listSceneBkgCell.Clear();

        foreach (SpriteRenderer sr in sprites)
        {
            m_listSceneBkgCell.Add(new SceneBkgCell(sr.transform));
        }

        DoWork(m_listSceneBkgCell);
    }

    /// <summary>
    /// 拼接一行
    /// </summary>
    /// <param name="list"></param>
    /// <param name="pre"></param>
    private void DoWork(List<SceneBkgCell> list, SceneBkgCell pre = null)
    {
        if (list == null
            || list.Count <= 0)
        {
            return;
        }

        SortSceneBkgCell(list);

        SceneBkgCell now;

        for (int i = 0, imax = list.Count; i < imax; i++)
        {
            now = list[i];

            if (now.m_bWorked)
            {
                continue;
            }

            if (pre != null)
            {
                if (i == 0)
                {
                    now.m_vtPos.x = pre.m_vtPos.x - pre.m_vtSizeHalf.x + now.m_vtSizeHalf.x;
                    now.m_vtPos.y = pre.m_vtPos.y + pre.m_vtSizeHalf.y + now.m_vtSizeHalf.y - EPS_OVERLAP;
                }
                else
                {
                    now.m_vtPos.x = pre.m_vtPos.x + pre.m_vtSizeHalf.x + now.m_vtSizeHalf.x - EPS_OVERLAP;
                    now.m_vtPos.y = pre.m_vtPos.y - pre.m_vtSizeHalf.y + now.m_vtSizeHalf.y;
                }
                now.m_vtPos.z = 0;
                now.m_tfTransform.position = now.m_vtPos;
                now.m_bWorked = true;
            }

            pre = now;

            DoWork(GetTop(list, now), now);
        }
    }

    /// <summary>
    /// 提取正上方的
    /// </summary>
    /// <param name="list"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    private List<SceneBkgCell> GetTop(List<SceneBkgCell> list, SceneBkgCell cell)
    {
        List<SceneBkgCell> retList = new List<SceneBkgCell>();

        foreach (SceneBkgCell sc in list)
        {
            if (sc.m_bWorked)
            {
                continue;
            }

            if ((cell.xMax - sc.xMin > 2 * EPS_OVERLAP && (sc.xMin >= cell.xMin || cell.xMin - sc.xMin <= 2 * EPS_OVERLAP))
               || sc.yMin > cell.yMin)
            {
                retList.Add(sc);
            }
        }

        return retList;
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <param name="list"></param>
    private void SortSceneBkgCell(List<SceneBkgCell> list)
    {
        SceneBkgCell tmp;

        for (int i = 0, imax = list.Count - 1; i < imax; i++)
        {
            for (int j = 0, jmax = list.Count - i - 1; j < jmax; j++)
            {
                if (IsBig(list[j], list[j + 1]))
                {
                    tmp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = tmp;
                }
            }
        }
    }

    /// <summary>
    /// 是否是大于
    /// </summary>
    /// <param name="cell1"></param>
    /// <param name="cell2"></param>
    /// <returns></returns>
    private bool IsBig(SceneBkgCell cell1, SceneBkgCell cell2)
    {
        if (Mathf.Abs(cell1.xMin - cell2.xMin) <= EPS_EQUAL)
        {
            return cell1.yMin > cell2.yMin;
        }

        return cell1.xMin > cell2.xMin;
    }

    /// <summary>
    /// 场景背景单元
    /// </summary>
    public class SceneBkgCell
    {
        /// <summary>
        /// 对应的Transform
        /// </summary>
        public Transform m_tfTransform;

        /// <summary>
        /// 大小的一半
        /// </summary>
        public Vector3 m_vtSizeHalf;

        /// <summary>
        /// 当前位置
        /// </summary>
        public Vector3 m_vtPos;

        /// <summary>
        /// 是否处理过
        /// </summary>
        public bool m_bWorked;

        public float xMin
        {
            get
            {
                return m_vtPos.x - m_vtSizeHalf.x;
            }
        }

        public float xMax
        {
            get
            {
                return m_vtPos.x + m_vtSizeHalf.x;
            }
        }

        public float yMin
        {
            get
            {
                return m_vtPos.y - m_vtSizeHalf.y;
            }
        }

        public float yMax
        {
            get
            {
                return m_vtPos.y + m_vtSizeHalf.y;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public SceneBkgCell()
        {
            m_tfTransform = null;
            m_vtSizeHalf = Vector3.one;
            m_vtPos = Vector3.zero;
            m_bWorked = false;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tf"></param>
        public SceneBkgCell(Transform tf)
        {
            m_tfTransform = tf;
            m_vtSizeHalf = tf.GetComponent<SpriteRenderer>().sprite.bounds.size * 0.5f;
            m_vtSizeHalf.x *= tf.lossyScale.x;
            m_vtSizeHalf.y *= tf.lossyScale.y;
            m_vtPos = tf.position;
            m_bWorked = false;
        }
    }
}
