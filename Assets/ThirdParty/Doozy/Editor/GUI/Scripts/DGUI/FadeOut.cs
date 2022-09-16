// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor.AnimatedValues;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class FadeOut
        {
            public static bool Begin(float faded, bool indentContent = true)
            {
                AlphaGroup.Begin(faded);
                return FoldOut.Begin(faded, indentContent);
            }

            public static bool Begin(AnimBool expanded, bool indentContent = true) { return Begin(expanded.faded, indentContent); }

            public static void End(float faded, bool addExtraSpaceWhenExpanded = true, float customAlpha = 1f)
            {
                FoldOut.End(faded, addExtraSpaceWhenExpanded);
                AlphaGroup.End(customAlpha);
            }

            public static void End(AnimBool expanded, bool addExtraSpaceWhenExpanded = true, float customAlpha = 1f) { End(expanded.faded, addExtraSpaceWhenExpanded, customAlpha); }
        }
    }
}