// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Settings;
using Doozy.Engine.Soundy;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Input;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Doozy.Engine.UI.Base
{
	/// <inheritdoc />
	/// <summary> Base class for UI Components </summary>
	/// <typeparam name="T"> The component type that implements it </typeparam>
	/// <seealso cref="T:UnityEngine.MonoBehaviour" />
	public abstract class UIComponentBase<T> : MonoBehaviour
	{
		#region UNITY_EDITOR

#if UNITY_EDITOR

		/// <summary>
		///     Method used when creating an UIComponent.
		///     It looks if the selected object has a RectTransform (and returns it as the parent).
		///     If the selected object is null or does not have a RectTransform, it returns the MasterCanvas GameObject as the parent.
		/// </summary>
		/// <param name="selectedObject"> Selected object </param>
		protected static GameObject GetParent(GameObject selectedObject)
		{
			if (selectedObject == null) return UICanvas.GetMasterCanvas().gameObject; //selected object is null -> returns the MasterCanvas GameObject
			return selectedObject.GetComponent<RectTransform>() != null ? selectedObject : UICanvas.GetMasterCanvas().gameObject;
		}

		/// <summary>
		///     Method used when creating an UIComponent that needs to be under an UICanvas directly.
		///     It looks if the selected object has an UICanvas component (and returns it.
		///     If the selected object does not have an UICanvas component it searches for one at it's hierarchy root.
		///     If the selected object is null or no UICanvas was found, it returns the MasterCanvas GameObject to be used as a parent
		/// </summary>
		/// <param name="selectedObject"> Selected object </param>
		protected static GameObject GetCanvasAsParent(GameObject selectedObject)
		{
			if (selectedObject == null) return UICanvas.GetMasterCanvas().gameObject; //selected object is null -> returns the MasterCanvas GameObject
			if (selectedObject.GetComponent<UICanvas>()) return selectedObject; //selected object has an UICanvas -> returns its GameObject
			if (selectedObject.transform.root.GetComponent<UICanvas>()) return selectedObject.transform.root.gameObject; //selected object root has an UICanvas -> returns the root GameObject
			return UICanvas.GetMasterCanvas().gameObject; //selected object is NOT null, but has no UICanvas -> returns the MasterCanvas GameObject
		}

#endif

		#endregion

		#region Static Properties

		protected static DoozySettings Settings
		{
			get { return DoozySettings.Instance; }
		}

		// ReSharper disable once InconsistentNaming
		/// <summary> Database used to keep track of all the components of the given type </summary>
		public static readonly List<T> Database = new List<T>();

		/// <summary>
		///     Internal variable used to keep track if the UI interactions are disabled or not.
		///     <para>This is an additive bool so if == 0 --> false (the UI interactions are NOT disabled) and if > 0 --> true (the UI interactions are disabled).</para>
		/// </summary>
		// ReSharper disable once StaticMemberInGenericType
		private static int s_uiInteractionsDisableLevel;

		/// <summary> Returns TRUE if the UI interactions are disabled </summary>
		// ReSharper disable once MemberCanBeProtected.Global
		public static bool UIInteractionsDisabled
		{
			get
			{
				if (s_uiInteractionsDisableLevel < 0) s_uiInteractionsDisableLevel = 0;

				return s_uiInteractionsDisableLevel != 0;
			}
		}

		private static EventSystem s_unityEventSystem;

		public static EventSystem UnityEventSystem
		{
			get
			{
				if (s_unityEventSystem != null) return s_unityEventSystem;
				s_unityEventSystem = EventSystem.current;
				if (s_unityEventSystem != null) return s_unityEventSystem;
				s_unityEventSystem = FindObjectOfType<EventSystem>();
				if (s_unityEventSystem != null) return s_unityEventSystem;
				s_unityEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();
				return s_unityEventSystem;
			}
		}

		#endregion

		#region Public Variables

		/// <summary> Enables relevant debug messages to be printed to the console </summary>
		public bool DebugMode;

		/// <summary> Holds the start RectTransform.anchoredPosition3D </summary>
		public Vector3 StartPosition = UIAnimator.DEFAULT_START_POSITION;

		/// <summary> Holds the start RectTransform.localEulerAngles </summary>
		public Vector3 StartRotation = UIAnimator.DEFAULT_START_ROTATION;

		/// <summary> Holds the start RectTransform.localScale </summary>
		public Vector3 StartScale = UIAnimator.DEFAULT_START_SCALE;

		/// <summary> Holds the start alpha. It does that by checking if a CanvasGroup component is attached (holding the alpha value) or it just remembers 1 (as in 100% visibility) </summary>
		public float StartAlpha = UIAnimator.DEFAULT_START_ALPHA;

		#endregion

		#region Properties

		/// <summary> Reference to the RectTransform component </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();

				return m_rectTransform;
			}
		}

		#endregion

		#region Private Variables

		/// <summary> Internal variable that holds a reference to the RectTransform component </summary>
		private RectTransform m_rectTransform;

		#endregion

		#region Unity Methods

		protected virtual void Reset()
		{
		}

		public virtual void Awake()
		{
			BackButton.Init();
			SoundyManager.Init();
			Database.Add(GetComponent<T>());
			UpdateStartValues();
		}

		public virtual void Start()
		{
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		public virtual void OnDestroy()
		{
			Database.Remove(GetComponent<T>());
		}

		/// <summary> Returns true if the GameObject and the Component are active </summary>
		public virtual bool IsActive()
		{
			return isActiveAndEnabled;
		}

		/// <summary> Returns true if the native representation of the behaviour has been destroyed </summary>
		public bool IsDestroyed()
		{
			return this == null;
		}

		#endregion

		#region Public Methods

		/// <summary> Resets the RectTransform values to the Start values (StartPosition, StartRotation, StartScale and StartAlpha) </summary>
		public virtual void ResetToStartValues()
		{
			UIAnimator.ResetCanvasGroup(RectTransform);
			ResetPosition();
			ResetRotation();
			ResetScale();
			ResetAlpha();
		}

		/// <summary> Resets the RectTransform.anchoredPosition3D to the StartPosition value </summary>
		public virtual void ResetPosition()
		{
			RectTransform.anchoredPosition3D = StartPosition;
		}

		/// <summary> Resets the RectTransform.localEulerAngles to the StartRotation value </summary>
		public virtual void ResetRotation()
		{
			RectTransform.localEulerAngles = StartRotation;
		}

		/// <summary> Resets the RectTransform.localScale to the StartScale value </summary>
		public virtual void ResetScale()
		{
			RectTransform.localScale = new Vector3(StartScale.x, StartScale.y, 1f);
		}

		/// <summary> Resets the CanvasGroup.alpha to the StartAlpha value (if a CanvasGroup is attached) </summary>
		public virtual void ResetAlpha()
		{
			if (RectTransform.GetComponent<CanvasGroup>() != null) RectTransform.GetComponent<CanvasGroup>().alpha = StartAlpha;
		}

		/// <summary> Updates the StartPosition, StartRotation, StartScale and StartAlpha for this RectTransform to the current values </summary>
		public virtual void UpdateStartValues()
		{
			UpdateStartPosition();
			UpdateStartRotation();
			UpdateStartScale();
			UpdateStartAlpha();
		}

		/// <summary> Updates the StartPosition to the RectTransform.anchoredPosition3D value </summary>
		public virtual void UpdateStartPosition()
		{
			StartPosition = RectTransform.anchoredPosition3D;
		}

		/// <summary> Updates the StartRotation to the RectTransform.localEulerAngles value </summary>
		public virtual void UpdateStartRotation()
		{
			StartRotation = RectTransform.localEulerAngles;
		}

		/// <summary> Updates the StartScale to the RectTransform.localScale value </summary>
		public virtual void UpdateStartScale()
		{
			Vector3 localScale = RectTransform.localScale;
			StartScale = new Vector3(localScale.x, localScale.y, 1f);
		}

		/// <summary> Updates the StartAlpha to the CanvasGroup.alpha value (if a CanvasGroup is attached) </summary>
		public virtual void UpdateStartAlpha()
		{
			StartAlpha = RectTransform.GetComponent<CanvasGroup>() == null ? 1 : RectTransform.GetComponent<CanvasGroup>().alpha;
		}

		#endregion

		#region Protected Methods

		/// <summary> Removes any null references from the Database </summary>
		protected static void RemoveAnyNullReferencesFromTheDatabase()
		{
			for (int i = Database.Count; i >= 0; i--)
				if (Database[i] == null)
					Database.RemoveAt(i);
		}

		#endregion

		#region Static Methods

		/// <summary> Enables the UI interactions </summary>
		public static void EnableUIInteractions()
		{
			s_uiInteractionsDisableLevel--; //if == 0 --> false (UI interactions are NOT disabled) if > 0 --> true (UI interactions are disabled)
			if (s_uiInteractionsDisableLevel < 0) s_uiInteractionsDisableLevel = 0;
		}

		/// <summary> Enables the UI interactions by resetting the additive bool to zero. s_uiInteractionsDisableLevel = 0.
		/// Use this ONLY for special cases when something wrong happens and the UI interactions are stuck in disabled mode </summary>
		public static void EnableUIInteractionsByForce()
		{
			s_uiInteractionsDisableLevel = 0;
		}

		/// <summary> Disables the UI interactions </summary>
		public static void DisableUIInteractions()
		{
			s_uiInteractionsDisableLevel++; //if == 0 --> false (UI interactions are NOT disabled) if > 0 --> true (UI interactions are disabled)
		}

		#endregion
	}
}