using UnityEngine;
using System.Collections;

/// <summary>
/// 触发器模型加载器
/// </summary>
public class TriggerModelLoader : MonoBehaviour
{
    /// <summary>
    /// 触发器
    /// </summary>
    private BaseTrigger m_Trigger;

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    public static TriggerModelLoader Get(BaseTrigger trigger)
    {
        TriggerModelLoader loader = null;
        if (trigger == null)
        {
            return loader;
        }

        loader = trigger.gameObject.AddComponent<TriggerModelLoader>();
        loader.m_Trigger = trigger;

        return loader;
    }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="modelName">模型名</param>
    public void Load(string modelName)
    {

    }
}
