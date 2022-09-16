using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Fxb.CMSVR
{
    public class RepairToolVideoCtr : MonoBehaviour
    {
        private bool isPlay;
        public GameObject videoObj;
        private VideoPlayer videoPlayer; 
        void Start()
        {
            videoPlayer = videoObj.GetComponent<VideoPlayer>();
            videoObj.SetActive(false);
    
            Message.AddListener<RepairToolVideoPlayState>(OnRepairToolVideoPlayState);
            videoPlayer.loopPointReached += PlayvideoLoop;
        }

        private void PlayvideoLoop(VideoPlayer source)
        {
            videoObj.SetActive(false);
            isPlay = false;
        }

        private void OnRepairToolVideoPlayState(RepairToolVideoPlayState obj)
        {
            videoObj.SetActive(!isPlay);
            isPlay = !isPlay;
        }

        private void OnDestroy()
        {
            Message.RemoveListener<RepairToolVideoPlayState>(OnRepairToolVideoPlayState);
            videoPlayer.loopPointReached -= PlayvideoLoop;
        }
    }
    public class RepairToolVideoPlayState : Message{}
}
