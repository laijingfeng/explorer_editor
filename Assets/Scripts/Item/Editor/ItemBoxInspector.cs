using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ItemBox))]
public class ItemBoxInspector : Editor
{
    /// <summary>
    /// 盒子
    /// </summary>
    protected ItemBox m_ItemBox;

    public override void OnInspectorGUI()
    {
        m_ItemBox = target as ItemBox;
        DrawAttr();
    }

    /// <summary>
    /// 绘制属性
    /// </summary>
    public virtual void DrawAttr()
    {
        m_ItemBox.m_LifeMode = (ItemBox.LifeMode)EditorGUILayout.EnumPopup("生命模式", m_ItemBox.m_LifeMode);
        switch (m_ItemBox.m_LifeMode)
        {
            case ItemBox.LifeMode.Count:
                {
                    m_ItemBox.m_iCount = EditorGUILayout.IntField("碰撞次数", m_ItemBox.m_iCount);
                }
                break;
            case ItemBox.LifeMode.Time:
                {
                    m_ItemBox.m_fLife = EditorGUILayout.FloatField("生存时长(秒)", m_ItemBox.m_fLife);
                }
                break;
        }

        m_ItemBox.m_DeadResult = (ItemBox.DeadResult)EditorGUILayout.EnumPopup("死亡模式", m_ItemBox.m_DeadResult);
        switch (m_ItemBox.m_DeadResult)
        {
            case ItemBox.DeadResult.ChangeSprite:
                {
                    m_ItemBox.m_ChangeSprite = (Sprite)EditorGUILayout.ObjectField("死亡换图", m_ItemBox.m_ChangeSprite, typeof(Sprite), true) as Sprite;
                }
                break;
        }

        m_ItemBox.m_TriggerDir = (ItemBox.CollisionDir)EditorGUILayout.EnumPopup("有效触发方向", m_ItemBox.m_TriggerDir);

        m_ItemBox.m_bMakeCoin = EditorGUILayout.Toggle("是否生产金币", m_ItemBox.m_bMakeCoin);
    }
}
