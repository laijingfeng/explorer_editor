using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 触发器配置
/// </summary>
public class TriggerBaseConfig : MonoBehaviour
{
    /// <summary>
    /// 父亲
    /// </summary>
    public TriggerBaseConfig m_Father;

    /// <summary>
    /// 类型
    /// </summary>
    public TriggerBase.TriggerType m_Type;

    /// <summary>
    /// 是否是通关触发器
    /// </summary>
    public bool m_bIsFinishTrigger;

    #region 时间触发器

    /// <summary>
    /// 时间触发器时间
    /// </summary>
    public float m_fTimerTime;

    #endregion

    #region  范围触发器

    /// <summary>
    /// 触发盒子
    /// </summary>
    public BoxCollider2D m_bcTriggerBox;

    /// <summary>
    /// <para>物体名</para>
    /// <para>空则是隐形的</para>
    /// </summary>
    public string m_strRangeTriggerItemName;

    #endregion

    #region 死亡触发器

    /// <summary>
    /// 模型名
    /// </summary>
    public string m_strDeadTriggerBossName;

    #endregion

    /// <summary>
    /// 唯一ID
    /// </summary>
    [HideInInspector]
    public int m_iUniqueID;

    /// <summary>
    /// 触发角色
    /// </summary>
    [HideInInspector]
    public List<int> m_listTrigger = new List<int>();

    void OnDrawGizmos()
    {
        switch (m_Type)
        {
            case TriggerBase.TriggerType.TimerTrigger:
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(transform.position, 0.1f);
                    Gizmos.color = Color.white;
                }
                break;
            case TriggerBase.TriggerType.DeadTrigger:
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(transform.position, 0.1f);
                    Gizmos.color = Color.white;
                }
                break;
            case TriggerBase.TriggerType.RangeTrigger:
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(transform.position, 0.1f);
                    Gizmos.color = Color.white;
                }
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (m_Father != null)
        {
            Gizmos.DrawLine(m_Father.transform.position, transform.position);
        }
    }
}
