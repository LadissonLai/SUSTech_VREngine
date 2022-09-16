using Framework;
using Fxb.SpawnPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class TaskErrorItemView : MonoBehaviour
    {
        public TextMeshProUGUI dockedScore;

        TextMeshProUGUI title;

        IRecordModel recordModel;

        string curID;

        int curIndex;

        float curStepScore;

        // Start is called before the first frame update
        void Start()
        {
            recordModel = World.Get<IRecordModel>();

            title = GetComponent<TextMeshProUGUI>();

            Refresh();
        }

        public void Init(string stepID, int index, float stepScore)
        {
            curIndex = index;

            curID = stepID;

            curStepScore = stepScore;

            if (recordModel == null)
                return;

            Refresh();
        }

        void Refresh()
        {
            string error;

            string score;

            if (recordModel.CheckRecordCompleted(curID))
            {
                error = recordModel.GetRecordAllErrors(curID);

                score = recordModel.GetRecordErrorScoreDeducting(curID).ToString();
            }
            else
            {
                error = "未做";

                score = curStepScore.ToString();
            }

            title.text = $"{curIndex}.{error}";

            dockedScore.text = $"-{score}";
        }
    }

}