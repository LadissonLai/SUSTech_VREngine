// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Soundy
{
    /// <inheritdoc />
    /// <summary>
    ///     Dynamic sound pool manager for SoundyControllers
    /// </summary>
    [DefaultExecutionOrder(DoozyExecutionOrder.SOUNDY_POOLER)]
    public class SoundyPooler : MonoBehaviour
    {
        #region Static Properties

        /// <summary> Returns a reference to the SoundyPooler component that should be attached to the SoundyManager GameObject. If one does not exist, it gets added. </summary>
        public static SoundyPooler Instance { get { return SoundyManager.Pooler; } }

        /// <summary> Internal variable that holds a list of available SoundyControllers </summary>
        private static List<SoundyController> s_pool;

        /// <summary> The list of available SoundyControllers that make up the pool </summary>
        private static List<SoundyController> Pool { get { return s_pool ?? (s_pool = new List<SoundyController>()); } set { s_pool = value; } }

        /// <summary> Auto kill any SoundyControllers that are have been unused for the set idle kill duration </summary>
        public static bool AutoKillIdleControllers { get { return SoundySettings.Instance.AutoKillIdleControllers; } }

        /// <summary> The duration a SoundyController needs to be idle to be considered killable </summary>
        public static float ControllerIdleKillDuration { get { return SoundySettings.Instance.ControllerIdleKillDuration; } }

        /// <summary> Time interval to check for idle SoundyControllers to kill them </summary>
        public static float IdleCheckInterval { get { return SoundySettings.Instance.IdleCheckInterval; } }

        /// <summary> The minimum number of SoundyControllers that should be available in the pool, that will not get automatically killed even if they are killable </summary>
        public static int MinimumNumberOfControllers { get { return SoundySettings.Instance.MinimumNumberOfControllers; } }

        #endregion

        #region Properties

        private bool DebugComponent { get { return DoozySettings.Instance.DebugSoundyPooler; } }

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the coroutine that performs the check for idle controllers </summary>
        private Coroutine m_idleCheckCoroutine;

        /// <summary> Wait interval between used by the coroutine that performs the check for idle controllers </summary>
        private WaitForSecondsRealtime m_idleCheckIntervalWaitForSecondsRealtime;

        /// <summary> Internal variable used as a reference holder to minimise memory allocations </summary>
        private SoundyController m_tempController;

        #endregion

        #region Unity Methods

        private void Reset() { SoundySettings.Instance.ResetComponent(this); }

        private void OnEnable()
        {
            if (!AutoKillIdleControllers) return;
            StartIdleCheckInterval();
        }

        private void OnDisable() { StopIdleCheckInterval(); }

        #endregion

        #region Static Methods

        /// <summary> Stop all SoundyControllers from playing, destroy the GameObjects they are attached to and clear the Pool </summary>
        /// <param name="keepMinimumNumberOfControllers"> Should there be a minimum set number of controllers in the pool left after clearing? </param>
        public static void ClearPool(bool keepMinimumNumberOfControllers = false)
        {
            if (keepMinimumNumberOfControllers)
            {
                RemoveNullControllersFromThePool();           //remove any null controllers (sanity check)
                if (Pool.Count <= MinimumNumberOfControllers) //make sure the minimum number of controllers are in the pool before killing them
                {
                    if (Instance.DebugComponent) DDebug.Log("Clear Pool - " + Pool.Count + " Controllers Available", Instance);
                    return;
                }

                int killedControllersCount = 0;
                for (int i = Pool.Count - 1; i >= MinimumNumberOfControllers; i--) //go through the pool
                {
                    SoundyController controller = Pool[i];
                    Pool.Remove(controller); //remove controller from the pool
                    controller.Kill();       //kill the controller
                    killedControllersCount++;
                }

                if (Instance.DebugComponent) DDebug.Log("Clear Pool - Killed " + killedControllersCount + " Controllers - " + Pool.Count + "' Controllers Available", Instance);

                return;
            }

            SoundyController.KillAll();
            Pool.Clear();
            if (Instance.DebugComponent) DDebug.Log("Clear Pool - Killed All Controllers - " + Pool.Count + " Controllers Available", Instance);
        }

        /// <summary> Get a SoundyController from the Pool, or create a new one if all the available controllers are in use </summary>
        public static SoundyController GetControllerFromPool()
        {
            RemoveNullControllersFromThePool();
            if (Pool.Count <= 0) PutControllerInPool(SoundyController.GetController()); //the pool does not have any controllers in it -> create and return a new controller
            SoundyController controller = Pool[0];                        //assign the first found controller
            Pool.Remove(controller);                                      //remove the assigned controller from the pool
            controller.gameObject.SetActive(true);
            return controller; //return a reference to the controller
        }

        /// <summary> Create a set number of SoundyControllers and add them to the Pool </summary>
        /// <param name="numberOfControllers"> How many controllers should be created </param>
        public static void PopulatePool(int numberOfControllers)
        {
            RemoveNullControllersFromThePool();
            if (numberOfControllers < 1) return; //sanity check
            for (int i = 0; i < numberOfControllers; i++)
                PutControllerInPool(SoundyController.GetController());
            if (Instance.DebugComponent) DDebug.Log("Populate Pool - Added " + numberOfControllers + " Controllers to the Pool - " + Pool.Count + " Controllers Available", Instance);
        }

        /// <summary> Put a SoundyController back in the Pool </summary>
        /// <param name="controller"> The controller </param>
        public static void PutControllerInPool(SoundyController controller)
        {
            if (controller == null) return;
            if (!Pool.Contains(controller)) Pool.Add(controller);
            controller.gameObject.SetActive(false);
            controller.transform.SetParent(Instance.transform);
            if (Instance.DebugComponent) DDebug.Log("Put '" + controller.name + "' Controller in the Pool - " + Pool.Count + " Controllers Available", Instance);
        }

        #endregion

        #region Private Methods

        private void StartIdleCheckInterval()
        {
            if (Instance.DebugComponent) DDebug.Log("Start Idle Check", Instance);
            m_idleCheckIntervalWaitForSecondsRealtime = new WaitForSecondsRealtime(IdleCheckInterval < 0 ? 0 : IdleCheckInterval);
            m_idleCheckCoroutine = StartCoroutine(KillIdleControllersEnumerator());
        }

        private void StopIdleCheckInterval()
        {
            if (Instance.DebugComponent) DDebug.Log("Stop Idle Check", Instance);
            if (m_idleCheckCoroutine == null) return;
            StopCoroutine(m_idleCheckCoroutine);
            m_idleCheckCoroutine = null;
        }

        /// <summary> Removes any null controllers from the pool </summary>
        private static void RemoveNullControllersFromThePool() { Pool = Pool.Where(p => p != null).ToList(); }

        #endregion

        #region Enumerators

        private IEnumerator KillIdleControllersEnumerator()
        {
            while (AutoKillIdleControllers)
            {
                yield return m_idleCheckIntervalWaitForSecondsRealtime; //check idle wait interval
                RemoveNullControllersFromThePool();                     //remove any null controllers (sanity check)
                int minimumNumberOfControllers = MinimumNumberOfControllers > 0 ? MinimumNumberOfControllers : 0;
                float controllerIdleKillDuration = ControllerIdleKillDuration > 0 ? ControllerIdleKillDuration : 0;
                if (Pool.Count <= minimumNumberOfControllers) continue;            //make sure the minimum number of controllers are in the pool before killing any more of them
                for (int i = Pool.Count - 1; i >= minimumNumberOfControllers; i--) //go through the pool
                {
                    m_tempController = Pool[i];
                    if (m_tempController.gameObject.activeSelf) continue;                     //controller is active do not kill it
                    if (m_tempController.IdleDuration < controllerIdleKillDuration) continue; //controller is not killable as it has not been idle for long enough
                    Pool.Remove(m_tempController);                                            //remove controller from the pool
                    m_tempController.Kill();                                                  //kill the controller
                }
            }

            m_idleCheckCoroutine = null;
        }

        #endregion
    }
}