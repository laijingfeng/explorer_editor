using UnityEngine;
using System.Collections;

/// <summary>
/// 浮桥
/// </summary>
[ExecuteInEditMode]
public class FloatBridge : MonoBehaviour
{
    public float upValue = 1f;

    public float downValue = 1f;

    public float leftValue = 1f;

    public float rightValue = 1f;

    public float speed = 1f;

    /// <summary>
    /// 贴图的大小
    /// </summary>
    private Vector3 spriteSize;

    void Start()
    {
        StartCoroutine("Work");
    }

    /// <summary>
    /// 获得贴图大小
    /// </summary>
    private void GetSpriteSize()
    {
        SpriteRenderer sr = transform.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            spriteSize = Vector3.one;//临时修改//UIUtil.Vector3XVector3(sr.sprite.bounds.size, transform.lossyScale, true, true);
        }
        else
        {
            spriteSize = Vector3.zero;
        }
    }

    private IEnumerator Work()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            int aa = Random.Range(0, 126);

            if (aa < 25)
            {
                yield return StartCoroutine("FloatUp", upValue);
                yield return StartCoroutine("FloatDown", upValue);
            }
            else if (aa < 50)
            {
                yield return StartCoroutine("FloatLeft", leftValue);
                yield return StartCoroutine("FloatRight", leftValue);
            }
            else if (aa < 75)
            {
                yield return StartCoroutine("FloatDown", downValue);
                yield return StartCoroutine("FloatUp", downValue);
            }
            else if (aa < 100)
            {
                yield return StartCoroutine("FloatRight", rightValue);
                yield return StartCoroutine("FloatLeft", rightValue);
            }
        }
    }

    private IEnumerator FloatUp(float value)
    {
        Vector3 origionPos = transform.position;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            Vector3 vt = transform.position;
            vt.y += Time.deltaTime * speed;
            if (vt.y >= origionPos.y + value)
            {
                vt.y = origionPos.y + value;
                transform.position = vt;
                yield break;
            }
            transform.position = vt;
        }
    }

    private IEnumerator FloatDown(float value)
    {
        Vector3 origionPos = transform.position;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            Vector3 vt = transform.position;
            vt.y -= Time.deltaTime * speed;
            if (vt.y <= origionPos.y - value)
            {
                vt.y = origionPos.y - value;
                transform.position = vt;
                yield break;
            }
            transform.position = vt;
        }
    }

    private IEnumerator FloatLeft(float value)
    {
        Vector3 origionPos = transform.position;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            Vector3 vt = transform.position;
            vt.x -= Time.deltaTime * speed;
            if (vt.x <= origionPos.x - value)
            {
                vt.x = origionPos.x - value;
                transform.position = vt;
                yield break;
            }
            transform.position = vt;
        }
    }

    private IEnumerator FloatRight(float value)
    {
        Vector3 origionPos = transform.position;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            Vector3 vt = transform.position;
            vt.x += Time.deltaTime * speed;
            if (vt.x >= origionPos.x + value)
            {
                vt.x = origionPos.x + value;
                transform.position = vt;
                yield break;
            }
            transform.position = vt;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == true)
        {
            return;
        }

        //中心
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position;
        center.y += upValue - 0.5f * (upValue + downValue);
        center.x += rightValue - 0.5f * (leftValue + rightValue);

        Vector3 size = new Vector3(leftValue + rightValue, upValue + downValue, 0f);

        Gizmos.DrawWireCube(center, size);

        GetSpriteSize();

        //外环
        Gizmos.color = Color.red;
        float top = center.y + 0.5f * (size.y + spriteSize.y);
        float down = center.y - 0.5f * (size.y + spriteSize.y);
        float left = center.x - 0.5f * (size.x + spriteSize.x);
        float right = center.x + 0.5f * (size.x + spriteSize.x);
        Gizmos.DrawWireCube(new Vector3(0.5f * (right + left), 0.5f * (top + down), 0), new Vector3(right - left, top - down, 0));

        //内环
        Gizmos.color = Color.gray;
        float top1 = center.y + 0.5f * (size.y - spriteSize.y);
        float down1 = center.y - 0.5f * (size.y - spriteSize.y);
        float left1 = center.x - 0.5f * (size.x - spriteSize.x);
        float right1 = center.x + 0.5f * (size.x - spriteSize.x);
        Gizmos.DrawWireCube(new Vector3(0.5f * (right1 + left1), 0.5f * (top1 + down1), 0), new Vector3(right1 - left1, top1 - down1, 0));
    }
#endif
}
