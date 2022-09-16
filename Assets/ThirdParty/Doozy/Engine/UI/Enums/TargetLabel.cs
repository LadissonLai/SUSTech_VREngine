// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes the types of labels available for some UI components </summary>
    public enum TargetLabel
    {
        /// <summary> No label is used </summary>
        None,

        /// <summary> A Text component is used as the target label </summary>
        Text,

        /// <summary> A TextMeshProUGUI component is used as the target label. Available only if TextMeshPro support is enabled </summary>
        TextMeshPro
    }
}