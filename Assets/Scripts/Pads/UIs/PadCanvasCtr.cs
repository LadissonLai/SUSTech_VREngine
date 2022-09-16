using Doozy.Engine;
using Doozy.Engine.UI;
using Framework;
using Fxb.CMSVR;
using Fxb.CPTTS;
using UnityEngine;
using UnityEngine.UI;

public class PadCanvasCtr : MonoBehaviour
{
    public Button btnBack;

    public Button btnQuit;

    public Button btnHome;

    public HomeView homeView;

    // Start is called before the first frame update
    private void Awake()
    {
        btnBack.onClick.AddListener(OnBackClick);

        btnQuit.onClick.AddListener(OnQuitClick);

        btnHome.onClick.AddListener(OnHomeClick);

        Message.AddListener<UIViewMessage>(OnUIViewMessage);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<UIViewMessage>(OnUIViewMessage);
    }

    private void OnUIViewMessage(UIViewMessage msg)
    {
        if (msg.View.ViewCategory != DoozyNamesDB.VIEW_CATEGORY_PAD)
            return;

        switch (msg.Type)
        {
            case UIViewBehaviorType.Show:
                //msg.View.transform.SetAsLastSibling();

                if (IsTargetView(UIView.VisibleViews[UIView.VisibleViews.Count - 1],
       DoozyNamesDB.VIEW_PAD_HOMEVIEW))
                {
                    homeView.TryExchangeTaskBtn();

                    homeView.TryExchangeRecordBtn();
                }

                break;

            case UIViewBehaviorType.Hide:

                //if (IsTargetView(UIView.VisibleViews[UIView.VisibleViews.Count - 1],
                //    DoozyNamesDB.VIEW_PAD_HOMEVIEW))
                //    homeView.TryExchangeTaskBtn();

                break;
            default:
                break;
        }
    }

    private void OnHomeClick()
    {
        for (int i = UIView.VisibleViews.Count - 1; i >= 0; i--)
        {
            var view = UIView.VisibleViews[i];

            if (!IsHideAblePadView(view))
                continue;

            view.Hide();
        }

        UIView.ShowView(DoozyNamesDB.VIEW_CATEGORY_PAD, DoozyNamesDB.VIEW_PAD_HOMEVIEW, true);
    }

    private void OnQuitClick()
    {
        var alert = UIPopupManager.ShowPopup(DoozyNamesDB.POPUP_NAME_YESORNO, false, false)
            .GetComponent<YesOrNoPopup>();

        alert.UpdateMsg(new YesOrNoPopup.Data()
        {
            title = "退出",
            msg = "是否退出?",
            enterBtnText = "是",
            cancelBtnText = "否"
        });

        alert.OnEntrerBtnClick += (param) => Application.Quit();
    }

    private void OnBackClick()
    {
        var topView = GetTopView();

        if (topView == null)
            return;

        if (IsTargetView(topView, DoozyNamesDB.VIEW_PAD_TASKDETAIL) ||
            IsTargetView(topView, DoozyNamesDB.VIEW_PAD_RECORD))
        {
            OnHomeClick();

            return;
        }

        topView.GetComponent<PadViewBase>().Back();

        //topView.Hide();
    }

    protected UIView GetTopView()
    {
        return UIView.VisibleViews.FindLast(IsHideAblePadView);
    }

    private bool IsHideAblePadView(UIView view)
    {
        if (view.ViewCategory != DoozyNamesDB.VIEW_CATEGORY_PAD)
            return false;

        if (view.ViewName == DoozyNamesDB.VIEW_PAD_HOMEVIEW)
            return false;

        return true;
    }

    bool IsTargetView(UIView view, string viewName)
    {
        if (view.ViewCategory != DoozyNamesDB.VIEW_CATEGORY_PAD)
            return false;

        return view.ViewName == viewName;
    }
}
