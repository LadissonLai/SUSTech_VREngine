using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using LitJson;
using System.IO;
using Doozy.Engine;
using Framework;
using Fxb.CPTTS;

namespace Fxb.CMSVR
{
    public class OperationStepsScript : MonoBehaviour
    {
        public GameObject Step;
        public GameObject Content;
        public GameObject CompleteOperationSteps;
        public GameObject RecordTable;
        private List<GameObject> steps;
        private int doneCount = 0;
        private Color DarkGray = Color.grey;
        private Color Black = Color.black;
        private bool hasFirstUndo = false;
        ITaskModel taskModel;
        IRecordModel recordModel;
        // Start is called before the first frame update
        void Awake() {
            Message.AddListener<PrepareTaskMessage>(OnprepareTaskMessage);
            Message.AddListener<RefreshRecordItemStateMessage>(Onrefresh);
        }
        
        void OnDestroy(){
            Message.RemoveListener<PrepareTaskMessage>(OnprepareTaskMessage);
            Message.RemoveListener<RefreshRecordItemStateMessage>(Onrefresh);
            foreach(var item in steps) {
                Destroy(item);
            }
            steps.Clear();
        }
        void Start() {
            this.gameObject.SetActive(false);
            steps = new List<GameObject>();
        }

        void OnprepareTaskMessage(PrepareTaskMessage msg) {
            loadScreen(true);
        }
        void Onrefresh(RefreshRecordItemStateMessage msg) {
            loadScreen(false);
        }

        private void checkStepState(GameObject step, bool done) {
            var imgColor = step.GetComponentInChildren<Image>().color;
            // done = true;
            if(done) {
                doneCount++;
                // 颜色设置为灰色，显示√
                step.GetComponentInChildren<Text>().color = DarkGray;
                step.GetComponentInChildren<Image>().color = new Color(imgColor.r, imgColor.g, imgColor.b,255);
            } else {
                // 颜色设置为黑色，隐藏√
                step.GetComponentInChildren<Text>().color = Black;
                step.GetComponentInChildren<Image>().color = new Color(imgColor.r, imgColor.g, imgColor.b,0);
            }
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSeconds(0.1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(Content.GetComponent<RectTransform>());
        }

        // 暴露出的接口

        // 加载指定页面
        public void loadScreen(bool isInitial) {
            taskModel = World.Get<ITaskModel>();
            recordModel = World.Get<IRecordModel>();
            if(int.Parse(taskModel.GetData()[0].taskID) <= 0) {
                return;
            }
            // initialize
            if(CompleteOperationSteps) {
                CompleteOperationSteps.SetActive(false);
            }
            if(RecordTable) {
                RecordTable.SetActive(false);
            }
            this.gameObject.SetActive(true);
            foreach(var item in steps) {
                Destroy(item);
            }
            steps.Clear();
            doneCount = 0;
            hasFirstUndo = false;
            var curPage = taskModel.GetData()[0];
            var stepGroups = curPage.stepGroups;
            // load new steps
            foreach(var stepGroup in stepGroups) {
                foreach(var stepID in taskModel.GetChildStepIDs(stepGroup.id)) {
                    GameObject tmpStep = Instantiate(Step, Content.transform) as GameObject;
                    var texts = tmpStep.GetComponentsInChildren<Text>();
                    texts[0].text = (steps.Count + 1).ToString() + ". ";
                    texts[1].text = recordModel.FindRecord(stepID).Title;
                    if(!recordModel.CheckRecordCompleted(recordModel.FindRecord(stepID).ID) && !hasFirstUndo) {
                        texts[0].fontSize = 21;
                        texts[0].fontStyle = FontStyle.Bold;
                        texts[1].fontSize = 21;
                        texts[1].fontStyle = FontStyle.Bold;
                        hasFirstUndo = true;
                    }
                    checkStepState(tmpStep, !isInitial && recordModel.CheckRecordCompleted(recordModel.FindRecord(stepID).ID));
                    // tmpStep.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 - (30) * steps.Count);
                    steps.Add(tmpStep);
                }
            }

            Content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            StartCoroutine(RebuildLayout());

            LayoutRebuilder.ForceRebuildLayoutImmediate(Content.GetComponent<RectTransform>());

            // Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x, 30 * stepGroups.Count);
            foreach (var item in GetComponentsInChildren<Text>()) {
                if(item.name == "Title") {
                    item.text = curPage.taskTitle;
                }
                if(item.name == "Description") {
                    item.text = "操作步骤";
                }
                if(item.name == "DoneCount") {
                    item.text = doneCount.ToString() + "/" + steps.Count;
                }
            }
        }
    }
}