using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TriggerBaseConfig))]
public class TriggerBaseConfigInspector : Editor
{
    protected TriggerBaseConfig m_TriggerBaseConfig;

    public override void OnInspectorGUI()
    {
        m_TriggerBaseConfig = target as TriggerBaseConfig;

        m_TriggerBaseConfig.m_Father = EditorGUILayout.ObjectField("触发者", m_TriggerBaseConfig.m_Father, typeof(TriggerBaseConfig), true) as TriggerBaseConfig;

        m_TriggerBaseConfig.m_Type = (TriggerBase.TriggerType)EditorGUILayout.EnumPopup("类型", m_TriggerBaseConfig.m_Type);

        m_TriggerBaseConfig.m_bIsFinishTrigger = EditorGUILayout.Toggle("是否通关触发器", m_TriggerBaseConfig.m_bIsFinishTrigger);

        switch (m_TriggerBaseConfig.m_Type)
        {
            case TriggerBase.TriggerType.TimerTrigger:
                {
                    m_TriggerBaseConfig.m_fTimerTime = EditorGUILayout.FloatField("等候时长(秒)", m_TriggerBaseConfig.m_fTimerTime);
                }
                break;
            case TriggerBase.TriggerType.DeadTrigger:
                {
                    GUILayout.BeginHorizontal();
                    if(m_TriggerBaseConfig.transform.childCount <= 0)
                    {
                        GUI.color = Color.red;
                        GUILayout.Label("需要添加Boss子结点");
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("Boss子结点死亡时触发");
                        GUI.color = Color.white;
                    }
                    GUILayout.EndHorizontal();
                }
                break;
            case TriggerBase.TriggerType.RangeTrigger:
                {
                    if(m_TriggerBaseConfig.GetComponent<BoxCollider2D>() == null)
                    {
                        GUI.color = Color.red;
                        GUILayout.Label("需要添加BoxCollider2D");
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("在BoxCollider2D中设置范围");
                        GUI.color = Color.white;
                    }

                    if (m_TriggerBaseConfig.transform.childCount <= 0)
                    {
                        GUI.color = Color.yellow;
                        GUILayout.Label("隐形触发器，可添加Item子结点成为可视");
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUI.color = Color.yellow;
                        GUILayout.Label("可视触发器");
                        GUI.color = Color.white;
                    }
                }
                break;
        }
    }
}
