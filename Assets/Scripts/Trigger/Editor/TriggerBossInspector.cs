using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TriggerBoss))]
public class TriggerBossInspector : TriggerBaseInspector
{
    /// <summary>
    /// 绘制属性
    /// </summary>
    public override void DrawAttr()
    {
        base.DrawAttr();

        if(m_BaseTrigger.transform.childCount <= 0)
        {
            GUI.color = Color.red;
            GUILayout.Label("需要添加boss子结点");
            GUI.color = Color.white;
        }
        else if (m_BaseTrigger.transform.childCount > 1)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("只能添加一个boss子结点");
            GUI.color = Color.white;
        }
        else
        {
            if (m_BaseTrigger.transform.GetChild(0).transform.name.Contains("boss_"))
            {
                GUI.color = Color.green;
                GUILayout.Label("Boss子结点死亡时触发");
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("子结点需要是boss");
                GUI.color = Color.white;
            }
        }
    }
}
