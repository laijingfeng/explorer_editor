using UnityEngine;
using System.Collections;

/// <summary>
/// 范围触发器
/// </summary>
public class TriggerRange : TriggerBase
{
    /// <summary>
    /// 触发盒子
    /// </summary>
    public BoxCollider2D m_bcTriggerBox;

    /// <summary>
    /// <para>物体名</para>
    /// <para>空则是隐形的</para>
    /// </summary>
    public string m_strItemName;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config"></param>
    public override void Init(TriggerBase config)
    {
        base.Init(config);

        TriggerRange range = config as TriggerRange;

        m_bcTriggerBox = range.m_bcTriggerBox;
        m_strItemName = range.m_strItemName;
    }

    void Awake()
    {
        if (this.transform.GetComponent<BoxCollider2D>() != null)
        {
            this.transform.GetComponent<BoxCollider2D>().enabled = false;
        }

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

        if (this.transform.GetComponent<BoxCollider2D>() == null)
        {
            BoxCollider2D box = transform.gameObject.AddComponent<BoxCollider2D>();
            box.center = m_bcTriggerBox.center;
            box.size = m_bcTriggerBox.size;
            box.isTrigger = m_bcTriggerBox.isTrigger;
            transform.tag = "RangeTrigger";
        }
        else
        {
            this.transform.GetComponent<BoxCollider2D>().enabled = true;
        }

        if (this.transform.childCount > 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            if (!string.IsNullOrEmpty(m_strItemName))
            {
                EntryManager.Instance.CreateItem(m_strItemName, this.transform);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            OnFinish();
        }
    }
}
