using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BulletScreenDisplayerInfo
{

    [Header("组件挂接的节点")]
    public Transform Owner;
    [Header("弹幕ID")]
    //  public GameObject Icon;
    [Header("文本预制")]
    public GameObject TextPrefab;  //普通弹幕

    [Header("举手文本预制")]
    public GameObject HandTextPrefab;

    [Header("弹幕布局组件")]
    public GridLayoutGroup ScreenRoot;

    [Header("举手布局组件")]
    public GridLayoutGroup HandScreenRoot;

    [Header("初始化行数")]
    public int TotalRowCount = 2;
    [Header("行高（单位：像素）")]
    public float RowHeight;
    [Header("字体边框的节点名字")]
    public string TextBoxNodeName;
    [Header("从屏幕一侧到另外一侧用的时间")]
    public float ScrollDuration = 8F;
    [Header("举手从屏幕一侧到另外一侧用的时间")]
    public float HandScrollDuration = 3f;

    [Header("两弹幕文本之间的最小间隔")]
    public float MinInterval = 20F;
    [Header("移动完成后的销毁延迟")]
    public float KillBulletTextDelay = 0F;
    public BulletScreenDisplayerInfo(Transform owner,   GameObject textPrefab, GameObject handTextPrefab, GridLayoutGroup screenRoot,GridLayoutGroup handScreenRoot,
        int initialRowCount = 1,
        float rowHeight = 100F,
        string textBoxNodeName = "text_box_node_name")
    {
        Owner = owner;
      
        TextPrefab = textPrefab;
        HandTextPrefab = handTextPrefab;
        HandScreenRoot = handScreenRoot;
        ScreenRoot = screenRoot;
        TotalRowCount = initialRowCount;
        RowHeight = rowHeight;
        TextBoxNodeName = textBoxNodeName;
    }
}


/// <summary>
/// 弹幕播放器组件
/// </summary>
public class BulletScreenDisplayer : MonoBehaviour
{

    public bool Enable { get; set; }
    public BulletTextInfo[] _currBulletTextInfoList;
    public BulletHandTextInfo[] _currBulletHandTextInfoList;
    [SerializeField] private BulletScreenDisplayerInfo _info;
    public float ScrollDuration
    {
        get { return _info.ScrollDuration; }
    }
    public float HandScrollDuration
    {
        get { return _info.HandScrollDuration; }
    }
    private float _bulletScreenWidth;
    private float _bulletHandScreenWidth;
    public float BulletScreenWidth
    {
        get { return _bulletScreenWidth; }
    }
    public float BulletHandScreenWidth
    {
        get { return _bulletHandScreenWidth; }
    }
    public GameObject TextElementPrefab
    {
        get { return _info.TextPrefab; }
    }
    public GameObject HandElementPrefab
    {
        get { return _info.HandTextPrefab; }
    }
    public string TextBoxNodeName
    {
        get { return _info.TextBoxNodeName; }
    }
    public float KillBulletTextDelay
    {
        get { return _info.KillBulletTextDelay; }
    }
    public Transform ScreenRoot
    {
        get { return _info.ScreenRoot.transform; }
    }

    public Transform HandScreenRoot
    {
        get { return _info.HandScreenRoot.transform;  }
    }
    public static BulletScreenDisplayer Create(BulletScreenDisplayerInfo displayerInfo)
    {
        BulletScreenDisplayer instance = displayerInfo.Owner.gameObject.AddComponent<BulletScreenDisplayer>();
        instance._info = displayerInfo;
        return instance;
    }
    public void AddBullet(string textContent, bool showBox = false, ScrollDirection direction = ScrollDirection.RightToLeft)
    {
       // Debug.Log(textContent);
        BulletScreenTextElement.Create(this, textContent, showBox, direction);
    }
    private void Start()
    {
        SetScrollScreen();
        SetScrollHandScreen();
        InitRow();
        InitHandRow();
    }
    /// <summary>
    /// 初始化行数
    /// </summary>
    private void InitRow()
    {
        Utility.DestroyAllChildren(_info.ScreenRoot.gameObject);
        _currBulletTextInfoList = new BulletTextInfo[_info.TotalRowCount];
        for (int rowIndex = 0; rowIndex < _info.TotalRowCount; rowIndex++)
        {
            _currBulletTextInfoList[rowIndex] = null;
            string rowNodeName = string.Format("row_{0}", rowIndex);
            GameObject newRow = new GameObject(rowNodeName);
            var rt = newRow.AddComponent<RectTransform>();
            rt.SetParent(_info.ScreenRoot.transform, false);
        }
    }


    /// <summary>
    /// 初始化举手弹幕行数
    /// </summary>
    private void InitHandRow()
    {
        Utility.DestroyAllChildren(_info.HandScreenRoot  .gameObject);
        _currBulletHandTextInfoList = new BulletHandTextInfo[_info.TotalRowCount];
        for (int rowIndex = 0; rowIndex < _info.TotalRowCount; rowIndex++)
        {
            _currBulletHandTextInfoList[rowIndex] = null;
            string rowNodeName = string.Format("row_{0}", rowIndex);
            GameObject newRow = new GameObject(rowNodeName);
            var rt = newRow.AddComponent<RectTransform>();
            rt.SetParent(_info.HandScreenRoot .transform, false);
        }
    }

    private void SetScrollScreen()
    {
        _info.ScreenRoot.childAlignment = TextAnchor.MiddleCenter;
        _info.ScreenRoot.cellSize = new Vector2(100F, _info.RowHeight);
        _bulletScreenWidth = _info.ScreenRoot.GetComponent<RectTransform>().rect.width;
    }
    private void SetScrollHandScreen()
    {
        _info.ScreenRoot.childAlignment = TextAnchor.MiddleCenter;
        _info.ScreenRoot.cellSize = new Vector2(100F, _info.RowHeight);
        _bulletHandScreenWidth = _info.HandScreenRoot .GetComponent<RectTransform>().rect.width;
    }
    public Transform GetTempRoot()
    {
        return _info.ScreenRoot.transform.Find(string.Format("row_{0}", 0));
    }

    public Transform GetTempHandRoot()
    {
        return _info.HandScreenRoot.transform.Find(string.Format(""));
    }
    public Transform GetRowRoot(BulletTextInfo newTextInfo)
    {
        const int notFoundRowIndex = -1;
        int searchedRowIndex = notFoundRowIndex;
        newTextInfo.SendTime = Time.realtimeSinceStartup;

        for (int rowIndex = 0; rowIndex < _currBulletTextInfoList.Length; rowIndex++)
        {
            var textInfo = _currBulletTextInfoList[rowIndex];
            //如果此行没有弹幕，直接创建新的
            if (textInfo == null)
            {
                searchedRowIndex = rowIndex;
                break;
            }
            float l1 = textInfo.TextWidth;
            float l2 = newTextInfo.TextWidth;
            float sentDeltaTime = newTextInfo.SendTime - textInfo.SendTime;
           
            var aheadTime = GetAheadTime(l1, 20);
         
            if (sentDeltaTime >= aheadTime)
            {//fit and add.
                searchedRowIndex = rowIndex;
                break;
            }
            //go on searching in next row.
        }
        if (searchedRowIndex == notFoundRowIndex)
        {//no fit but random one row.
            int repairRowIndex = Random.Range(0, _currBulletTextInfoList.Length);
            searchedRowIndex = repairRowIndex;
        }
        _currBulletTextInfoList[searchedRowIndex] = newTextInfo;
        Transform root = _info.ScreenRoot.transform.Find(string.Format("row_{0}", searchedRowIndex));
        return root;
    }

    public Transform GetHandRowRoot(BulletHandTextInfo newTextInfo)
    {
        const int notFoundRowIndex = -1;
        int searchedRowIndex = notFoundRowIndex;
        newTextInfo.HandSendTime = Time.realtimeSinceStartup;

        for (int rowIndex = 0; rowIndex < _currBulletHandTextInfoList.Length; rowIndex++)
        {
            var textInfo = _currBulletHandTextInfoList[rowIndex];
            //如果此行没有弹幕，直接创建新的
            if (textInfo == null)
            {
                searchedRowIndex = rowIndex;
                break;
            }
            float l1 = textInfo.HandTextWidth;
            float l2 = newTextInfo.HandTextWidth ;
            float sentDeltaTime = newTextInfo.HandSendTime - textInfo.HandSendTime ;

            var aheadTime = GetHandAheadTime(20, 11);

            if (sentDeltaTime >= aheadTime)
            {//fit and add.
                searchedRowIndex = rowIndex;
                break;
            }
            //go on searching in next row.
        }
        if (searchedRowIndex == notFoundRowIndex)
        {//no fit but random one row.
            int repairRowIndex = Random.Range(0, _currBulletHandTextInfoList.Length);
            searchedRowIndex = repairRowIndex;
        }
        _currBulletHandTextInfoList[searchedRowIndex] = newTextInfo;
        Transform root = _info.HandScreenRoot.transform.Find(string.Format("row_{0}", searchedRowIndex));
        return root;
    }

    /// <summary>
    /// 换行规则
    /// <param name="lastBulletTextWidth">最后一个子弹文本的宽度</param>
    /// <param name="newCameBulletTextWidth">新的子弹文本的宽度</param>
    /// <returns></returns>
    private float GetAheadTime(float lastBulletTextWidth, float newCameBulletTextWidth)
    {
        float aheadTime = 0f;
        if (lastBulletTextWidth <= newCameBulletTextWidth)
        {
            float s1 = lastBulletTextWidth + BulletScreenWidth + _info.MinInterval;
            float v1 = (lastBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
            float s2 = BulletScreenWidth;
            float v2 = (newCameBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
            aheadTime = s1 / v1 - s2 / v2;
      
        }
        else
        {
            float aheadDistance = lastBulletTextWidth + _info.MinInterval;
            float v1 = (lastBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
            aheadTime = aheadDistance / v1;
            
        }
        return aheadTime;
    }

    /// <summary>
    /// 换行规则
    /// <param name="lastBulletTextWidth">最后一个子弹文本的宽度</param>
    /// <param name="newCameBulletTextWidth">新的子弹文本的宽度</param>
    /// <returns></returns>
    private float GetHandAheadTime(float lastBulletTextWidth, float newCameBulletTextWidth)
    {
        float aheadTime = 0f;
        if (lastBulletTextWidth <= newCameBulletTextWidth)
        {
            float s1 = lastBulletTextWidth + BulletHandScreenWidth + _info.MinInterval;
            float v1 = (lastBulletTextWidth + BulletHandScreenWidth) / _info.HandScrollDuration;
            float s2 = BulletHandScreenWidth;
            float v2 = (newCameBulletTextWidth + BulletHandScreenWidth) / _info.HandScrollDuration;
            aheadTime = s1 / v1 - s2 / v2;

        }
        else
        {
            float aheadDistance = lastBulletTextWidth + _info.MinInterval;
            float v1 = (lastBulletTextWidth + BulletHandScreenWidth) / _info.HandScrollDuration;
            aheadTime = aheadDistance / v1;

        }
        return aheadTime;
    }


}
