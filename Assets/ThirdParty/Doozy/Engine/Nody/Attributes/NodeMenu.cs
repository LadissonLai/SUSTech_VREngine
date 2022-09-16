// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine.Nody.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeMenu : Attribute
    {
        public readonly string MenuName;
        public readonly int Order;
        public readonly bool AddSeparatorAfter;
        public readonly bool AddSeparatorBefore;

        /// <inheritdoc />
        /// <summary> Manually supply Node class with a context menu path </summary>
        /// <param name="menuName"> Path to this Node in the context menu. Null or empty hides it. </param>
        /// <param name="order"> The order by which the menu items are displayed. </param>
        /// <param name="addSeparatorAfter"> Add a separator after this item </param>
        /// <param name="addSeparatorBefore"> Add a separator before this item </param>
        public NodeMenu(string menuName, int order, bool addSeparatorAfter = false, bool addSeparatorBefore = false)
        {
            MenuName = menuName;
            Order = order;
            AddSeparatorAfter = addSeparatorAfter;
            AddSeparatorBefore = addSeparatorBefore;
        }
    }
}