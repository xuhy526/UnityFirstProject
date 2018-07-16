using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
/// <summary>
/// 弹幕文本组件
/// </summary>
public enum ScrollDirection
{
    RightToLeft = 0,
    LeftToRight = 1
}
public class BulletTextInfo
{
    public float TextWidth;
    public float SendTime;
}
public class BulletHandTextInfo
{
    public float HandTextWidth;
    public float HandSendTime;
}

public class BulletScreenTextElement : MonoBehaviour
{

    [SerializeField] private BulletScreenDisplayer _displayer;
    [SerializeField] private string _textContent;
    [SerializeField] private bool _showBox;
    [SerializeField] private ScrollDirection _scrollDirection;
    [SerializeField] private Text _text;
    [SerializeField] private float _textWidth;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;

    public static BulletScreenTextElement Create(BulletScreenDisplayer displayer, string textContent,
        bool showBox = false,
        ScrollDirection direction = ScrollDirection.RightToLeft)
    {
        BulletScreenTextElement instance = null;
        if (displayer == null)
        {
            Debug.Log("BulletScreenTextElement.Create(), displayer can not be null !");
            return null;
        }
       
        if (textContent.Contains ("1"))
        {
            // Debug.Log("ddd2222");
            GameObject go = Instantiate(displayer.TextElementPrefab) as GameObject;
            go.transform.SetParent(displayer.GetTempRoot());
            go.transform.localPosition = Vector3.up * 10000F;
            go.transform.localScale = Vector3.one;
            instance = go.AddComponent<BulletScreenTextElement>();
            instance._displayer = displayer;
            instance._textContent = textContent;
            // instance._showBox = showBox;
            instance._showBox = true;

            //限制方向
            //instance._scrollDirection = direction;
            instance._scrollDirection = ScrollDirection.RightToLeft;

        }
        else if (textContent.Contains("2"))
        {
            //Debug.Log("ddd");
            GameObject go = Instantiate(displayer.HandElementPrefab) as GameObject;
            go.transform.SetParent(displayer.GetTempHandRoot());
            go.transform.localPosition = Vector3.up * 10000F;
            go.transform.localScale = Vector3.one;
            instance = go.AddComponent<BulletScreenTextElement>();
            instance._displayer = displayer;
            instance._textContent = textContent;

            instance._showBox = showBox;
            instance._showBox = true;

            //限制方向
            instance._scrollDirection = direction;
            instance._scrollDirection = ScrollDirection.RightToLeft;
        }

        return instance;
    }
    private IEnumerator Start()
    {
        SetBoxView();
        SetText();
        //get correct text width in next frame.
        yield return new WaitForSeconds(0.2f);
        RecordTextWidthAfterFrame();
        SetRowInfo();
        SetHandRowInfo();
        SetTweenStartPosition();
        SetTweenEndPosition();
        StartMove();
    }
    /// <summary>
    /// The outer box view of text
    /// </summary>
    private void SetBoxView()
    {
        //Transform boxNode = transform.Find(_displayer.TextBoxNodeName);
        //if (boxNode == null)
        //{
        //    Debug.LogErrorFormat(
        //        "BulletScreenTextElement.SetBoxView(), boxNode == null. boxNodeName: {0}",
        //        _displayer.TextBoxNodeName);
        //    return;
        //}
        //boxNode.gameObject.SetActive(_showBox);
    }

    /// <summary>
    /// 给弹幕文本赋值
    /// </summary>
    private void SetText()
    {
        _text = GetComponentInChildren<Text>();
        //_text.enabled = false;
        if (_text == null)
        {
            Debug.Log("BulletScreenTextElement.SetText(), not found Text!");
            return;
        }
        _text.alignment = _scrollDirection == ScrollDirection.RightToLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
        //make sure there exist ContentSizeFitter componet for extend text width
        var sizeFitter = _text.GetComponent<ContentSizeFitter>();
        if (!sizeFitter)
        {
            sizeFitter = _text.gameObject.AddComponent<ContentSizeFitter>();
        }
        //text should extend in horizontal
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        _text.text = _textContent;
    }
    private void RecordTextWidthAfterFrame()
    {
        _textWidth = _text.GetComponent<RectTransform>().sizeDelta.x;
    }
    private void SetTweenStartPosition()
    {
       // Debug.Log(_displayer.BulletScreenWidth);
        if (_textContent.Contains("2"))
        {
            Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.right : Vector3.left;
            _startPos = nor * (_displayer.BulletHandScreenWidth  / 2F + _textWidth / 2F);
            transform.localPosition = _startPos;
        }
        else
        {
            Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.right : Vector3.left;           
            _startPos = nor * (_displayer.BulletScreenWidth / 2F + _textWidth / 2F);        
            transform.localPosition = _startPos;
        }
            
      
    }
    private void SetTweenEndPosition()
    {
        if (_textContent.Contains("2"))
        {
            Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.left : Vector3.right;
            _endPos = nor * (_displayer.BulletHandScreenWidth / 2F + _textWidth / 2F);
            //Debug.Log(_endPos);
        }
        else
        {
            Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.left : Vector3.right;
            _endPos = nor * (_displayer.BulletScreenWidth / 2F + _textWidth / 2F);
        }
           
    }
    private void SetRowInfo()
    {
        if (_textContent.Contains("2"))
            return;
            var bulletTextInfo = new BulletTextInfo()
        {
            SendTime = Time.realtimeSinceStartup,
            TextWidth = _textWidth
        };
  
            var rowRoot = _displayer.GetRowRoot(bulletTextInfo);
            transform.SetParent(rowRoot, false);
            transform.localScale = Vector3.one;
    }

    private void SetHandRowInfo()
    {
        if (_textContent.Contains("1"))
            return;

        var bulletHandTextInfo = new BulletHandTextInfo ()
        {
            HandSendTime = Time.realtimeSinceStartup,
            HandTextWidth = _textWidth
        };

        var rowRoot = _displayer.GetHandRowRoot(bulletHandTextInfo);
        transform.SetParent(rowRoot, false);
        transform.localScale = Vector3.one;
    }
    private void StartMove()
    {
        if (_textContent.Contains("1"))
        {
            //make sure the text is active.
            //the default ease of DoTewwen is not Linear.
            transform.DOLocalMoveX(_endPos.x, _displayer.ScrollDuration).OnComplete(OnTweenFinished).SetEase(Ease.Linear);
        }
        else
        {
            //make sure the text is active.
            //the default ease of DoTewwen is not Linear.
            transform.DOLocalMoveX(_endPos.x, _displayer.HandScrollDuration).OnComplete(OnTweenFinished).SetEase(Ease.Linear);
        }

    }
    private void OnTweenFinished()
    {
        if (_textContent.Contains("2"))
        {
            Destroy(gameObject, 2);
            return;
        }
      
            Destroy(gameObject, _displayer.KillBulletTextDelay);
        
      
    }
}

