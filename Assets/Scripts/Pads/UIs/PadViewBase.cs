using System;
using Doozy.Engine;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using Framework;
using UnityEngine;


namespace Fxb.CPTTS
{
    public abstract class PadViewBase : MonoBehaviour
    {
        protected UIView doozyView;

        private UIView preDoozyView;
        /// <summary>
        /// Start调用为true
        /// </summary>
        protected bool hasInited;

        private CanvasGroup[] cgsCache;

        protected virtual void Start()
        {
            //占位    
            hasInited = true;
        }

        protected virtual void Awake()
        {
            doozyView = GetComponent<UIView>();

            doozyView.ShowBehavior.OnFinished.Event.AddListener(OnShowFinished);

            doozyView.ShowBehavior.OnStart.Event.AddListener(OnStartShow);

            doozyView.HideBehavior.OnStart.Event.AddListener(OnStartHide);

            doozyView.HideBehavior.OnFinished.Event.AddListener(OnHideFinished);
        }


        protected virtual void OnDisable()
        {
            Message.RemoveListener<UIPopupMessage>(OnPopupMessage);
        }

        /// <summary>
        /// 被隐藏  可以代替OnDisable
        /// </summary>
        private void OnHideFinished()
        {
            Message.RemoveListener<UIPopupMessage>(OnPopupMessage);
        }

        private void OnStartHide()
        {

        }

        protected virtual void OnShowFinished()
        {
            if (preDoozyView == null)
                preDoozyView = GetBottomView();

            if (preDoozyView != null && !preDoozyView.IsShowing)
                preDoozyView.Hide(true);

        }

        protected UIView GetBottomView()
        {
            //VisibleViews 可能乱序
            var preViewList = UIView.VisibleViews.FindAll(FindViewMatch);

            UIView preView = null;

            foreach (var item in preViewList)
            {
                if (item == doozyView)
                    continue;

                preView = item;
            }

            return preView;
        }

        private bool FindViewMatch(UIView view)
        {
            if (view.transform.parent != transform.parent)
                return false;

            return true;
        }

        /// <summary>
        /// 被显示  可以代替OnEnable
        /// </summary>
        protected virtual void OnStartShow()
        {
            Message.AddListener<UIPopupMessage>(OnPopupMessage);

        }

        private void OnPopupMessage(UIPopupMessage message)
        {
            switch (message.AnimationType)
            {
                case AnimationType.Show:
                    CtrCanvasGroupInteractable(false);
                    break;
                case AnimationType.Hide:
                    CtrCanvasGroupInteractable(true);
                    break;
                default:
                    break;
            }
        }

        protected void CtrCanvasGroupInteractable(bool enable)
        {
            if (cgsCache == null)
            {
                var masterCanvas = transform.GetComponentInParent<UICanvas>();

                DebugEx.AssertNotNull(masterCanvas, "未找到父层uicanvas");

                cgsCache = masterCanvas.GetComponentsInChildren<CanvasGroup>(false);
            }

            foreach (var cg in cgsCache)
            {
                if (cg.GetComponentInParent<UIPopup>() != null)
                    continue;

                cg.blocksRaycasts = enable;
            }

            if (enable)
                cgsCache = null;
        }

        /// <summary>
        /// 返回 子类重写
        /// </summary>
        public virtual void Back()
        {
            //if (GetTopView() == doozyView)
            doozyView?.Hide();

            preDoozyView?.Show(true);

            preDoozyView = null;
        }

        protected UIView GetTopView()
        {
            return UIView.VisibleViews.FindLast(FindViewMatch);
        }
    }
}
