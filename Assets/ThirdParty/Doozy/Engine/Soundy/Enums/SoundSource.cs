// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Soundy
{
    /// <summary> Describes types of available sound sources </summary>
    public enum SoundSource
    {
        /// <summary> Soundy - Sound Manager. Powerful audio solution perfectly integrated with DoozyUI. </summary>
        Soundy,

        /// <summary> Audio clip direct reference. As simple as a drag and a drop. </summary>
        AudioClip,

        /// <summary> External audio solution plugin created by Dark Tonic. To work, this option requires that the Master Audio plugin be installed in the project </summary>
        MasterAudio
    }
}