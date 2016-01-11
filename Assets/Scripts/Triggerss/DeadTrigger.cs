using UnityEngine;
using System.Collections;

/// <summary>
/// 死亡触发器
/// </summary>
public class DeadTrigger : TriggerBase 
{
    /// <summary>
    /// 模型名
    /// </summary>
    protected string m_strModelName;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config">配置</param>
    public override void Init(TriggerBaseConfig config)
    {
        base.Init(config);

        m_strModelName = config.m_strDeadTriggerBossName;

        Resource res = ResourceManager.Instance.LoadResource(string.Format("Boss/{0}.unity3d", m_strModelName), false);
        res.onLoaded += OnAvatarLoaded;
    }

    /// <summary>
    /// 消亡
    /// </summary>
    public override void Dead()
    {
        OnTrigger();
        base.Dead();
    }

    /// <summary>
    /// 加载成功
    /// </summary>
    /// <param name="res"></param>
    private void OnAvatarLoaded(Resource res)
    {
        GameObject go = UnityEngine.Object.Instantiate(res.MainAsset) as GameObject;
        go.transform.parent = this.transform;
        go.transform.localPosition = Vector3.zero;
    }
}
