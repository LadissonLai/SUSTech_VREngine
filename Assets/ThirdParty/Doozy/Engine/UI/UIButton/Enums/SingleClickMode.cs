// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Setting for the UIButton OnClick trigger that mark if a click should be registered instantly, without checking if it's a double click, or not </summary>
    public enum SingleClickMode
    {
        /// <summary>
        ///     The click will get registered instantly without checking if it's a double click or not.
        ///     <para>This is the normal behavior of a single click in any OS.</para>
        ///     <para>Use this if you want to make sure a single click will get executed before a double click (dual actions).</para>
        ///     <para>(usage example: SingleClick - selects, DoubleClick - executes an action)</para>
        /// </summary>
        Instant,

        /// <summary>
        ///     The click will get registered after checking if it's a double click or not (it has a small delay)
        ///     <para>If it's a double click, the single click will not get triggered.</para>
        ///     <para>Use this if you want to make sure the user does not execute a single click before a double click.</para>
        ///     <para>The downside is that there is a delay when executing the single click (the delay is the double click register interval), so make sure you take that into account</para>
        /// </summary>
        Delayed
    }
}