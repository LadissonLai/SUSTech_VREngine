// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Events
{
    /// <summary>
    ///     Event-like class that allows interacting with an Animator's parameters in a dynamic way.
    /// </summary>
    [Serializable]
    public class AnimatorEvent
    {
        #region Enums

        /// <summary>
        ///     Types of parameters that an Animator can have
        /// </summary>
        public enum ParameterType
        {
            /// <summary>
            ///     Bool parameter
            /// </summary>
            Bool,

            /// <summary>
            ///     Float parameter
            /// </summary>
            Float,

            /// <summary>
            ///     Int parameter
            /// </summary>
            Int,

            /// <summary>
            ///     Trigger parameter
            /// </summary>
            Trigger
        }

        #endregion

        #region Public Variables

        /// <summary> Target Animator </summary>
        public Animator Animator;

        /// <summary> Value assigned to the target parameter if the TargetParameterType is set to ParameterType.Bool </summary>
        public bool BoolValue;

        /// <summary> Value assigned to the target parameter if the TargetParameterType is set to ParameterType.Float </summary>
        public float FloatValue;

        /// <summary> Value assigned to the target parameter if the TargetParameterType is set to ParameterType.Int </summary>
        public int IntValue;

        /// <summary> Name of the parameter (trigger) used to assign the value, if the TargetParameterType is set to ParameterType.Trigger </summary>
        public string ParameterName;

        /// <summary> If enabled and the TargetParameterType is set to ParameterType.Trigger, ResetTrigger will get performed on the Animator before setting the new trigger value </summary>
        public bool ResetTrigger;

        /// <summary> Target parameter type </summary>
        public ParameterType TargetParameterType;

        #endregion

        #region Constructors

        /// <summary> Constructs a new AnimatorEvent with default values </summary>
        public AnimatorEvent() { Reset(); }

        /// <summary> Constructs a new AnimatorEvent and initializes it to target a parameter name and set its bool value </summary>
        /// <param name="animator"> Target Animator </param>
        /// <param name="parameterName"> The parameter name </param>
        /// <param name="boolValue"> The parameter value </param>
        public AnimatorEvent(Animator animator, string parameterName, bool boolValue)
        {
            Reset();

            TargetParameterType = ParameterType.Bool;

            Animator = animator;
            ParameterName = parameterName;
            BoolValue = boolValue;
        }

        /// <summary> Constructs a new AnimatorEvent and initializes it to target a parameter name and set its int value </summary>
        /// <param name="animator"> Target Animator </param>
        /// <param name="parameterName"> The parameter name </param>
        /// <param name="intValue"> The parameter value </param>
        public AnimatorEvent(Animator animator, string parameterName, int intValue)
        {
            Reset();

            TargetParameterType = ParameterType.Int;

            Animator = animator;
            ParameterName = parameterName;
            IntValue = intValue;
        }

        /// <summary> Constructs a new AnimatorEvent and initializes it to target a parameter name and set its float value </summary>
        /// <param name="animator"> Target Animator </param>
        /// <param name="parameterName"> The parameter name </param>
        /// <param name="floatValue"> The parameter value </param>
        public AnimatorEvent(Animator animator, string parameterName, float floatValue)
        {
            Reset();

            TargetParameterType = ParameterType.Float;

            Animator = animator;
            ParameterName = parameterName;
            FloatValue = floatValue;
        }

        /// <summary> Constructs a new AnimatorEvent and initializes it to target a parameter name that is the Trigger name </summary>
        /// <param name="animator"> Target Animator </param>
        /// <param name="parameterName"> The parameter name (Trigger name) </param>
        public AnimatorEvent(Animator animator, string parameterName)
        {
            Reset();

            TargetParameterType = ParameterType.Trigger;

            Animator = animator;
            ParameterName = parameterName;
        }

        #endregion

        #region Public Methods

        /// <summary> Invokes the AnimatorEvent </summary>
        /// <param name="callback"> Callback executed with false if the Animator is null, or true otherwise </param>
        public void Invoke(UnityAction<bool> callback = null)
        {
            if (Animator == null)
            {
                InvokeCallback(callback, false);
                return;
            }

            SetValue();

            InvokeCallback(callback, true);
        }
        
        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            Animator = null;
            TargetParameterType = ParameterType.Trigger;
            ParameterName = "";
            BoolValue = false;
            FloatValue = 0;
            IntValue = 0;
            ResetTrigger = false;
        }

        /// <summary> Sets the selected value, in the Animator, as defined by the TargetParameterType, ParameterName and set value (BoolValue, FloatValue, IntValue or Trigger) </summary>
        public void SetValue()
        {
            switch (TargetParameterType)
            {
                case ParameterType.Bool:
                    Animator.SetBool(ParameterName, BoolValue);
                    break;
                case ParameterType.Float:
                    Animator.SetFloat(ParameterName, FloatValue);
                    break;
                case ParameterType.Int:
                    Animator.SetInteger(ParameterName, IntValue);
                    break;
                case ParameterType.Trigger:
                    if (ResetTrigger) Animator.ResetTrigger(ParameterName);
                    Animator.SetTrigger(ParameterName);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Static Methods

        /// <summary> Invokes the passed callback with the given value. If callback is null, nothing will happen. </summary>
        /// <param name="callback"> Target callback </param>
        /// <param name="value"> Passed value </param>
        private static void InvokeCallback(UnityAction<bool> callback, bool value)
        {
            if (callback == null) return;
            callback.Invoke(value);
        }

        #endregion
    }
}