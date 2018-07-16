using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExampleBulletScreen : MonoBehaviour {
    public BulletScreenDisplayer Displayer;
    private string strDanMu;

    /// <summary>
    /// 弹幕内容
    /// </summary>
   //// public List<string> _textPool = new List<string>() {
   //     "ウワァン!!(ノДヽ) ・・(ノ∀・)チラ 実ゎ・・嘘泣き",
   //     "(╯#-_-)╯~~~~~~~~~~~~~~~~~╧═╧ ",
   //     "<(￣︶￣)↗[GO!]",
   //     "(๑•́ ₃ •̀๑) (๑¯ิε ¯ิ๑) ",
   //     "(≖͞_≖̥)",
   //     "(｀д′) (￣^￣) 哼！ <(｀^′)>",
   //     "o(*￣︶￣*)o",
   //     " ｡:.ﾟヽ(｡◕‿◕｡)ﾉﾟ.:｡+ﾟ",
   //     "号(┳Д┳)泣",
   //     "( ＾∀＾）／欢迎＼( ＾∀＾）",
   //     "ドバーッ（┬┬＿┬┬）滝のような涙",
   //     "(。┰ω┰。",
   //     "132233333333124255555555555555"
   // };
    // Use this for initialization

    void Start()
    {
        Displayer.Enable = true;
       // StartCoroutine(StartDisplayBulletScreenEffect());
    }
    //private void  StartDisplayBulletScreenEffect()
    //{
    //    while (Displayer.Enable)
    //    {
    //        if (GetText() != null)
    //        {
    //            Displayer.AddBullet(GetText(), CheckShowBox(), GetDirection());

    //        }
    //        else
    //        {
    //            break ;
    //        }

    //    }

    //}

    ///// <summary>
    ///// 举手文本预设
    ///// </summary>
    //public GameObject HandTextPrefab;
    //public Transform HandTextParent;
    //public float fadeSpeed = 1.0f;
    //private float alpha = 1.0f;


    void Update()
    {
          GetText();
        if (!string.IsNullOrEmpty(strDanMu))
        {
            Displayer.AddBullet(strDanMu, CheckShowBox(), GetDirection());
        }
           
          strDanMu = "";   
        
    }
   

    private void  GetText()
    {
        //int textIndex = Random.Range(0, _textPool.Count);
        //var weightDict = new Dictionary<object, float>() {
        //    {"<color=yellow>{0}</color>", 10f},
        //    {"<color=red>{0}</color>", 2f},
        //    {"<color=white>{0}</color>", 80f}
        //};
        //string randomColor = (string)Utility.RandomObjectByWeight(weightDict);
        //string text = string.Format(randomColor, _textPool[textIndex]);

        if (TestSocket.instance.receiveMessage.Count == 0)
            return;

        var weightDict = new Dictionary<object, float>() {
            {"<color=yellow>{0}</color>", 10f},
            {"<color=red>{0}</color>", 2f},
            {"<color=white>{0}</color>", 80f}
        };
        string randomColor = (string)Utility.RandomObjectByWeight(weightDict);
        if (TestSocket.instance.receiveMessage.Count != 0)
        {
            strDanMu  = string.Format(randomColor, TestSocket.instance.receiveMessage.Dequeue());
           // Debug.Log(strDanMu);         
        }
    }
    private bool CheckShowBox()
    {
        var weightDict = new Dictionary<object, float>() {
            {true, 20f},
            {false, 80f}
        };
        bool ret = (bool)Utility.RandomObjectByWeight(weightDict);
        return ret;
    }
    private ScrollDirection GetDirection()
    {
        var weightDict = new Dictionary<object, float>() {
            {ScrollDirection.LeftToRight, 5f},
            {ScrollDirection.RightToLeft, 80f}
        };
        ScrollDirection direction = (ScrollDirection)Utility.RandomObjectByWeight(weightDict);
        return direction;
    }

  

   
}
