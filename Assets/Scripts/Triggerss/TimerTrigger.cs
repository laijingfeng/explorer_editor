using UnityEngine;
using System.Collections;

/// <summary>
/// 时间触发器
/// </summary>
public class TimerTrigger : TriggerBase
{
    /// <summary>
    /// 等待时长
    /// </summary>
    protected float m_fTimerTime;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config">配置</param>
    public override void Init(TriggerBaseConfig config)
    {
        base.Init(config);

        m_fTimerTime = config.m_fTimerTime;

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
        OnTrigger();
        Dead();
        yield return null;
    }
}
