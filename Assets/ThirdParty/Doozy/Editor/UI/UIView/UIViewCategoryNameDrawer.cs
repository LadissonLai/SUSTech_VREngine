// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Internal;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.UI
{
    [CustomPropertyDrawer(typeof(UIViewCategoryName))]
    public class UIViewCategoryNameDrawer : BaseDrawer
    {
        protected override ColorName DrawerColorName { get { return DGUI.Colors.ActionColorName; } }

        private readonly Dictionary<string, bool> m_initialized = new Dictionary<string, bool>();

        private static NamesDatabase Database { get { return UIViewSettings.Database; } }

        private void Init(SerializedProperty property)
        {
            if (m_initialized.ContainsKey(property.propertyPath) && m_initialized[property.propertyPath]) return;

            Elements.Add(Properties.Add(PropertyName.Category, property), Contents.Add(UILabels.Category));
            Elements.Add(Properties.Add(PropertyName.Name, property), Contents.Add(UILabels.Name));
            Elements.Add(Properties.Add(PropertyName.InstantAction, property), Contents.Add(UILabels.InstantAction));

            if (!m_initialized.ContainsKey(property.propertyPath))
                m_initialized.Add(property.propertyPath, true);
            else
                m_initialized[property.propertyPath] = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            Init(property);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            {
                // don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                Draw(position, property);

                // set indent back to what it was
                EditorGUI.indentLevel = indent;

                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        private void Draw(Rect position, SerializedProperty property)
        {
            SerializedProperty category = Properties.Get(PropertyName.Category, property);
            SerializedProperty name = Properties.Get(PropertyName.Name, property);
            SerializedProperty instantAction = Properties.Get(PropertyName.InstantAction, property);

            var items = new List<string>();

            Color initialColor = GUI.color; //save the GUI color
            NumberOfLines[property.propertyPath] = 1;
            Rect drawRect = GetDrawRect(position); //calculate draw rect

            GUI.color = DGUI.Colors.PropertyColor(DrawerColorName);

            float x = drawRect.x + DGUI.Properties.Space(2);
            float width = drawRect.width
                          - DGUI.Toggle.Switch.Width
                          - Contents.GetWidth(UILabels.InstantAction, DGUI.Label.Style(Size.S))
                          - DGUI.Properties.Space(9);


            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;

            //VIEW CATEGORY
            int categorySelectedIndex = Database.CategoryNames.Contains(category.stringValue)
                ? Database.CategoryNames.IndexOf(category.stringValue)
                : Database.CategoryNames.IndexOf(NamesDatabase.CUSTOM);

            EditorGUI.BeginChangeCheck();
            categorySelectedIndex = EditorGUI.Popup(new Rect(x, drawRect.y + 1, width * 0.5f, drawRect.height), categorySelectedIndex, Database.CategoryNames.ToArray());
            x += width * 0.5f + DGUI.Properties.Space();

            if (EditorGUI.EndChangeCheck())
            {
                category.stringValue = Database.CategoryNames[categorySelectedIndex];
                if (Database.CategoryNames[categorySelectedIndex] != NamesDatabase.CUSTOM)
                {
                    if (string.IsNullOrEmpty(name.stringValue.Trim()))
                    {
                        name.stringValue = Database.GetNamesList(Database.CategoryNames[categorySelectedIndex])[0];
                    }
                    else if (name.stringValue.Trim() != NamesDatabase.UNNAMED &&
                             !Database.GetNamesList(Database.CategoryNames[categorySelectedIndex]).Contains(name.stringValue.Trim()))
                    {
                        if (EditorUtility.DisplayDialog("Add Name", "Add the '" + name.stringValue.Trim() + "' name to the '" + Database.CategoryNames[categorySelectedIndex] + "' category?", "Yes", "No"))
                        {
                            string cleanName = name.stringValue.Trim();
                            ListOfNames categoryAsset = Database.GetCategory(Database.CategoryNames[categorySelectedIndex]);
                            categoryAsset.Names.Add(cleanName);
                            categoryAsset.SetDirty(false);
                            Database.Sort(false, true);
//                            Database.GetNamesList(Database.CategoryNames[categorySelectedIndex], true).Add(name.stringValue.Trim());
//                            Database.SetDirty(true);
                        }
                        else if (Database.CategoryNames[categorySelectedIndex] == NamesDatabase.GENERAL)
                        {
                            name.stringValue = NamesDatabase.UNNAMED;
                        }
                        else
                        {
                            name.stringValue = Database.GetNamesList(Database.CategoryNames[categorySelectedIndex])[0];
                        }
                    }
                }
            }

            bool hasCustomName = category.stringValue.Equals(NamesDatabase.CUSTOM);

            if (!Database.Contains(category.stringValue)) //database does not contain this category -> reset it to custom
            {
                hasCustomName = true;
                category.stringValue = NamesDatabase.CUSTOM;
            }

            //VIEW NAME
            if (!hasCustomName)
            {
                items = Database.GetNamesList(category.stringValue);
                if (items.Count == 0)
                {
                    if (!Database.GetNamesList(NamesDatabase.GENERAL, true).Contains(NamesDatabase.UNNAMED))
                    {
                        Database.GetNamesList(NamesDatabase.GENERAL, true).Add(NamesDatabase.UNNAMED);
                        Database.SetDirty(true);
                    }

                    category.stringValue = NamesDatabase.GENERAL;
                    items = Database.GetNamesList(category.stringValue);
                }
            }

            if (hasCustomName)
            {
                EditorGUI.PropertyField(new Rect(x, drawRect.y + 1, width * 0.5f, drawRect.height), name, GUIContent.none, true);
            }
            else
            {
                int nameSelectedIndex = 0;
                if (items.Contains(name.stringValue))
                {
                    nameSelectedIndex = items.IndexOf(name.stringValue);
                }
                else
                {
                    if (category.stringValue.Equals(NamesDatabase.GENERAL))
                    {
                        name.stringValue = NamesDatabase.UNNAMED;
                        nameSelectedIndex = items.IndexOf(NamesDatabase.UNNAMED);
                    }
                    else if (name.stringValue != NamesDatabase.UNNAMED &&
                             EditorUtility.DisplayDialog("Add Name", "Add the '" + name.stringValue + "' name to the '" + category.stringValue + "' category?", "Yes", "No"))
                    {
                        string cleanName = name.stringValue.Trim();
                        if (string.IsNullOrEmpty(cleanName))
                        {
                            name.stringValue = items[nameSelectedIndex];
                        }
                        else if (items.Contains(cleanName))
                        {
                            name.stringValue = cleanName;
                            nameSelectedIndex = items.IndexOf(cleanName);
                        }
                        else
                        {
                            ListOfNames categoryAsset = Database.GetCategory(category.stringValue);
                            categoryAsset.Names.Add(cleanName);
                            categoryAsset.SetDirty(false);
                            Database.Sort(false, true);
//                            Database.GetNamesList(category.stringValue, true).Add(cleanName);
//                            Database.SetDirty(true);
                            name.stringValue = cleanName;
                            nameSelectedIndex = items.IndexOf(name.stringValue);
                        }
                    }
                    else
                    {
                        name.stringValue = items[nameSelectedIndex];
                    }
                }

                EditorGUI.BeginChangeCheck();
                nameSelectedIndex = EditorGUI.Popup(new Rect(x, drawRect.y + 1, width * 0.5f, drawRect.height), nameSelectedIndex, items.ToArray());
                if (EditorGUI.EndChangeCheck()) name.stringValue = items[nameSelectedIndex];
            }

            x += width * 0.5f + DGUI.Properties.Space();

            GUI.color = initialColor; //restore the GUI color
            DGUI.Toggle.Switch.Draw(new Rect(x, drawRect.y + 1, DGUI.Toggle.Switch.Width, drawRect.height), instantAction, DrawerColorName);
            x += DGUI.Toggle.Switch.Width + DGUI.Properties.Space();

            GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(instantAction.boolValue));
            DGUI.Label.Draw(new Rect(x, drawRect.y, Contents.GetWidth(UILabels.InstantAction), drawRect.height), UILabels.InstantAction, Size.S, instantAction.boolValue ? DrawerColorName : DGUI.Colors.DisabledTextColorName);
            GUI.color = initialColor; //restore the GUI color
        }
    }
}