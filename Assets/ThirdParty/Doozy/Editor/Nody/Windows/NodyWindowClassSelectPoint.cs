// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        /// <summary> Holds selection start data </summary>
        private class SelectPoint
        {
            public readonly float X;
            public readonly float Y;

            public SelectPoint(Vector2 position)
            {
                X = position.x;
                Y = position.y;
            }
        }
    }
}