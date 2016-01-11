using UnityEngine;
using System.Collections;

/// <summary>
/// 范围触发器
/// </summary>
public class RangeTrigger : TriggerBase
{
    /// <summary>
    /// 触发盒子
    /// </summary>
    protected BoxCollider2D m_bcTriggerBox;

    /// <summary>
    /// <para>物体名</para>
    /// <para>空则是隐形的</para>
    /// </summary>
    protected string m_strItemName;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config">配置</param>
    public override void Init(TriggerBaseConfig config)
    {
        base.Init(config);

        m_bcTriggerBox = config.m_bcTriggerBox;
        m_strItemName = config.m_strRangeTriggerItemName;

        BoxCollider2D box = transform.gameObject.AddComponent<BoxCollider2D>();
        box.center = m_bcTriggerBox.center;
        box.size = m_bcTriggerBox.size;
        box.isTrigger = m_bcTriggerBox.isTrigger;

        transform.tag = "RangeTrigger";

        if (!string.IsNullOrEmpty(m_strItemName))
        {
            EntryManager.Instance.CreateItem(m_strItemName, this.transform);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            OnTrigger();
            Dead();
        }
    }
}
