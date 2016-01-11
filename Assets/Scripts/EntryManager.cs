using UnityEngine;
using System.Collections;

/// <summary>
/// 入口
/// </summary>
public class EntryManager : SingletonMono<EntryManager>
{
    /// <summary>
    /// 创建英雄
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public GameObject CreateHero(UnityEngine.Object obj)
    {
        GameObject hero = UnityEngine.Object.Instantiate(obj) as GameObject;
        hero.name = "hero";
        hero.transform.parent = transform;
        return hero;
    }

    /// <summary>
    /// 创造道具
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="father">空则附加在EntryManager</param>
    public void CreateBoss(string itemName, Transform father = null)
    {
        Resource res = ResourceManager.Instance.LoadResource(string.Format("Boss/{0}.unity3d", itemName), false);
        res.customData = father;
        res.onLoaded += OnItemLoaded;
    }

    /// <summary>
    /// 创造道具
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="father">空则附加在EntryManager</param>
    public void CreateItem(string itemName,Transform father = null)
    {
        Resource res = ResourceManager.Instance.LoadResource(string.Format("Item/{0}.unity3d", itemName), false);
        res.customData = father;
        res.onLoaded += OnItemLoaded;
    }

    /// <summary>
    /// 道具加载成功
    /// </summary>
    /// <param name="res"></param>
    private void OnItemLoaded(Resource res)
    {
        Transform father = res.customData as Transform;
        if (father == null)
        {
            father = this.transform;
        }

        GameObject go = UnityEngine.Object.Instantiate(res.MainAsset) as GameObject;
        go.transform.parent = father;
        go.transform.localPosition = Vector3.zero;
    }

    public BaseTrigger CreateTrigger(BaseTrigger config)
    {
        GameObject go = new GameObject(string.Format("Trigger_{0}", config.m_iUniqueID));
        go.transform.parent = transform;
        go.transform.position = config.transform.position;

        BaseTrigger tt = null;

        if (config is TriggerTimer)
        {
            tt = go.AddComponent<TriggerTimer>();
        }
        else if(config is TriggerRange)
        {
            tt = go.AddComponent<TriggerRange>();
        }
        else if (config is TriggerBoss)
        {
            tt = go.AddComponent<TriggerBoss>();
        }

        tt.Init(config);

        return tt;
    }

    /// <summary>
    /// 创建触发器
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public TriggerBase CreateTrigger(TriggerBaseConfig config)
    {
        TriggerBase trigger = null;
        GameObject go = new GameObject(string.Format("{0}_{1}", config.m_Type, config.m_iUniqueID));
        go.transform.parent = transform;
        go.transform.position = config.transform.position;

        switch (config.m_Type)
        {
            case TriggerBase.TriggerType.DeadTrigger:
                {
                    trigger = go.AddComponent<DeadTrigger>();
                }
                break;
            case TriggerBase.TriggerType.RangeTrigger:
                {
                    trigger = go.AddComponent<RangeTrigger>();
                }
                break;
            case TriggerBase.TriggerType.TimerTrigger:
                {
                    trigger = go.AddComponent<TimerTrigger>();
                }
                break;
        }

        trigger.Init(config);

        return trigger;
    }

    public void Clear()
    {
        Util.DestroyAllChildrenImmediate(gameObject);
    }
}
