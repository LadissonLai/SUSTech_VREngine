using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Doozy.Engine.UI;
using Fxb.CPTTS;
using System;

public class YesOrNoPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI msgText,m_Title,m_EnterBtnText, m_CancelBtnText;

    [SerializeField]
    private Button enterButton,cancelButton;

    [SerializeField]
    private UIPopup popup;

    public event Action<int> OnEntrerBtnClick;

    public struct Data
    {
        public string title;

        public string msg;

        public string enterBtnText;

        public string cancelBtnText;
    }

    protected void Awake()
    {
        enterButton.onClick.AddListener(() => {
            StartCoroutine(Enter());
        });

        cancelButton.onClick.AddListener(() =>
        {
            StartCoroutine(Hide());
        });

    }
    
    public void UpdateMsg(Data data)
    {
        m_Title.text=data.title;

        msgText.text = data.msg;

        m_EnterBtnText.text = data.enterBtnText;

        m_CancelBtnText.text = data.cancelBtnText;
    }
    IEnumerator Hide()
    {
        yield return new WaitForSeconds(0.02f);
        
        popup.Hide();
    }

    IEnumerator Enter()
    {
        yield return new WaitForSeconds(0.02f);

        OnEntrerBtnClick?.Invoke(0);

        popup.Hide();
    }
}
