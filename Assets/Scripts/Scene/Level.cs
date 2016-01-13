using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 关卡
/// </summary>
public class Level : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    private static Level m_instance;

    /// <summary>
    /// 单例
    /// </summary>
    public static Level Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject instGo = GameObject.FindGameObjectWithTag("Level");
                if (instGo != null)
                {
                    m_instance = instGo.GetComponent<Level>();
                }
            }
            return m_instance;
        }
    }

    /// <summary>
    /// 主角位置
    /// </summary>
    public Vector3 m_PlayerPos;

    /// <summary>
    /// 触发器
    /// </summary>
    public List<TriggerBase> m_listTrigger;

    /// <summary>
    /// ID->BaseTrigger
    /// </summary>
    private Dictionary<int, TriggerBase> m_dicID2Trigger = new Dictionary<int, TriggerBase>();

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CreateTrigger();
    }

    /// <summary>
    /// 触发器结束
    /// </summary>
    public delegate void OnTriggerFinish(TriggerBase trigger);

    /// <summary>
    /// 触发器结束事件
    /// </summary>
    public event OnTriggerFinish onTriggerFinish = null;

    /// <summary>
    /// 触发器结束
    /// </summary>
    /// <param name="trigger"></param>
    private void OnTriggerDead(TriggerBase trigger)
    {
        if (onTriggerFinish != null)
        {
            onTriggerFinish(trigger);
        }
    }

    /// <summary>
    /// 创建触发器
    /// </summary>
    private void CreateTrigger()
    {
        TriggerBase trigger;
        bool done = true;
        do
        {
            done = true;
            foreach (TriggerBase config in m_listTrigger)
            {
                if (config.m_Father == null)
                {
                    trigger = EntryManager.Instance.CreateTrigger(config);
                    trigger.onTriggerFinish += OnTriggerDead;
                    m_dicID2Trigger.Add(trigger.m_iUniqueID, trigger);
                }
                else
                {
                    TriggerBase father = null;
                    if (m_dicID2Trigger.TryGetValue(config.m_iUniqueID, out father))
                    {
                        config.m_Father = father;
                        trigger = EntryManager.Instance.CreateTrigger(config);
                        trigger.onTriggerFinish += OnTriggerDead;
                        m_dicID2Trigger.Add(trigger.m_iUniqueID, trigger);
                    }
                    else
                    {
                        done = false;
                    }
                }
            }
        } while (done);
    }
}
