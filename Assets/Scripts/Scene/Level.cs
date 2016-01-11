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
    public List<BaseTrigger> m_listTrigger;

    /// <summary>
    /// ID->BaseTrigger
    /// </summary>
    private Dictionary<int, BaseTrigger> m_dicID2Trigger = new Dictionary<int, BaseTrigger>();

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CreateTrigger();
    }

    /// <summary>
    /// 创建触发器
    /// </summary>
    private void CreateTrigger()
    {
        BaseTrigger tt;
        bool done = true;
        do
        {
            done = true;
            foreach (BaseTrigger t in m_listTrigger)
            {
                if (t.m_Father == null)
                {
                    tt = EntryManager.Instance.CreateTrigger(t);
                    m_dicID2Trigger.Add(tt.m_iUniqueID, tt);
                }
                else
                {
                    BaseTrigger ff = null;
                    if (m_dicID2Trigger.TryGetValue(t.m_iUniqueID, out ff))
                    {
                        t.m_Father = ff;
                        tt = EntryManager.Instance.CreateTrigger(t);
                        m_dicID2Trigger.Add(tt.m_iUniqueID, tt);
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
