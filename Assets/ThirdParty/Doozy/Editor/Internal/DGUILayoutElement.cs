// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Editor.Internal
{
    public class DGUILayoutElement
    {
        public DGUIElement Element { get; private set; }
        public float Weight { get; private set; }
        public DGUIElement.DrawMode DrawMode { get; private set; }

        public DGUILayoutElement(DGUIElement element, float weight = 1f, DGUIElement.DrawMode drawMode = DGUIElement.DrawMode.LabelAndField)
        {
            Element = element;
            Weight = weight;
            DrawMode = drawMode;
        }
    }
}