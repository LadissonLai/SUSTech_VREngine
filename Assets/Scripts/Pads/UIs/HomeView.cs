using System;
using Doozy.Engine;
using Doozy.Engine.UI;
using Framework;
using Fxb.CMSVR;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CPTTS
{
    public class HomeView : PadViewBase
    {
        public Button[] menuMapBtns;

        public UIView[] menuMapViews;

        public Button recordBtn;

        public Button taskDetailBtn;

        public Button structureBtn, repairToolLearnBtn;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            Debug.Assert(menuMapBtns.Length == menuMapViews.Length);

            for (var i = 0; i < menuMapBtns.Length; i++)
            {
                var btn = menuMapBtns[i];

                var _i = i;

                btn.onClick.AddListener(() =>
                {
                    menuMapViews[_i].Show();
                });
            }

            doozyView.Show(true);

            structureBtn.onClick.AddListener(OnStructurePlayAni);

            repairToolLearnBtn.onClick.AddListener(OnRepairToolPlayVideo);
        }

        private void OnRepairToolPlayVideo()
        {
            Message.Send(new RepairToolVideoPlayState { });
        }

        private void OnStructurePlayAni()
        {
            Message.Send(new StructureAniStateMessage
            {
                isAniExpansion =! World.Get<DASceneState>().isStructureAniCompleted
            });
        }

        public void TryExchangeTaskBtn()
        {
            if (World.Get<DASceneState>().taskMode == DaTaskMode.GroupingMode)
                return;

            if (World.Get<ITaskModel>() != null)
                taskDetailBtn.gameObject.SetActive(true);
        }

        public void TryExchangeRecordBtn()
        {
            if (World.Get<DASceneState>().taskMode == DaTaskMode.GroupingMode)
                return;

            var task = World.Get<ITaskModel>();

            if (task != null && task.IsSubmitAllTask)
                recordBtn.gameObject.SetActive(true);
        }
    }
}