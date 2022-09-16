using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Doozy.Engine.UI;
using Fxb.CPTTS;
using System;

public class ModelInfoPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI msgText,m_Title;

    [SerializeField]
    private RawImage msgImg;

    [SerializeField]
    private Button enterButton;

    [SerializeField]
    private UIPopup popup;
    [SerializeField]
    private AspectRatioFitter aspectRatio;
    [SerializeField]
    ContentSizeFitter contentSizeFitter;  

    public struct Data
    {
        public string msg;

        public string imgPath;

        public string title;
      
    }

    protected void Awake()
    {
        enterButton.onClick.AddListener(() => {
            StartCoroutine(Hide());
        });
      
       
    }
    
    public void UpdateMsg(Data data)
    {
        m_Title.text=data.title;
        msgText.gameObject.SetActive(!string.IsNullOrEmpty(data.msg));
        msgText.text = "     "+data.msg;

        msgImg.gameObject.SetActive(!string.IsNullOrEmpty(data.imgPath));

        if(!string.IsNullOrEmpty(data.imgPath))
        {
            msgImg.texture = Resources.Load<Texture2D>(data.imgPath);

            aspectRatio.aspectRatio = msgImg.texture.width / (float)msgImg.texture.height;
        }

        StartCoroutine(OncontentSizeFitter());

    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(0.02f);
        popup.Hide();
    }

    IEnumerator OncontentSizeFitter()
    {
        contentSizeFitter.enabled = false;
        yield return new WaitForSeconds(0.02f);
        contentSizeFitter.enabled = true;
    }

}
