using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
    /// <summary>
    /// 分数
    /// </summary>
	public int score = 0;

    /// <summary>
    /// 角色控制器
    /// </summary>
	private PlayerControl playerControl;
	
    /// <summary>
    /// 前一次的分数
    /// </summary>
    private int previousScore = 0;

	void Awake ()
	{
		//playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
	}

	void Update ()
	{
		guiText.text = "Score: " + score;

        if (previousScore != score
            && playerControl != null)
        {
            playerControl.StartCoroutine(playerControl.Taunt());

            playerControl.ContinueJumpCount = score / 5000 + 1;
        }

		previousScore = score;
	}
}
