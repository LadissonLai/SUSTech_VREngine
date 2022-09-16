// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
// ReSharper disable MemberCanBePrivate.Global
#endif

namespace Doozy.Engine.Themes
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents a theme and contains all the data it's comprised of like the properties and theme variants.
    /// </summary>
    [Serializable]
    public class ThemeData : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Constants

        public const string UNNAMED_THEME_NAME = "Unnamed Theme";
        public const string UNNAMED_VARIANT_NAME = "Unnamed Variant";
        public const string UNNAMED_PROPERTY = "Unnamed Property";
        public const string DEFAULT_VARIANT_NAME = "Default";

        #endregion

        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Private Variables

        /// <summary> Theme name </summary>
        [SerializeField]
        private string m_themeName = UNNAMED_THEME_NAME;
        
        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this ThemeData </summary>
        [SerializeField]
        private Guid m_id;

        /// <summary> Reference to the active Theme Variant </summary>
        [SerializeField]
        private ThemeVariantData m_activeVariant;

        #endregion

        #region Public Variables

        /// <summary> List of LabelIds for Color properties </summary>
        public List<LabelId> ColorLabels = new List<LabelId>();
        /// <summary> List of LabelIds for Sprite properties </summary>
        public List<LabelId> SpriteLabels = new List<LabelId>();
        /// <summary> List of LabelIds for Texture properties </summary>
        public List<LabelId> TextureLabels = new List<LabelId>();
        /// <summary> List of LabelIds for Font properties </summary>
        public List<LabelId> FontLabels = new List<LabelId>();
        /// <summary> List of LabelIds for FontAsset properties </summary>
        public List<LabelId> FontAssetLabels = new List<LabelId>();
        /// <summary> List of all the theme variant names found in this database </summary>
        public List<string> VariantsNames = new List<string>();
        /// <summary> List of the theme variants that this theme has  </summary>
        public List<ThemeVariantData> Variants = new List<ThemeVariantData>();

        #endregion

        #region Properties

        /// <summary> Get the currently active theme variant. If the active variant has not been set, the first variant from the Variants list will get automatically selected </summary>
        public ThemeVariantData ActiveVariant
        {
            get
            {
                if (m_activeVariant != null) return m_activeVariant;
                if (Variants.Count == 0) AddDefaultVariant();
                m_activeVariant = Variants[0];
                SetDirty(false);
                return m_activeVariant;
            }
        }
        
        /// <summary> Id of this theme </summary>
        public Guid Id { get { return m_id; } }
        
        /// <summary> Name of this theme </summary>
        public string ThemeName { get { return m_themeName; } set { m_themeName = value; } }
        
        /// <summary> Returns TRUE if this theme's name is 'General'. This is a special theme name as it considered the default theme (it will always exist) </summary>
        public bool IsGeneralTheme { get { return ThemeName.Equals(ThemesDatabase.GENERAL_THEME_NAME); } }

        #endregion

        #region Constuctors

        /// <summary> Construct a new theme with an unique Id </summary>
        public ThemeData() { m_id = Guid.NewGuid(); }

        #endregion

        #region Unity Methods

        public void OnBeforeSerialize() { SerializedGuid = GuidUtils.GuidToSerializedGuid(m_id); }

        public void OnAfterDeserialize() { m_id = GuidUtils.SerializedGuidToGuid(SerializedGuid); }

        #endregion

        #region Public Methods

        /// <summary> Activate the given variant </summary>
        /// <param name="variant"> Variant to activate </param>
        public void ActivateVariant(ThemeVariantData variant)
        {
            if (variant == null || !Variants.Contains(variant)) return;
            m_activeVariant = variant;
            SetDirty(false);
        }

        /// <summary> Activate the given variant by Guid </summary>
        /// <param name="variantId"> Variant Id of the variant to activate </param>
        public void ActivateVariant(Guid variantId)
        {
            if (variantId == Guid.Empty || !ContainsVariant(variantId)) return;
            m_activeVariant = GetVariant(variantId);
            SetDirty(false);
        }

        /// <summary> Activate the given variant by its name </summary>
        /// <param name="variantName"> Variant name of the variant to activate </param>
        public void ActivateVariant(string variantName)
        {
            if (string.IsNullOrEmpty(variantName)) return;
            m_activeVariant = GetVariant(variantName);
            SetDirty(false);
        }

        /// <summary> Add a new color property </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddColorProperty(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.AddItem);
            ColorLabels.Add(new LabelId(UNNAMED_PROPERTY));
            Guid guid = ColorLabels[ColorLabels.Count - 1].Id;
            foreach (ThemeVariantData variant in Variants)
                variant.AddColorProperty(guid);
            SetDirty(saveAssets);
        }

        /// <summary> Add a new sprite property </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddSpriteProperty(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.AddItem);
            SpriteLabels.Add(new LabelId(UNNAMED_PROPERTY));
            Guid guid = SpriteLabels[SpriteLabels.Count - 1].Id;
            foreach (ThemeVariantData variant in Variants)
                variant.AddSpriteProperty(guid);
            SetDirty(saveAssets);
        }

        /// <summary> Add a new texture property </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddTextureProperty(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.AddItem);
            TextureLabels.Add(new LabelId(UNNAMED_PROPERTY));
            Guid guid = TextureLabels[TextureLabels.Count - 1].Id;
            foreach (ThemeVariantData variant in Variants)
                variant.AddTextureProperty(guid);
            SetDirty(saveAssets);
        }

        /// <summary> Add a new font property </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddFontProperty(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.AddItem);
            FontLabels.Add(new LabelId(UNNAMED_PROPERTY));
            Guid guid = FontLabels[FontLabels.Count - 1].Id;
            foreach (ThemeVariantData variant in Variants)
                variant.AddFontProperty(guid);
            SetDirty(saveAssets);
        }

        /// <summary> Add a new font asset property (TextMeshPro font) </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddFontAssetProperty(bool performUndo, bool saveAssets = false)
        {
#if dUI_TextMeshPro
            if (performUndo) UndoRecord(UILabels.AddItem);
            FontAssetLabels.Add(new LabelId(UNNAMED_PROPERTY));
            Guid guid = FontAssetLabels[FontAssetLabels.Count - 1].Id;
            foreach (ThemeVariantData variant in Variants)
                variant.AddFontAssetProperty(guid);
            SetDirty(saveAssets);
#endif
        }

        /// <summary> Add a new variant to the theme </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddVariant(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.NewThemeVariant);
            Variants.Add(new ThemeVariantData(UNNAMED_VARIANT_NAME,
                                              new List<LabelId>(ColorLabels),
                                              new List<LabelId>(SpriteLabels),
                                              new List<LabelId>(TextureLabels),
                                              new List<LabelId>(FontLabels),
                                              new List<LabelId>(FontAssetLabels)));
            SetDirty(saveAssets);
        }

        /// <summary> Returns TRUE if a Color property, with the given Id, exists in this theme </summary>
        /// <param name="propertyId"> Guid to search for </param>
        public bool ContainsColorProperty(Guid propertyId) { return ColorLabels != null && ColorLabels.Any(colorLabel => colorLabel.Id.Equals(propertyId)); }
        
        /// <summary> Returns TRUE if a Sprite property, with the given Id, exists in this theme </summary>
        /// <param name="propertyId"> Guid to search for </param>
        public bool ContainsSpriteProperty(Guid propertyId) { return SpriteLabels != null && SpriteLabels.Any(spriteLabel => spriteLabel.Id.Equals(propertyId)); }
        
        /// <summary> Returns TRUE if a Texture property, with the given Id, exists in this theme </summary>
        /// <param name="propertyId"> Guid to search for </param>
        public bool ContainsTextureProperty(Guid propertyId) { return TextureLabels != null && TextureLabels.Any(textureLabel => textureLabel.Id.Equals(propertyId)); }
        
        /// <summary> Returns TRUE if a Font property, with the given Id, exists in this theme </summary>
        /// <param name="propertyId"> Guid to search for </param>
        public bool ContainsFontProperty(Guid propertyId) { return FontLabels != null && FontLabels.Any(fontLabel => fontLabel.Id.Equals(propertyId)); }
        
        /// <summary> Returns TRUE if a FontAsset property, with the given Id, exists in this theme </summary>
        /// <param name="propertyId"> Guid to search for </param>
        public bool ContainsFontAssetProperty(Guid propertyId) { return FontAssetLabels != null && FontAssetLabels.Any(fontAssetLabel => fontAssetLabel.Id.Equals(propertyId)); }

        /// <summary> Returns TRUE if the variant Guid has been found in the database </summary>
        /// <param name="variantGuid"> Target sound name to search for </param>
        public bool ContainsVariant(Guid variantGuid) { return Variants != null && Variants.Any(variant => variant.Id.Equals(variantGuid)); }

        /// <summary> Returns TRUE if the variant name has been found in the database </summary>
        /// <param name="variantName"> Target variant name to search for </param>
        public bool ContainsVariant(string variantName) { return Variants != null && Variants.Any(variant => variant.VariantName.Equals(variantName)); }

        /// <summary> Get the variant with the given Guid </summary>
        /// <param name="variantId"> Variant Id </param>
        public ThemeVariantData GetVariant(Guid variantId) { return Variants.FirstOrDefault(variant => variant.Id.Equals(variantId)); }

        /// <summary> Get the variant with the given name </summary>
        /// <param name="variantName"> Variant name </param>
        public ThemeVariantData GetVariant(string variantName) { return Variants.FirstOrDefault(variant => variant.VariantName.Equals(variantName)); }

        /// <summary> Get the color property index by searching for the given Guid </summary>
        /// <param name="id"> Guid value to search for </param>
        public int GetColorPropertyIndex(Guid id) { return GetPropertyIndex(id, ColorLabels); }
        
        /// <summary> Get the sprite property index by searching for the given Guid </summary>
        /// <param name="id"> Guid value to search for </param>
        public int GetSpritePropertyIndex(Guid id) { return GetPropertyIndex(id, SpriteLabels); }
        
        /// <summary> Get the texture property index by searching for the given Guid </summary>
        /// <param name="id"> Guid value to search for </param>
        public int GetTexturePropertyIndex(Guid id) { return GetPropertyIndex(id, TextureLabels); }
        
        /// <summary> Get the font property index by searching for the given Guid </summary>
        /// <param name="id"> Guid value to search for </param>
        public int GetFontPropertyIndex(Guid id) { return GetPropertyIndex(id, FontLabels); }
        
        /// <summary> Get the font asset property index by searching for the given Guid </summary>
        /// <param name="id"> Guid value to search for </param>
        public int GetFontAssetPropertyIndex(Guid id) { return GetPropertyIndex(id, FontAssetLabels); }

        /// <summary> Get the variant index by searching for the given Guid </summary>
        /// <param name="id"> Guid value to search for </param>
        public int GetVariantIndex(Guid id)
        {
            if (id == Guid.Empty) return -1;
            for (int i = 0; i < Variants.Count; i++)
            {
                if (!Variants[i].Id.Equals(id)) continue;
                return i;
            }

            return -1;
        }

        /// <summary> Initializes this theme by checking if it has the 'Default' variant. If it does not, it adds it </summary>
        /// <param name="showProgress"> Should a progress window be shown while executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void Init(bool showProgress, bool saveAssets)
        {
            RefreshThemeVariants(showProgress, false, saveAssets);
            if (ActiveVariant == null || !ContainsVariant(ActiveVariant.Id))
            {
                m_activeVariant = Variants[0];
            }
        }

        /// <summary> Remove a color property </summary>
        /// <param name="deleteGuid"> Guid to search for </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveColorProperty(Guid deleteGuid, bool performUndo, bool saveAssets)
        {
            if (performUndo) UndoRecord(UILabels.RemoveItem);
            RemoveProperty(deleteGuid, ColorLabels);
            
            foreach (ThemeVariantData variant in Variants)
            {
                foreach (ColorId c in variant.Colors)
                {
                    if (!c.Id.Equals(deleteGuid)) continue;
                    variant.Colors.Remove(c);
                    break;
                }
            }

            SetDirty(saveAssets);
        }
        
        /// <summary> Remove a sprite property </summary>
        /// <param name="deleteGuid"> Guid to search for </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveSpriteProperty(Guid deleteGuid, bool performUndo, bool saveAssets)
        {
            if (performUndo) UndoRecord(UILabels.RemoveItem);
            RemoveProperty(deleteGuid, SpriteLabels);
            
            foreach (ThemeVariantData variant in Variants)
            {
                foreach (SpriteId s in variant.Sprites)
                {
                    if (!s.Id.Equals(deleteGuid)) continue;
                    variant.Sprites.Remove(s);
                    break;
                }
            }

            SetDirty(saveAssets);
        }
        
        /// <summary> Remove a texture property </summary>
        /// <param name="deleteGuid"> Guid to search for </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveTextureProperty(Guid deleteGuid, bool performUndo, bool saveAssets)
        {
            if (performUndo) UndoRecord(UILabels.RemoveItem);
            RemoveProperty(deleteGuid, TextureLabels);
            
            foreach (ThemeVariantData variant in Variants)
            {
                foreach (TextureId t in variant.Textures)
                {
                    if (!t.Id.Equals(deleteGuid)) continue;
                    variant.Textures.Remove(t);
                    break;
                }
            }

            SetDirty(saveAssets);
        }

        /// <summary> Remove a font property </summary>
        /// <param name="deleteGuid"> Guid to search for </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveFontProperty(Guid deleteGuid, bool performUndo, bool saveAssets)
        {
            if (performUndo) UndoRecord(UILabels.RemoveItem);
            RemoveProperty(deleteGuid, FontLabels);

            foreach (ThemeVariantData variant in Variants)
            {
                foreach (FontId f in variant.Fonts)
                {
                    if (!f.Id.Equals(deleteGuid)) continue;
                    variant.Fonts.Remove(f);
                    break;
                }
            }

            SetDirty(saveAssets);
        }

        /// <summary> Remove a font asset property </summary>
        /// <param name="deleteGuid"> Guid to search for </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveFontAssetProperty(Guid deleteGuid, bool performUndo, bool saveAssets)
        {
            if (performUndo) UndoRecord(UILabels.RemoveItem);
            RemoveProperty(deleteGuid, FontAssetLabels);

            foreach (ThemeVariantData variant in Variants)
            {
                foreach (FontAssetId fa in variant.FontAssets)
                {
                    if (!fa.Id.Equals(deleteGuid)) continue;
                    variant.FontAssets.Remove(fa);
                    break;
                }
            }

            SetDirty(saveAssets);
        }


        /// <summary> Refresh the entire database by removing empty, duplicate and unnamed entries, sorting the database and updating the names list </summary>
        /// <param name="showProgress"> Should a progress window be shown while executing the action </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RefreshThemeVariants(bool showProgress, bool performUndo, bool saveAssets)
        {
            if (showProgress) DoozyUtils.DisplayProgressBar(UILabels.ThemeName + ": " + ThemeName, UILabels.RefreshDatabase, 0.1f);
            if (performUndo) UndoRecord(UILabels.RefreshDatabase);
            if (showProgress) DoozyUtils.DisplayProgressBar(UILabels.ThemeName + ": " + ThemeName, UILabels.RefreshDatabase, 0.2f);
            bool addedDefaultColorLabels = ColorLabels.Count == 0; //if the color properties list is empty -> add the default color labels
            if(addedDefaultColorLabels) AddDefaultColorLabels();
            if (showProgress) DoozyUtils.DisplayProgressBar(UILabels.ThemeName + ": " + ThemeName, UILabels.RefreshDatabase, 0.3f);
            bool addedDefaultVariant = AddDefaultVariant();
            if (showProgress) DoozyUtils.DisplayProgressBar(UILabels.ThemeName + ": " + ThemeName, UILabels.RefreshDatabase, 0.6f);
            UpdateVariantsNames(false);
            if (showProgress) DoozyUtils.DisplayProgressBar(UILabels.ThemeName + ": " + ThemeName, UILabels.RefreshDatabase, 0.7f);
            if (addedDefaultColorLabels || addedDefaultVariant) SetDirty(saveAssets);
            if (showProgress) DoozyUtils.DisplayProgressBar(UILabels.ThemeName + ": " + ThemeName, UILabels.RefreshDatabase, 1f);
            if (showProgress) DoozyUtils.ClearProgressBar();
        }

        /// <summary> Iterate through the database to look for the given variant. If found, removes the entry and returns TRUE </summary>
        /// <param name="data"> ThemeVariantData to search for </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool RemoveVariant(ThemeVariantData data, bool performUndo = false, bool showDialog = false, bool saveAssets = false)
        {
            if (data == null) return false;
            if (!Variants.Contains(data)) return false;
#if UNITY_EDITOR
            if (showDialog)
                if (!EditorUtility.DisplayDialog(UILabels.RemovedEntry + " '" + data.VariantName + "'",
                                                 UILabels.AreYouSureYouWantToRemoveTheEntry,
                                                 UILabels.Yes,
                                                 UILabels.No))
                    return false;
#endif

            for (int i = Variants.Count - 1; i >= 0; i--)
                if (Variants[i] == data)
                {
                    if (performUndo) UndoRecord(UILabels.RemovedEntry);
                    Variants.RemoveAt(i);
                    break;
                }

            UpdateVariantsNames(false);
            SetDirty(saveAssets);

            return true;
        }

        /// <summary> [Editor Only] Mark target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Sort the variants </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void Sort(bool performUndo, bool saveAssets)
        {
            Variants = Variants.OrderBy(variant => variant.VariantName).ToList(); //sort by variant name
            UpdateVariantsNames(saveAssets);
        }

        /// <summary> Record any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        /// <summary> Update the list of theme variants names found in the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void UpdateVariantsNames(bool saveAssets)
        {
#if UNITY_EDITOR
            AddDefaultVariant();
#endif
            VariantsNames.Clear();
            List<string> list = Variants.Select(variant => variant.VariantName).ToList();
            list.Sort();
            VariantsNames.AddRange(list);
            SetDirty(saveAssets);
        }

        #endregion

        #region Private Methods

        private const string COLOR_DEFAULT_COLOR_LABEL_1 = "Primary";
        private const string COLOR_DEFAULT_COLOR_LABEL_2 = "Secondary";
        private const string COLOR_DEFAULT_COLOR_LABEL_3 = "Accent 1";
        private const string COLOR_DEFAULT_COLOR_LABEL_4 = "Accent 2";
        private const string COLOR_DEFAULT_COLOR_LABEL_5 = "Text";
        private const string COLOR_DEFAULT_COLOR_LABEL_6 = "Disabled";

        /// <summary> Add the default color properties </summary>
        private void AddDefaultColorLabels()
        {
            ColorLabels.Add(new LabelId(COLOR_DEFAULT_COLOR_LABEL_1));
            ColorLabels.Add(new LabelId(COLOR_DEFAULT_COLOR_LABEL_2));
            ColorLabels.Add(new LabelId(COLOR_DEFAULT_COLOR_LABEL_3));
            ColorLabels.Add(new LabelId(COLOR_DEFAULT_COLOR_LABEL_4));
            ColorLabels.Add(new LabelId(COLOR_DEFAULT_COLOR_LABEL_5));
            ColorLabels.Add(new LabelId(COLOR_DEFAULT_COLOR_LABEL_6));
        }

        /// <summary> Add the 'Default' variant to the database (if it does not exist) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        private bool AddDefaultVariant(bool saveAssets = false)
        {
            if (Variants.Count > 0 || ContainsVariant(DEFAULT_VARIANT_NAME)) return false;
            VariantsNames.Add(DEFAULT_VARIANT_NAME);
            var variant = new ThemeVariantData(DEFAULT_VARIANT_NAME,
                                               ColorLabels,
                                               SpriteLabels,
                                               TextureLabels,
                                               FontLabels,
                                               FontAssetLabels);
            Variants.Add(variant);
            SetDirty(saveAssets);
            return true;
        }

       

        #endregion

        #region Static Methods

        /// <summary> Get the index of the property with the given id from the given property list </summary>
        /// <param name="id"> Guid of the property to search for </param>
        /// <param name="propertyList"> List of LabelIds to search for the given property id </param>
        private static int GetPropertyIndex(Guid id, List<LabelId> propertyList)
        {
            if (id == Guid.Empty) return -1;
            if (propertyList == null) return -1;
            for (int i = 0; i < propertyList.Count; i++)
            {
                if (!propertyList[i].Id.Equals(id)) continue;
                return i;
            }

            return -1;
        }

        /// <summary> Remove the property with the given id from the given property list </summary>
        /// <param name="deleteGuid"> Guid of the property that needs to be deleted from the given property list </param>
        /// <param name="propertyList"> Property list where the property can be found </param>
        private static void RemoveProperty(Guid deleteGuid, List<LabelId> propertyList)
        {
            for (int i = propertyList.Count - 1; i >= 0; i--)
            {
                LabelId property = propertyList[i];
                if (!property.Id.Equals(deleteGuid)) continue;
                propertyList.RemoveAt(i);
                break;
            }
        }

        #endregion
    }
}