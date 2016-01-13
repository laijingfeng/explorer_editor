using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TriggerRange))]
public class TriggerRangeInspector : TriggerBaseInspector
{
    /// <summary>
    /// 范围触发器
    /// </summary>
    public TriggerRange m_TriggerRange;

    /// <summary>
    /// 绘制属性
    /// </summary>
    public override void DrawAttr()
    {
        base.DrawAttr();

        m_TriggerRange = m_BaseTrigger as TriggerRange;

        m_TriggerRange.transform.tag = "RangeTrigger";

        if (m_TriggerRange.GetComponent<BoxCollider2D>() == null)
        {
            m_TriggerRange.gameObject.AddComponent<BoxCollider2D>();
        }

        GUI.color = Color.green;
        GUILayout.Label("在BoxCollider2D中设置范围");
        GUI.color = Color.white;

        if (m_TriggerRange.transform.childCount <= 0)
        {
            GUI.color = Color.white;
            GUILayout.Label("隐形触发器，可添加item子结点成为可视");
            GUI.color = Color.white;
        }
        else if (m_TriggerRange.transform.childCount > 1)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("只能添加一个item子结点");
            GUI.color = Color.white;
        }
        else
        {
            if (m_BaseTrigger.transform.GetChild(0).transform.name.Contains("item_"))
            {
                GUI.color = Color.green;
                GUILayout.Label("可视触发器");
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("子结点需要是item");
                GUI.color = Color.white;
            }   
        }
    }
}
