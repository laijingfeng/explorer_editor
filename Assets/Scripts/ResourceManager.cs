using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// <para>相同两个资源中可以互相引用的属性</para>
/// <para>用类包起来作为引用</para>
/// <para>和旧版本有同步到2015.03.14</para>
/// </summary>
public class ResCommonAttr
{
    /// <summary>
    /// 是否加载失败
    /// </summary>
    private bool m_bIsLoadFaild;

    /// <summary>
    /// 文件名
    /// </summary>
    private string m_fileName;

    /// <summary>
    /// <para>是否加载完成</para>
    /// <para>成功/失败都是完成</para>
    /// </summary>
    private bool m_bIsLoaded;

    /// <summary>
    /// mainAsset
    /// </summary>
    public UnityEngine.Object m_mainAsset;

    /// <summary>
    /// AssetBundle
    /// </summary>
    public AssetBundle m_assetBundle;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="fileName"></param>
    public ResCommonAttr(string fileName)
    {
        m_fileName = fileName;
        m_bIsLoaded = false;
        m_bIsLoadFaild = false;
        m_mainAsset = null;
        m_assetBundle = null;
    }

    /// <summary>
    /// 是否加载失败
    /// </summary>
    public bool IsLoadFaild
    {
        get
        {
            return m_bIsLoadFaild;
        }

        set
        {
            m_bIsLoadFaild = value;
        }
    }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName
    {
        get
        {
            return m_fileName;
        }
    }

    /// <summary>
    /// 是否加载完成
    /// </summary>
    public bool IsLoaded
    {
        get
        {
            return m_bIsLoaded;
        }

        set
        {
            m_bIsLoaded = value;
        }
    }
}

/// <summary>
/// 资源
/// </summary>
public class Resource
{
    #region 路径处理

    public static string RES_PERSIST_PATH(string fileName)
    {
#if UNITY_ANDROID
        return Application.isEditor ? ("file://" + Application.dataPath + "/StreamingAssetsP/" + fileName) : 
                                      ("file://" + Application.persistentDataPath + "/" + fileName);
#elif UNITY_IPHONE
        return Application.isEditor ? ("file://" + Application.dataPath + "/StreamingAssetsP/" + fileName) : 
                                      ("file://" + Application.persistentDataPath + "/" + fileName);
#else  //UNITY_WEBPLAYER
        return RES_PATH(fileName);
#endif
    }

    /// <summary>
    /// 更新路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string RES_UPDATE_PATH(string fileName)
    {
#if UNITY_ANDROID
        //return Application.isEditor ? ("file://" + Application.dataPath + "/StreamingAssetsU/" + fileName) : 
        //                              ("file://" + Application.temporaryCachePath + "/" + fileName);
        // 某些平台(如小米)的清理工具会默认清理缓存目录，故对Android平台，更新路径设置为persistentDataPath
        return RES_PERSIST_PATH(fileName);
#elif UNITY_IPHONE
        return Application.isEditor ? ("file://" + Application.dataPath + "/StreamingAssetsU/" + fileName) : 
                                      ("file://" + Application.temporaryCachePath + "/" + fileName);
#else //UNITY_WEBPLAYER
        return RES_PATH(fileName);
#endif
    }

    public static string RefinePath(string filePath)
    {
#if UNITY_ANDROID
        if (filePath.Contains("jar:file://"))
        {
            filePath = filePath.Substring("jar:file://".Length);
        }
        else
#endif
            if (filePath.Contains("file://"))
            {
                filePath = filePath.Substring("file://".Length);
            }

        filePath = filePath.Replace("\\", "/");

        return filePath;
    }

    /// <summary>
    /// 资源路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string RES_PATH(string fileName)
    {
#if UNITY_ANDROID
        return Application.isEditor ? ("file://" + Application.dataPath + "/StreamingAssets/" + fileName) :
                                      ("jar:file://" + Application.dataPath + "!/assets/" + fileName);

#elif UNITY_IPHONE
        return Application.isEditor ? ("file://" + Application.dataPath + "/StreamingAssets/" + fileName) :
                                      ("file://" + Application.dataPath + "/Raw/" + fileName);
#else //UNITY_WEBPLAYER
        if (Application.platform == RuntimePlatform.OSXWebPlayer
            || Application.platform == RuntimePlatform.WindowsWebPlayer)//浏览器版本，表格资源是各自（test、ljf、...）不同的，其他资源是共用的，路径是相对Assets的
        {
            return "AssetBundles/" + fileName;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer)//编辑器和WindowsPC版本，要用绝对路径
        {
            return "file://" + Application.dataPath + "/../AssetBundles/" + fileName;
        }
        else
        {
            return "file://" + Application.dataPath + "/../AssetBundles/" + fileName;
        }
#endif
    }

    #endregion

    #region 构造函数

    public Resource(Resource res, bool loadFailedShowError, bool permanent, bool skipCache, bool bForeceLoadFromPack)
    {
        m_ResCommonAttr = res.m_ResCommonAttr;

        this.loadFailedShowError = loadFailedShowError;
        this.permanent = permanent;
        this.bSkipCache = skipCache;
        this.bForeceLoadFromPack = bForeceLoadFromPack;

        this.m_www = res.WWW;
    }

    public Resource(string name, bool loadFailedShowError, bool permanent, bool skipCache, bool bForceLoadFromPack)
    {
        m_ResCommonAttr = new ResCommonAttr(name);
        this.loadFailedShowError = loadFailedShowError;
        this.permanent = permanent;
        this.bSkipCache = skipCache;
        this.bForeceLoadFromPack = bForceLoadFromPack;
    }

    #endregion

    public string FileName
    {
        get
        {
            return m_ResCommonAttr.FileName;
        }
    }

    public UnityEngine.Object MainAsset
    {
        get
        {
            if (m_ResCommonAttr.m_mainAsset == null && m_ResCommonAttr.m_assetBundle != null)
            {
                m_ResCommonAttr.m_mainAsset = m_ResCommonAttr.m_assetBundle.mainAsset;
            }

            return m_ResCommonAttr.m_mainAsset;

            // Note here: seems that every time we call mainAsset, unity re-parse the asset and generate a new copy;  This behavior
            // generate lots of unused resource
            // return www.assetBundle.mainAsset; 
        }
    }

    public AssetBundle GetAssetBundle
    {
        get
        {
            return m_ResCommonAttr.m_assetBundle;
        }

        set
        {
            m_ResCommonAttr.m_assetBundle = value;
        }
    }

    public WWW WWW
    {
        get
        {
            return m_www;
        }
    }

    public float TimeScale
    {
        get
        {
            if (m_ResCommonAttr.IsLoaded == true)
            {
                return 0.0f;
            }

            if (m_www == null)
            {
                return 0.0f;
            }

            return timeScaleStart + m_www.progress * (timeScaleEnd - timeScaleStart);
        }
    }

    public float TimeScaleStart
    {
        get
        {
            return timeScaleStart;
        }

        set
        {
            timeScaleStart = value;
        }
    }

    public float TimeScaleEnd
    {
        get
        {
            return timeScaleEnd;
        }

        set
        {
            timeScaleEnd = value;
        }
    }

    public bool IsLoadFaild
    {
        get
        {
            return m_ResCommonAttr.IsLoadFaild;
        }
    }

    public bool Permanent
    {
        get
        {
            return permanent;
        }
    }

    #region 委托事件

    public System.Object customData = null;

    public delegate void OnLoaded(Resource res);
    public event OnLoaded onLoaded;
    public delegate void OnLoading(Resource res);
    public event OnLoading onLoading;
    public delegate void OnError(Resource res);
    public event OnError onError;

    #endregion

    #region 实例管理

    /// <summary>
    /// 实例列表
    /// </summary>
    //private List<GameObject> m_goList = new List<GameObject>();

    /// <summary>
    /// <para>实例化</para>
    /// </summary>
    //public GameObject MyInstantiate()
    //{
    //    GameObject goRes = null;

    //    if (MainAsset == null)
    //    {
    //        return goRes;
    //    }

    //    goRes = GameObject.Instantiate(MainAsset) as GameObject;

    //    m_goList.Add(goRes);

    //    return goRes;
    //}

    /// <summary>
    /// 清理
    /// </summary>
    //public void Clear()
    //{
    //    foreach (GameObject go in m_goList)
    //    {
    //        GameObject.DestroyImmediate(go, true);
    //    }
    //    m_goList.Clear();
    //}

    #endregion

    bool aaa = false;

    protected void Load()
    {
        if (m_ResCommonAttr.FileName.Contains("http:"))
        {
            // network address
            m_www = new WWW(m_ResCommonAttr.FileName);
        }
        else
        {
#if (UNITY_ANDROID || UNITY_IPHONE)
            if (!bForeceLoadFromPack)
            {
                // 不强制只能从包内读取资源
                if (aaa)
                {
                    // 资源已经从包内考出到缓存
                    m_www = new WWW(RES_UPDATE_PATH(m_ResCommonAttr.FileName));
                }
                else
                {
                    // 先尝试从缓存读取资源，没有的话在从包内读取资源
                    string filePath = RefinePath(RES_UPDATE_PATH(m_ResCommonAttr.FileName));
                    if (File.Exists(filePath))
                    {
                        // 资源已经从包内考出到缓存
                        m_www = new WWW(RES_UPDATE_PATH(m_ResCommonAttr.FileName));
                    }
                    else
                    {
                        // 尝试从包中读取资源
                        m_www = new WWW(RES_PATH(m_ResCommonAttr.FileName));
                    }
                }
            }
            else
#endif
            {
                // then, try original path
                m_www = new WWW(RES_PATH(m_ResCommonAttr.FileName));
            }
        }
    }

    public void InitWWW()
    {
        if (m_ResCommonAttr.IsLoaded == false && m_www == null)
        {
            Load();
        }
    }

    /// <summary>
    /// <para>检查是否加载完成</para>
    /// </summary>
    /// <returns></returns>
    public bool CheckDoneWWW(bool bBackGround = false)
    {
        if (m_ResCommonAttr.IsLoaded == true)
        {
            if (ResourceManager.m_bShowLog)
            {
                Debug.Log("原来就完成了" + this.FileName + " " + Time.time);
            }

            if (MainAsset == null)//maybe last time this res load failed or this res is deleted in a wrong way
            {
                if (IsLoadFaild)
                {
                    if (loadFailedShowError)
                    {
                        Debug.LogError("this res loaded error, maybe the res is not exist, please check the config file " + this.FileName);
                    }
                    else
                    {
                        Debug.Log("this res loaded error, maybe the res is not exist, please check the config file " + this.FileName);
                    }
                }
                else
                {
                    Debug.LogError("MainAsset不见了, maybe deleted in a wrong way " + this.FileName);
                }

                if (onError != null)
                {
                    onError(this);
                }
            }
            else
            {
                if (onLoaded != null)
                {
                    onLoaded(this);
                }
            }

            ClearEvent();

            customData = null;

            return true;
        }

        if (m_www != null && m_www.isDone)
        {
            #region 404 Not Found
            // 3.4.2的插件BUG...  在服务器上找不到文件但www.error 不为null，并且www.text会有如下内容：
            //<html><head>
            //<title>404 Not Found</title>
            //</head><body>
            //<h1>Not Found</h1>
            //<p>The requested URL /seed/WebPlayer/AssetBundles/Scene/InstZone/kaichangwuding/kaichangwuding_chushengdian.unity3d was not found on this server.</p>
            //<hr>
            //<address>Apache/2.2.14 (Ubuntu) Server at 192.168.0.110 Port 80</address>
            //</body></html>
            // www.text 有严重的性能问题，之后想办法优化 2012.08.18
            //Debug.Log(FileName);
            //if (www.error == null && www.text.IndexOf("404 Not Found") == -1)
            #endregion

            try
            {
                if (m_www.error == null)
                {
                    if (m_www.assetBundle != null)
                    {
                        m_ResCommonAttr.m_assetBundle = m_www.assetBundle;
                        if (!bBackGround)
                        {
                            m_ResCommonAttr.m_mainAsset = m_ResCommonAttr.m_assetBundle.mainAsset;
                        }
                    }

                    if (ResourceManager.m_bShowLog)
                    {
                        Debug.Log("加载完成：" + this.m_ResCommonAttr.FileName + Time.time);
                    }

                    if (onLoaded != null)
                    {
                        onLoaded(this);
                    }

                    if (m_www.assetBundle != null)
                    {
                        m_www.assetBundle.Unload(false);
                    }
                }
                else
                {
                    m_ResCommonAttr.IsLoadFaild = true;

                    if (loadFailedShowError)
                    {
                        Debug.LogError("资源: " + this.m_ResCommonAttr.FileName + " 加载失败 !!! ");
                    }

                    //Debug.LogError("this res loaded error, maybe the res is not exist, please check the config file " + this.FileName);

                    if (onError != null)
                    {
                        onError(this);
                    }

                    if (ResourceManager.m_bShowLog)
                    {
                        Debug.LogError("加载失败：" + this.m_ResCommonAttr.FileName);
                    }

                    Debug.LogWarning(this.m_ResCommonAttr.FileName + " Load ERROR !!! :" + m_www.error ?? "404 Not Found" + "\n");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }

            ClearEvent();

            customData = null;

            m_ResCommonAttr.IsLoaded = true;

            m_www.Dispose();

            m_www = null;

            return true;
        }

        return false;
    }

    /// <summary>
    /// 后台加载使用，优先级低
    /// </summary>
    /// <returns></returns>
    public bool Update()
    {
        InitWWW();

        return CheckDoneWWW(true);
    }

    /// <summary>
    /// 清除事件
    /// </summary>
    private void ClearEvent()
    {
        if (onLoaded != null)
        {
            Delegate[] invList = onLoaded.GetInvocationList();
            foreach (OnLoaded handler in invList)
            {
                onLoaded -= handler;
            }
        }

        if (onError != null)
        {
            Delegate[] invList = onError.GetInvocationList();
            foreach (OnError handler in invList)
            {
                onError -= handler;
            }
        }

        if (onLoading != null)
        {
            Delegate[] invList = onLoading.GetInvocationList();
            foreach (OnLoading handler in invList)
            {
                onLoading -= handler;
            }
        }
    }

    /// <summary>
    /// 加载器
    /// </summary>
    private WWW m_www = null;

    /// <summary>
    /// 同一资源共用属性
    /// </summary>
    private ResCommonAttr m_ResCommonAttr = null;

    private float timeScaleEnd = 0.0f;
    private float timeScaleStart = 0.0f;

    private bool loadFailedShowError = false;

    /// <summary>
    /// 是否常驻内存（不参与资源回收）
    /// </summary>
    private bool permanent = false;

    public bool bSkipCache = false;

    /// <summary>
    /// 从StreamingPath寻找
    /// </summary>
    public bool bForeceLoadFromPack = false;
}

public class ResourceManager : SingletonMono<ResourceManager>
{
    /****************  Overides  ****************/
    void Awake()
    {
        lUpdateInitTicks = DateTime.Now.Ticks;
    }

    /// <summary>
    /// 开始运行的时间
    /// </summary>
    private long lUpdateInitTicks = 0;

    /// <summary>
    /// 开始更新的时间
    /// </summary>
    private float fUpdateBeginTime = 0;

    /// <summary>
    /// 每次更新所占的时间长度限制 小于10ms; 小于等于0, 不做任何限制
    /// </summary>
    private float fUpdateTimePerFrame = 0.2f;

    public void SetUpdateTimePerFrame(float fTime)
    {
        fUpdateTimePerFrame = fTime;
    }

    public static bool m_bShowLog = false;

    void Start()
    {
#if UNITY_IPHONE
        MAX_INLOADING_RESCOUNT = 5;      
        
        if (ClientUtil.GetMemorySize() > 512)
        {
            MAX_INLOADING_RESCOUNT = 10;
        }
#else
        MAX_INLOADING_RESCOUNT = 10;
#endif

        curLoadingRes_background = new Resource[MAX_INLOADING_RESCOUNT];
        
        for (int i = 0; i < MAX_INLOADING_RESCOUNT; i++)
        {
            curLoadingRes_background[i] = null;
        }
    }

    void Update()
    {

#if !UNITY_EDITOR
        try
        {
#endif

        fUpdateBeginTime = (DateTime.Now.Ticks - lUpdateInitTicks) / 10000000f;
        UpdateNormalQueue();

        UpdateBackGround();
#if !UNITY_EDITOR
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
#endif
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Clear()
    {
        foreach (Resource res in gcList)
        {
            //res.Clear();

            if (loadedDic.ContainsKey(res.FileName))
            {
                if (res.GetAssetBundle != null)
                {
                    res.GetAssetBundle.Unload(true);
                    res.GetAssetBundle = null;
                }

                //if (res.MainAsset)
                //{
                //    GameObject.DestroyImmediate(res.MainAsset, true);
                //}

                loadedDic.Remove(res.FileName);
            }
        }

        gcList.Clear();
    }

    /*

    /// <summary>
    /// 目前的逻辑：在加载一个新的模型文件时，检测是否需要清除之前所有的无用模型文件
    /// </summary>
    /// <param name="res"></param>
    void TryGabageCollect(Resource res)
    {
        if (res == null)
        {
            return;
        }

        if (GameStateManager.Instance.CurState == GameUpdateState.Instance || GameStateManager.Instance.IsLoading == true)
        {
            // 资源更新状态 或是 在游戏状态的切换未完成前，跳过如下逻辑
            return;
        }

        if (!res.FileName.Contains("Models/Char/"))
        {
            // 暂时只考虑对人物模型作优化
            return;
        }

        Dictionary<string, PathologicalGames.PrefabPool> dict = HotPoolManager.Instance.GetPrefabPools("Models/Char/");
        if (dict == null)
        {
            return;
        }

        foreach (var v in dict)
        {
            PathologicalGames.PrefabPool p = v.Value as PathologicalGames.PrefabPool;
            if (p != null && p.strResFileName.Equals(res.FileName))
            {
                //此资源已经加载
                return;
            }
        }

        // 到达此处，说明要加载一个之前未加载的人物资源
        float fWeight = 0;  // boss模型权重1，小兵权重为0.3, 对话npc权重为0.07  (boss平均6.5M，小兵平均：2.5M, npc平均0.5M)
        foreach (var v in dict)
        {
            PathologicalGames.PrefabPool p = v.Value as PathologicalGames.PrefabPool;
            if (p.strResFileName.Contains("boss") || p.strResFileName.Contains("yingmu") || p.strResFileName.Contains("mm") || p.strResFileName.Contains("k"))
            {
                fWeight += 1;
            }
            else if (p.strResFileName.Contains("npc"))
            {
                fWeight += 0.07f;
            }
            else // 认为是小兵
            {
                fWeight += 0.3f;
            }
        }

        float fLimit = ClientUtil.GetMemorySize() > 512 ? 10 : (GameStateManager.Instance.CurState == CopyState.Instance ? 5 : 1);

        if (fWeight > fLimit)
        {
            UnloadUnUsedModels();
        }
    }

    /// <summary>
    /// 卸载不用模型
    /// </summary>
    public void UnloadUnUsedModels()
    {
        List<string> list = HotPoolManager.Instance.UnloadUnusedModels("Models/Char/");

        foreach (string str in list)
        {
            Resource res = gcList.Find(x => x.FileName.Equals(str));

            if (res != null && res.GetAssetBundle != null)
            {
                res.GetAssetBundle.Unload(true);
                //res.Clear();
                //if (res.MainAsset)
                //{
                //    GameObject.DestroyImmediate(res.MainAsset, true);
                //}
            }

            if (loadedDic.ContainsKey(res.FileName))
            {
                loadedDic.Remove(res.FileName);
            }

            gcList.Remove(res);
        }

        Resources.UnloadUnusedAssets();
    }
    */
    
    /// <summary>
    /// 资源管理器是否空闲
    /// </summary>
    /// <returns></returns>
    public bool IsIdle()
    {
        for (int i = 0; i < MAX_INLOADING_RESCOUNT; i++)
        {
            if (curLoadingRes_background[i] != null)
            {
                return false;
            }
        }

        return (curLoadingRes_foreground.Count == 0 && loadingQueue.Count == 0 && loadingBackGround.Count == 0);
    }

    /****************  Public Funcs ****************/

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="name">name is relative to "AssetBundles/"</param>
    /// <returns></returns>
    public Resource LoadResource(string name)
    {
        return LoadResource(name, false, false, false, false);
    }

    public Resource LoadResource(string name, bool loadFailedShowError)
    {
        return LoadResource(name, loadFailedShowError, false, false, false);
    }

    /// <summary>
    /// 后台加载
    /// </summary>
    /// <param name="name"></param>
    /// <param name="loadFailedShowError"></param>
    /// <param name="bLoadFromStreamPath">强制必须从包内的StreamingAssets中读取资源</param>
    /// <returns></returns>
    public Resource LoadResourceBK(string name, bool loadFailedShowError, bool bLoadFromStreamPath = false)
    {
        return LoadResource(name, loadFailedShowError, true, false, true, bLoadFromStreamPath);
    }

    /// <summary>
    /// 加载永久性资源
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Resource LoadResourcePermanent(string name)
    {
        return LoadResource(name, false, false, true, false);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="name">相对于"AssetBundles/"</param>
    /// <param name="loadFailedShowError">加载失败显示错误</param>
    /// <param name="background">是否后台加载</param>
    /// <param name="permanent">是否永久性</param>
    /// <param name="skipCacheRes">是否跳过缓存资源</param>
    /// <param name="bLoadFromStreamPath">是否强制只能从包内StreamPath加载</param>
    /// <returns></returns>
    public Resource LoadResource(string name, bool loadFailedShowError, bool background, bool permanent, bool skipCacheRes, bool bForceLoadFromPack = false)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Resource File Name is NULL!");
            return null;
        }
         
        name = name.Replace("\\", "/");

        if (m_bShowLog)
        {
            Debug.Log("加载资源:" + " " + name + " " + loadFailedShowError + background + permanent + skipCacheRes + bForceLoadFromPack + " " + Time.time);
        }

        Resource res = null;

        // 从已经加载的资源中查找
        if (res == null)
        {
            Resource tmpRes = null;
            if ((!skipCacheRes) && loadedDic.TryGetValue(name, out tmpRes))
            {
                if (m_bShowLog)
                {
                    Debug.LogWarning("--从已经加载的资源中查找");
                }
                res = new Resource(tmpRes, loadFailedShowError, permanent, skipCacheRes, bForceLoadFromPack);
            }
        }

        // 如果要加载的资源，就是当期正在加载中的资源，则返回该资源
        for (int i = 0, imax = curLoadingRes_foreground.Count; i < imax; i++)
        {
            if (curLoadingRes_foreground[i] != null && curLoadingRes_foreground[i].FileName == name)
            {
                if (m_bShowLog)
                {
                    Debug.LogWarning("--前端加载");
                }
                res = new Resource(curLoadingRes_foreground[i], loadFailedShowError, permanent, skipCacheRes, bForceLoadFromPack);
                break;
            }
        }

        // 从加载序列中查找是否已经有同类资源
        if (res == null)
        {
            foreach (Resource r in loadingQueue)
            {
                if (r.FileName == name)
                {
                    if (m_bShowLog)
                    {
                        Debug.LogWarning("--前端等待");
                    }
                    res = new Resource(r, loadFailedShowError, permanent, skipCacheRes, bForceLoadFromPack);
                    break;
                }
            }
        }

        // 从后台正在加载中的序列中查找
        if (res == null)
        {
            for (int i = 0; i < MAX_INLOADING_RESCOUNT; i++)
            {
                if (curLoadingRes_background == null)
                {
                    //Debug.LogError("aaa");
                    continue;
                }

                if (curLoadingRes_background[i] != null && curLoadingRes_background[i].FileName == name)
                {
                    if (m_bShowLog)
                    {
                        Debug.LogWarning("--后台加载");
                    }
                    res = new Resource(curLoadingRes_background[i], loadFailedShowError, permanent, skipCacheRes, bForceLoadFromPack);
                    break;
                }
            }
        }

        // 从background资源加载队列中查找
        if (res == null)
        {
            foreach (Resource r in loadingBackGround)
            {
                if (r.FileName == name)
                {
                    if (m_bShowLog)
                    {
                        Debug.LogWarning("--后台等待");
                    }
                    res = new Resource(r, loadFailedShowError, permanent, skipCacheRes, bForceLoadFromPack);
                    break;
                }
            }
        }

        // 创建新资源
        if (res == null)
        {
            if (m_bShowLog)
            {
                Debug.LogWarning("--新创建");
            }
            res = new Resource(name, loadFailedShowError, permanent, skipCacheRes, bForceLoadFromPack);
        }

        if (isLoadingDependence)  // 插在加载队列前面，为什么呢？着急
        {
            loadingQueue.Insert(0, res);
        }
        else if (!background) // 放入加载序列
        {
            loadingQueue.Add(res);
        }
        else // 后台加载
        {
            loadingBackGround.Add(res);
        }

        res.onLoaded += OnResourceLoaded;

        res.onError += OnResourceError;

        //TryGabageCollect(res);

        return res;
    }

    public delegate void OnLoaded(Resource res);
    public event OnLoaded onLoaded;

    public delegate void OnLoading(Resource res);
    public event OnLoading onLoading;

    private void OnResourceLoaded(Resource res)
    {
        if (onLoaded != null)
        {
            onLoaded(res);
        }
    }

    private void OnResourceError(Resource res)
    {
    }

    /// <summary>
    /// 资源加载完成
    /// </summary>
    /// <param name="res"></param>
    private void OnResLoadFinished(Resource res)
    {
        if (res == null)
        {
            return;
        }

        if (res.bSkipCache)
        {
            if (res.Permanent == false)
            {
                gcList.Add(res);
            }
        }
        else if (!loadedDic.ContainsKey(res.FileName))
        {
            if (m_bShowLog)
            {
                Debug.LogWarning(res.FileName + " 存起来" + Time.time);
            }

            loadedDic.Add(res.FileName, res);

            if (res.Permanent == false)
            {
                gcList.Add(res);
            }
        }

        if (curLoadingRes_foreground.Count > 0 && curLoadingRes_foreground[0] == res)  // 前台加载资源
        {
            curLoadingRes_foreground.RemoveAt(0);
        }
        else // 后台加载资源
        {
            int i;
            for (i = 0; i < MAX_INLOADING_RESCOUNT; i++)
            {
                if (res == curLoadingRes_background[i])
                {
                    curLoadingRes_background[i] = null;
                    break;
                }
            }

            if (i == MAX_INLOADING_RESCOUNT)
            {
                Debug.LogError("Resource Manager Error : Unknown Reason!!! " + res.FileName);
            }
        }
    }

    void UpdateNormalQueue()
    {
        if (curLoadingRes_foreground.Count == 0 && loadingQueue.Count == 0)
        {
            return;
        }

        while (curLoadingRes_foreground.Count < MAX_INLOADING_RESCOUNT && loadingQueue.Count != 0)
        {
            loadingQueue[0].InitWWW();
            curLoadingRes_foreground.Add(loadingQueue[0]);
            loadingQueue.RemoveAt(0);
        }

        // 如果每帧更新所花的时间超过fUpdateTimePerFrame, 停止本次更新
        if (curLoadingRes_foreground.Count > 0 && curLoadingRes_foreground[0].CheckDoneWWW())
        {
            OnResLoadFinished(curLoadingRes_foreground[0]);

            float fUpdateTimeNow = (DateTime.Now.Ticks - lUpdateInitTicks) / 10000000f;
            if (fUpdateTimePerFrame > 0 && fUpdateTimeNow - fUpdateBeginTime > fUpdateTimePerFrame)
            {
                return;
            }
            UpdateNormalQueue();
        }
    }

    void UpdateBackGround()
    {
        if (curLoadingRes_foreground.Count != 0 || loadingQueue.Count != 0)  // 等待前台加载完毕，前台优先级高于后台
        {
            return;
        }

        for (int i = 0; i < MAX_INLOADING_RESCOUNT; i++)
        {
            if (curLoadingRes_background[i] == null && loadingBackGround.Count > 0)
            {
                curLoadingRes_background[i] = loadingBackGround[0];

                loadingBackGround.RemoveAt(0);
            }

            if (curLoadingRes_background[i] != null && curLoadingRes_background[i].Update())
            {
                OnResLoadFinished(curLoadingRes_background[i]);
            }
        }
    }

    /// <summary>
    /// 计算加载时间缩放
    /// </summary>
    public void CalculateLoadingTimeScale()
    {
        for (int i = 0, imax = loadingQueue.Count; i < imax; i++)
        {
            Resource res = loadingQueue[i];
            res.TimeScaleStart = 1.0f / (float)(imax) * i;
            res.TimeScaleEnd = 1.0f / (float)(imax) * (i + 1);
        }
    }

    /// <summary>
    /// 开始加载依赖性资源
    /// </summary>
    public void BeginLoadingDependence()
    {
        isLoadingDependence = true;
    }

    /// <summary>
    /// 结束加载依赖性资源
    /// </summary>
    public void EndLoadingDependence()
    {
        isLoadingDependence = false;
    }

    /// <summary>
    /// 加载好的资源
    /// </summary>
    private Dictionary<string, Resource> loadedDic = new Dictionary<string, Resource>();

    /// <summary>
    /// 垃圾列表
    /// </summary>
    private List<Resource> gcList = new List<Resource>();

    /// <summary>
    /// 最大同时下载资源数
    /// </summary>
    private int MAX_INLOADING_RESCOUNT = 10;

    /// <summary>
    /// <para>前台等待队列</para>
    /// <para>前台资源加载可乱序，解析暂时不可乱序(ToDo: Fence机制)</para>
    /// </summary>
    private List<Resource> loadingQueue = new List<Resource>();

    /// <summary>
    /// 前台正在加载队列
    /// </summary>
    private List<Resource> curLoadingRes_foreground = new List<Resource>();

    /// <summary>
    /// <para>后台资源可以乱序</para>
    /// <para>后台加载等待队列</para>
    /// </summary>
    private List<Resource> loadingBackGround = new List<Resource>();

    /// <summary>
    /// <para>后台正在加载队列</para>
    /// <para>为什么要用数组</para>
    /// </summary>
    private Resource[] curLoadingRes_background;

    /// <summary>
    /// 是否加载依赖的资源
    /// </summary>
    private bool isLoadingDependence = false;
}
