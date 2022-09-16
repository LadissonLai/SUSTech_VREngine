using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class TaskItemView : MonoBehaviour
    {
        public TextMeshProUGUI title;

        public Button titleBtn;

        // public Transform progressMark;

        public Image levelIcon;

        public Transform progressMark;

        // TextMeshProUGUI levelTitle;

        public event Action<string> OnTitleBtnClicked;

        TaskCsvConfig taskCfg;

        public SpriteAtlas spriteAtlas;

        string id;

        // float middleColor = 86 / 255f;

        // Start is called before the first frame update
        void Start()
        {
            titleBtn.onClick.AddListener(TitleBtnClicked);
        }

        public void Refresh(string taskID, SpriteAtlas atlas = null)
        {
            // levelTitle = levelTitle ?? levelIcon.GetComponentInChildren<TextMeshProUGUI>();

            taskCfg = taskCfg ?? World.Get<TaskCsvConfig>();

            var data = taskCfg.FindRowDatas(taskID);

            //if (atlas != null)
            //    spriteAtlas = atlas;
            //else 
            //    Debug.Log("atlas is null");

            // titleBtn.image.sprite = spriteAtlas.GetSprite(data.Icon);

            title.text = data.Title;

            var taskModel = World.Get<ITaskModel>();

            if (taskModel == null || taskModel.IsSubmitAllTask || World.Get<DASceneState>().taskMode == DaTaskMode.GroupingMode)
                progressMark.gameObject.SetActive(false);
            else
                progressMark.gameObject.SetActive(taskModel.GetData()[0].taskID == taskID);

            LoadLevelIcon(data.Level);

            id = taskID;
        }

        void TitleBtnClicked()
        {
            OnTitleBtnClicked?.Invoke(id);
        }

        void LoadLevelIcon(float level)
        {
            string iconName = null;

            switch (level)
            {
                case 1:
                case 0: // 没填就默认1
                    iconName = "level1";
                    break;

                case 2:
                    iconName = "level2";
                    break;

                case 3:
                    iconName = "level3";
                    break;

                case 4:
                    iconName = "level4";
                    break;

                case 5:
                    iconName = "level5";
                    break;

                default:
                    Debug.LogError($"{level}- 无效的任务等级");
                    return;
            }

            levelIcon.sprite = spriteAtlas.GetSprite(iconName);
        }
    }

}