using UnityEngine;
using System.Collections;

/// <summary>
/// Boss触发器
/// </summary>
public class TriggerBoss : TriggerBase
{
    /// <summary>
    /// Boss名称
    /// </summary>
    public string m_strBossName;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config"></param>
    public override void Init(TriggerBase config)
    {
        base.Init(config);

        TriggerBoss boss = config as TriggerBoss;

        m_strBossName = boss.m_strBossName;
    }

    void Awake()
    {
        if (this.transform.childCount > 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 触发
    /// </summary>
    public override void OnTrigger()
    {
        base.OnTrigger();

        if (this.transform.childCount > 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            if (!string.IsNullOrEmpty(m_strBossName))
            {
                EntryManager.Instance.CreateBoss(m_strBossName, this.transform);
            }
        }
    }
}
