using UnityEditor;
using UnityEngine;

public sealed class ExplodePartEditor : EditorWindow
{
	private enum TransformType
	{
		Final,
		Initial,
	}

	private static SelectionMode SelectionModeMask = SelectionMode.Deep | SelectionMode.Editable;
	private static Vector2 WindowMinSize = new Vector2(320f, 82f);
	private static string WindowTitle = "Explode Part Editor";

	private float m_explodeOffset;

	#region Unity core events.
	[MenuItem("Window/Beffio/Explode Part Editor")]
	private static void ShowWindow()
	{
		ExplodePartEditor window = EditorWindow.GetWindow<ExplodePartEditor>(false, WindowTitle);
		window.minSize = WindowMinSize;
	}

	private void Awake()
	{
		m_explodeOffset = 0f;
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Set initial transform"))
		{
			ProcessSelection(TransformType.Initial);
		}

		if (GUILayout.Button("Set final transform"))
		{
			ProcessSelection(TransformType.Final);
		}

		GUILayout.Space(16f);

		float currentExplodeOffset = EditorGUILayout.Slider("Explode preview", m_explodeOffset, 0f, 1f);
		if (!Mathf.Approximately(currentExplodeOffset, m_explodeOffset))
		{
			ApplyExplodeOffset(currentExplodeOffset);
		}

		m_explodeOffset = currentExplodeOffset;
	}
	#endregion //Unity core events.

	#region Class functions.
	private void ApplyExplodeOffset(float offset)
	{
		Object[] selection = Selection.GetFiltered(typeof(ExplodePart), SelectionModeMask);
		if (selection.Length < 1)
		{
			return;
		}

		string undoLabel = string.Format("Apply explode offset ({0} selected)", selection.Length);
		foreach (ExplodePart item in selection)
		{
			Transform cachedTransform = item.transform;

			Undo.RecordObject(cachedTransform, undoLabel);
			item.ApplyExplodeOffset(offset);
			EditorUtility.SetDirty(cachedTransform);
		}
	}

	private void ProcessSelection(TransformType transformType)
	{
		Object[] selection = Selection.GetFiltered(typeof(ExplodePart), SelectionModeMask);
		if (selection.Length < 1)
		{
			return;
		}

		Undo.RecordObjects(selection, string.Format("Set explode {0} transform ({1} selected)", transformType, selection.Length));

		foreach (ExplodePart item in selection)
		{
			Transform cachedTransform = item.transform;
			ExplodePart.TransformParams explodeParams = transformType == TransformType.Final ?
				item.FinalTransform : item.InitialTransform;

			explodeParams.Position = cachedTransform.localPosition;
			explodeParams.Rotation = cachedTransform.localRotation;
			explodeParams.Scale = cachedTransform.localScale;

			EditorUtility.SetDirty(item);
		}
	}
	#endregion //Class functions.
}
