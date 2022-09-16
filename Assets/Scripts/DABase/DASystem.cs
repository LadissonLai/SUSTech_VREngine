using Doozy.Engine;
using Framework;
using Framework.Tools;
using Fxb.DA;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class DASystem : MonoBehaviour
    {
        private List<DAAbstractStep> steps;
         
        private DAState DaState { get => World.Get<DAState>();}

        private void OnDestroy()
        {
            DAAbstractStep.cmsListPool?.Dispose();
            DAAbstractStep.cmsListPool = null;
             
            Message.RemoveListener<StartDAModeMessage>(OnObjStartDAProcessMessage);
            Message.RemoveListener<DAProcessObjMessage>(OnDAProcessObjMessage);
            Message.RemoveListener<CancelDAStepMessage>(OnCancelDAStepMessage);
            Message.RemoveListener<DAObjProcessingMessage>(OnObjProcessingMessage);

            World.current.Injecter.UnRegist<DAState>();            
        }

        private void Awake()
        {
            if (DAAbstractStep.cmsListPool == null)
                DAAbstractStep.cmsListPool = new ObjectPool<List<AbstractDAObjCtr>>(3, null, list => list.Clear());
            
            Message.AddListener<StartDAModeMessage>(OnObjStartDAProcessMessage);
            Message.AddListener<DAProcessObjMessage>(OnDAProcessObjMessage);
            Message.AddListener<CancelDAStepMessage>(OnCancelDAStepMessage);
            Message.AddListener<DAObjProcessingMessage>(OnObjProcessingMessage);

            World.current.Injecter.Regist<DAState>();
        }

        private void OnObjProcessingMessage(DAObjProcessingMessage message)
        {
            var obj = message.target;

            if (obj.IsProcessing)
                DaState.processingObjs.AddUnique(obj);
            else
                DaState.processingObjs.Remove(obj);
        }
         
        private void OnCancelDAStepMessage(CancelDAStepMessage message)
        {
            if (steps == null)
                return;

            foreach (var step in steps)
            {
                step.Cancel();
            }

            steps = null;

            DaState.isRunning = false;
        }

        private void OnDAProcessObjMessage(DAProcessObjMessage message)
        {
            //此消息弃用
            Debug.LogError("弃用消息 DAProcessObjMessage");
        }

        private void OnObjStartDAProcessMessage(StartDAModeMessage message)
        {
            if (steps != null)
            {
                foreach (var step in steps)
                {
                    step.Exit();

                    TickStep(step);
                }

                steps = null;
            }

            steps = new List<DAAbstractStep>();

            foreach (var obj in message.rootCtrs)
            {
                var step = new DisassembleAssembleStep();

                step.Start(obj);

                steps.Add(step);
            }

            DaState.isRunning = true;
        }
 
        private void TickStep(DAAbstractStep step)
        {
            if (step != null)
            {
                if (step.MoveNext())
                {
                }
            }
        }

        void Update()
        {
            if (steps == null)
                return;

            foreach (var step in steps)
            {
                TickStep(step);
            }
        }
 
    }//CmsDASystem end
}
