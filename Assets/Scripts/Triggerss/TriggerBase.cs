using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 触发器
/// </summary>
public class TriggerBase : MonoBehaviour
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    protected int m_iUniqueID;

    /// <summary>
    /// 变换
    /// </summary>
    protected Transform m_Transform;

    /// <summary>
    /// 后续触发列表
    /// </summary>
    protected List<int> m_listTrigger = new List<int>();

    /// <summary>
    /// 类型
    /// </summary>
    protected TriggerType m_Type;

    /// <summary>
    /// 是否是通关触发器
    /// </summary>
    protected bool m_bIsFinishTrigger;

    /// <summary>
    /// 触发器类型
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// 时间触发器
        /// </summary>
        TimerTrigger = 0,

        /// <summary>
        /// 死亡触发器
        /// </summary>
        DeadTrigger,

        /// <summary>
        /// 范围触发器
        /// </summary>
        RangeTrigger,
    }

    /// <summary>
    /// 构造
    /// </summary>
    public TriggerBase()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config"></param>
    public virtual void Init(TriggerBaseConfig config)
    {
        m_Transform = config.transform;
        m_iUniqueID = config.m_iUniqueID;
        m_Type = config.m_Type;
        m_listTrigger = config.m_listTrigger;
    }

    /// <summary>
    /// 触发
    /// </summary>
    protected virtual void OnTrigger()
    {
        //Level.Instance.CreateTriggers(m_listTrigger);
    }

    /// <summary>
    /// 死亡回调
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    public delegate void OnDead(int uniqueID);

    /// <summary>
    /// 死亡回调
    /// </summary>
    public event OnDead onDead;

    /// <summary>
    /// 消亡，删除结点
    /// </summary>
    public virtual void Dead()
    {
        if (onDead != null)
        {
            onDead(m_iUniqueID);
        }

        if (this != null)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
