using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using LitJson;
using System.IO;
using Doozy.Engine.UI;
using Framework;
using Fxb.CPTTS;
using Doozy.Engine;


namespace Fxb.CMSVR
{
    public class RecordTableScript : MonoBehaviour
    {

        public GameObject Content;
        public GameObject PageTitle;
        public GameObject RecordTitle;
        public GameObject RecordContent;
        public GameObject RecordError;
        public GameObject ScoreContent;
        public GameObject CompleteOperationSteps;
        public GameObject OperationSteps;
        private int maxPageIndex;
        private List<GameObject> records;
        IRecordModel recordModel;
        ITaskModel taskModel;
        private void OnShow(ShowRecordMessage msg) {
            recordModel = World.Get<IRecordModel>();
            taskModel = World.Get<ITaskModel>();
            if(int.Parse(taskModel.GetData()[0].taskID) > 0) {
                loadScreen();
            }
        }
        void Awake() {
            Message.AddListener<ShowRecordMessage>(OnShow);
        }

        private void OnDestroy() {
            Message.RemoveListener<ShowRecordMessage>(OnShow);
        }
        void Start() {
            this.gameObject.SetActive(false);
            records = new List<GameObject>();
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSeconds(0.1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(Content.GetComponent<RectTransform>());
        }

        // 暴露的接口
        public void loadScreen() {
            // initialize
            if(OperationSteps) {
                OperationSteps.SetActive(false);
            }
            if(CompleteOperationSteps) {
                CompleteOperationSteps.SetActive(false);
            }
            this.gameObject.SetActive(true);
            foreach(var item in records) {
                Destroy(item);
            }
            var curPage = taskModel.GetData()[0];
            PageTitle.GetComponent<Text>().text = curPage.taskTitle;
            // 用于计数现在多少条，起始一条为offset
            int recordCount = 1;
            double score = 100;
            var stepGroups = curPage.stepGroups;
            foreach(var stepGroup in stepGroups) {
                // section的标题
                GameObject tmpTitle = Instantiate(RecordTitle, Content.transform) as GameObject;
                foreach(var item in tmpTitle.GetComponentsInChildren<Text>()) {
                    if(item.name == "Title") {
                        item.text = taskModel.GetStepGroupDescription(stepGroup.id);
                    }
                    if(item.name == "TotalScore") {
                        item.text = "(" + stepGroup.score + "分)";
                    }  
                }
                foreach(Transform t in tmpTitle.GetComponentsInChildren<Transform>()) {
                    if(t.name == "TotalScore") {
                        t.GetComponent<RectTransform>().anchoredPosition = new Vector2(70 + taskModel.GetStepGroupDescription(stepGroup.id).Length * 18, t.GetComponent<RectTransform>().anchoredPosition.y);
                    }  
                }
                tmpTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                records.Add(tmpTitle);
                recordCount++;

                // section的内容
                // Debug.Log("gsd" + taskModel.GetStepGroupDescription(stepGroup.id));
                foreach(var item in taskModel.GetChildStepIDs(stepGroup.id)) {
                    GameObject tmpContent = Instantiate(RecordContent, Content.transform) as GameObject;
                    tmpContent.GetComponentInChildren<Text>().text = recordModel.FindRecord(item).Title;
                    tmpContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                    records.Add(tmpContent);
                    recordCount++;
                }


                // section的error
                foreach(var record in taskModel.GetChildStepIDs(stepGroup.id)) {
                    var error = recordModel.GetRecordAllErrors(record);
                    var punishment = recordModel.GetRecordErrorScoreDeducting(record).ToString();
                    GameObject tmpError = Instantiate(RecordError, Content.transform) as GameObject;
                    foreach(var item in tmpError.GetComponentsInChildren<Text>()) {
                        if(item.name == "Title") {
                            item.text = error;
                        }
                        if(item.name == "Score") {
                            item.text = punishment;
                            score += float.Parse(punishment);
                        }  
                    }
                    tmpError.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * recordCount);
                    records.Add(tmpError);
                    recordCount++;
                    Debug.Log("gsd" + recordModel.GetRecordAllErrors(record));
                }
            }

            Content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            StartCoroutine(RebuildLayout());
            // 总成绩
            ScoreContent.GetComponent<Text>().text = score.ToString();
        }
    
    }
}