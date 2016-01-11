using UnityEngine;
using System.Collections;

/// <summary>
/// 时间触发器
/// </summary>
public class TriggerTimer : BaseTrigger
{
    /// <summary>
    /// 等待时长
    /// </summary>
    public float m_fTimerTime;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config"></param>
    public override void Init(BaseTrigger config)
    {
        base.Init(config);

        TriggerTimer timer = config as TriggerTimer;

        m_fTimerTime = timer.m_fTimerTime;
    }

    /// <summary>
    /// 触发
    /// </summary>
    public override void OnTrigger()
    {
        base.OnTrigger();
        StopCoroutine("CountTime");
        StartCoroutine("CountTime");
    }

    /// <summary>
    /// 计时
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountTime()
    {
        yield return new WaitForSeconds(m_fTimerTime);
        OnFinish();
        yield return null;
    }
}
