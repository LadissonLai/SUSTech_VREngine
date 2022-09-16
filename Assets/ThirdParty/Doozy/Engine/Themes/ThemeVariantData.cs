// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Utils;
using UnityEngine;

#if dUI_TextMeshPro
using TMPro;
#endif

// ReSharper disable InconsistentNaming

namespace Doozy.Engine.Themes
{
    /// <summary> Holds all the data of a theme variant </summary>
    [Serializable]
    public class ThemeVariantData : ISerializationCallbackReceiver
    {
        #region Constants

        public const string DEFAULT_THEME_VARIANT_NAME = "Unnamed Variant";

        public static Color DefaultColor { get { return Color.magenta; } }
        public static Sprite DefaultSprite { get { return null; } }
        public static Texture DefaultTexture { get { return null; } }
        public static Font DefaultFont { get { return Resources.GetBuiltinResource<Font>("Arial.ttf"); } }
#if dUI_TextMeshPro
        public static TMP_FontAsset DefaultFontAsset { get { return null; } }
#endif

        #endregion

        #region Private Variables

        /// <summary> Theme Variant Name </summary>
        [SerializeField]
        private string m_variantName = DEFAULT_THEME_VARIANT_NAME;

        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this Theme Variant </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Public Variables

        /// <summary> List of ColorIds that contains all the Color values </summary>
        public List<ColorId> Colors = new List<ColorId>();

        /// <summary> List of SpriteId that contains all the Sprite references </summary>
        public List<SpriteId> Sprites = new List<SpriteId>();

        /// <summary> List of TextureId that contains all the Texture references </summary>
        public List<TextureId> Textures = new List<TextureId>();

        /// <summary> List of FontId that contains all the Font references </summary>
        public List<FontId> Fonts = new List<FontId>();

        /// <summary> List of FontAssetId that contains all the TextMeshPro FontAsset references </summary>
        public List<FontAssetId> FontAssets = new List<FontAssetId>();

        #endregion

        #region Properties

        /// <summary> Id of this Theme Variant </summary>
        public Guid Id { get { return m_id; } }

        /// <summary> Name of this Theme Variant </summary>
        public string VariantName { get { return m_variantName; } set { m_variantName = value; } }

        #endregion

        #region Constructors

        /// <summary> Construct a new ThemeVariantData with a unique Id </summary>
        public ThemeVariantData() { m_id = Guid.NewGuid(); }

        /// <summary> Construct a new ThemeVariantData with a given name and a unique Id </summary>
        /// <param name="variantName"> Variant Name </param>
        public ThemeVariantData(string variantName)
        {
            m_id = Guid.NewGuid();
            m_variantName = variantName;
        }

        /// <summary> Construct a new ThemeVariantData with the given settings and a unique Id </summary>
        /// <param name="variantName"> Variant Name </param>
        /// <param name="colorLabels"> LabelId property list for Colors </param>
        /// <param name="spriteLabels"> LabelId property list for Sprites </param>
        /// <param name="textureLabels"> LabelId property list for Textures </param>
        /// <param name="fontLabels"> LabelId property list for Fonts </param>
        /// <param name="fontAssetLabels"> LabelId property list for FontAssets (TextMeshPro) </param>
        public ThemeVariantData(string variantName,
                                List<LabelId> colorLabels,
                                List<LabelId> spriteLabels,
                                List<LabelId> textureLabels,
                                List<LabelId> fontLabels,
                                List<LabelId> fontAssetLabels)
        {
            m_id = Guid.NewGuid();
            m_variantName = variantName;
            SyncColorIdsToLabelIds(colorLabels);
            SyncSpriteIdsToLabelIds(spriteLabels);
            SyncTextureIdsToLabelIds(textureLabels);
            SyncFontIdsToLabelIds(fontLabels);
            SyncFontAssetIdsToLabelIds(fontAssetLabels);
        }

        #endregion

        #region Unity Methods

        public void OnBeforeSerialize() { SerializedGuid = GuidUtils.GuidToSerializedGuid(m_id); }

        public void OnAfterDeserialize() { m_id = GuidUtils.SerializedGuidToGuid(SerializedGuid); }

        #endregion

        #region Public Methods

        #region COLOR

        /// <summary> Add a new Color property with the given Id </summary>
        /// <param name="guid"> Property id </param>
        public void AddColorProperty(Guid guid) { Colors.Add(new ColorId(guid, Color.white)); }

        /// <summary> Add a new Color property with a given value and Id </summary>
        /// <param name="guid"> Property id </param>
        /// <param name="color"> Color value </param>
        public void AddColorProperty(Guid guid, Color color) { Colors.Add(new ColorId(guid, color)); }
        
        /// <summary> Returns true if this theme contains a Color property with the given Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public bool ContainsColor(Guid propertyId) { return Colors.Any(p => p.Id.Equals(propertyId)); }

        /// <summary> Get the Color with the given property Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public Color GetColor(Guid propertyId)
        {
            foreach (ColorId p in Colors.Where(p => p.Id.Equals(propertyId)))
                return p.Color;
            return DefaultColor;
        }

        #endregion

        #region SPRITE

        /// <summary> Add a new Sprite property with the given Id </summary>
        /// <param name="guid"> Property id </param>
        public void AddSpriteProperty(Guid guid) { Sprites.Add(new SpriteId(guid, null)); }
        
        /// <summary> Add a new Sprite property with a given value and Id </summary>
        /// <param name="guid"> Property id </param>
        /// <param name="sprite"> Sprite reference </param>
        public void AddSpriteProperty(Guid guid, Sprite sprite) { Sprites.Add(new SpriteId(guid, sprite)); }
        
        /// <summary> Returns true if this theme contains a Sprite property with the given Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public bool ContainsSprite(Guid propertyId) { return Sprites.Any(p => p.Id.Equals(propertyId)); }

        /// <summary> Get the Sprite reference with the given property Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public Sprite GetSprite(Guid propertyId)
        {
            foreach (SpriteId p in Sprites.Where(p => p.Id.Equals(propertyId)))
                return p.Sprite;
            return DefaultSprite;
        }

        #endregion

        #region TEXTURE

        /// <summary> Add a new Texture property with the given Id </summary>
        /// <param name="guid"> Property id </param>
        public void AddTextureProperty(Guid guid) { Textures.Add(new TextureId(guid, null)); }
        
        /// <summary> Add a new Texture property with a given value and Id </summary>
        /// <param name="guid"> Property id </param>
        /// <param name="texture"> Texture reference </param>
        public void AddTextureProperty(Guid guid, Texture texture) { Textures.Add(new TextureId(guid, texture)); }
        
        /// <summary> Returns true if this theme contains a Texture property with the given Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public bool ContainsTexture(Guid propertyId) { return Textures.Any(p => p.Id.Equals(propertyId)); }

        /// <summary> Get the Texture reference with the given property Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public Texture GetTexture(Guid propertyId)
        {
            foreach (TextureId p in Textures.Where(p => p.Id.Equals(propertyId)))
                return p.Texture;
            return DefaultTexture;
        }

        #endregion

        #region FONT

        /// <summary> Add a new Font property with the given Id </summary>
        /// <param name="guid"> Property id </param>
        public void AddFontProperty(Guid guid) { Fonts.Add(new FontId(guid, DefaultFont)); }
        
        /// <summary> Add a new Font property with a given value and Id </summary>
        /// <param name="guid"> Property id </param>
        /// <param name="font"> Font reference </param>
        public void AddFontProperty(Guid guid, Font font) { Fonts.Add(new FontId(guid, font)); }
        
        /// <summary> Returns true if this theme contains a Font property with the given Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public bool ContainsFont(Guid propertyId) { return Fonts.Any(p => p.Id.Equals(propertyId)); }

        /// <summary> Get the Font reference with the given property Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public Font GetFont(Guid propertyId)
        {
            foreach (FontId fontProperty in Fonts.Where(fontProperty => fontProperty.Id.Equals(propertyId)))
                return fontProperty.Font;
            return DefaultFont;
        }

        #endregion

        #region FONT ASSET (TextMeshPro)

#if dUI_TextMeshPro

        /// <summary> Add a new FontAsset property with the given Id </summary>
        /// <param name="guid"> Property id </param>
        public void AddFontAssetProperty(Guid guid) { FontAssets.Add(new FontAssetId(guid, null)); }
        
        /// <summary> Add a new FontAsset property with a given value and Id </summary>
        /// <param name="guid"> Property id </param>
        /// <param name="fontAsset"> FontAsset reference </param>
        public void AddFontAssetProperty(Guid guid, TMP_FontAsset fontAsset) { FontAssets.Add(new FontAssetId(guid, fontAsset)); }
        
        /// <summary> Returns true if this theme contains a FontAsset property with the given Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public bool ContainsFontAsset(Guid propertyId) { return FontAssets.Any(p => p.Id.Equals(propertyId)); }
        
        /// <summary> Get the FontAsset reference with the given property Id </summary>
        /// <param name="propertyId"> Property Id </param>
        public TMP_FontAsset GetFontAsset(Guid propertyId)
        {
            foreach (FontAssetId fontAssetProperty in FontAssets)
                if (fontAssetProperty.Id.Equals(propertyId))
                    return fontAssetProperty.FontAsset;
            return DefaultFontAsset;
        }

#endif

        #endregion

        #endregion

        #region Private Methods

        private void SyncColorIdsToLabelIds(List<LabelId> colorLabels)
        {
            for (int i = 0; i < colorLabels.Count; i++)
            {
                LabelId label = colorLabels[i];
                if (Colors.Count < i + 1)
                {
                    Colors.Add(new ColorId(label.Id, Color.white));
                    continue;
                }

                Colors[i].SetId(label.Id);
            }
        }

        private void SyncSpriteIdsToLabelIds(List<LabelId> spriteLabels)
        {
            for (int i = 0; i < spriteLabels.Count; i++)
            {
                LabelId label = spriteLabels[i];
                if (Sprites.Count < i + 1)
                {
                    Sprites.Add(new SpriteId(label.Id, null));
                    continue;
                }

                Sprites[i].SetId(label.Id);
            }
        }

        private void SyncTextureIdsToLabelIds(List<LabelId> textureLabels)
        {
            for (int i = 0; i < textureLabels.Count; i++)
            {
                LabelId label = textureLabels[i];
                if (Textures.Count < i + 1)
                {
                    Textures.Add(new TextureId(label.Id, null));
                    continue;
                }

                Textures[i].SetId(label.Id);
            }
        }

        private void SyncFontIdsToLabelIds(List<LabelId> fontLabels)
        {
            for (int i = 0; i < fontLabels.Count; i++)
            {
                LabelId label = fontLabels[i];
                if (Fonts.Count < i + 1)
                {
                    Fonts.Add(new FontId(label.Id, DefaultFont));
                    continue;
                }

                Fonts[i].SetId(label.Id);
            }
        }

        private void SyncFontAssetIdsToLabelIds(List<LabelId> fontAssetLabels)
        {
#if dUI_TextMeshPro
            for (int i = 0; i < fontAssetLabels.Count; i++)
            {
                LabelId label = fontAssetLabels[i];
                if (FontAssets.Count < i + 1)
                {
                    FontAssets.Add(new FontAssetId(label.Id, null));
                    continue;
                }

                FontAssets[i].SetId(label.Id);
            }
#endif
        }

        #endregion
    }
}