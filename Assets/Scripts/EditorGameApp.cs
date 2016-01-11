using UnityEngine;
using System.Collections;

public class EditorGameApp : SingletonMono<EditorGameApp> 
{
    /// <summary>
    /// 是否暂停中
    /// </summary>
    private bool paused = false;

	void Awake() 
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

    void Start()
    {
        StopCoroutine("GameStart");
        StartCoroutine("GameStart");
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameStart()
    {
        //等候一帧，让其他脚本的Start执行完
        yield return new WaitForEndOfFrame();
        yield break;
    }

	void Update() 
    {
        HandlePause();

        if (Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.Home))
        {
            Application.Quit();
        }
	}

    /// <summary>
    /// 处理暂停
    /// </summary>
    private void HandlePause()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            paused = !paused;
        }

        if (paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
