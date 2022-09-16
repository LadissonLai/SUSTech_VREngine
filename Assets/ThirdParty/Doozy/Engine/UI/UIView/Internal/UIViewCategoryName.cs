// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Internal
{
    /// <summary>
    ///     Data class that holds a view category name, a view name and an instant action bool value.
    ///     Used by the UINode to SHOW / HIDE UIViews.
    /// </summary>
    [Serializable]
    public class UIViewCategoryName
    {
        #region Constants

        private const string DEFAULT_CATEGORY = NamesDatabase.GENERAL;
        private const string DEFAULT_NAME = NamesDatabase.UNNAMED;
        private const bool DEFAULT_INSTANT_ACTION = false;

        #endregion

        #region Public Variables

        /// <summary> UIView view category name </summary>
        public string Category;

        /// <summary> Determines if the animation should happen instantly (in zero seconds) </summary>
        public bool InstantAction;

        /// <summary> UIView view name </summary>
        public string Name;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        public UIViewCategoryName() { Reset(); }

        /// <summary> Initializes a new instance of the class with the passed view category and view name </summary>
        /// <param name="viewCategory"> View category to search for</param>
        /// <param name="viewName"> View name to search for (found in the view category) </param>
        public UIViewCategoryName(string viewCategory, string viewName)
        {
            Reset();
            Category = viewCategory;
            Name = viewName;
        }

        /// <summary> Initializes a new instance of the class with the passed view category, view name and an instant action bool value </summary>
        /// <param name="viewCategory"> View category to search for</param>
        /// <param name="viewName"> View name to search for (found in the view category) </param>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public UIViewCategoryName(string viewCategory, string viewName, bool instantAction)
        {
            Reset();
            Category = viewCategory;
            Name = viewName;
            InstantAction = instantAction;
        }

        #endregion

        #region Public Methods

        /// <summary> Returns a deep copy </summary>
        public UIViewCategoryName Copy()
        {
            return new UIViewCategoryName
                   {
                       Category = Category,
                       Name = Name,
                       InstantAction = InstantAction
                   };
        }
        
        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            Category = DEFAULT_CATEGORY;
            Name = DEFAULT_NAME;
            InstantAction = DEFAULT_INSTANT_ACTION;
        }

        #endregion
    }
}