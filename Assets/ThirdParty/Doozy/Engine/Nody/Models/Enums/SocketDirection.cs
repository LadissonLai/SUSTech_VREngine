// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Nody.Models
{
    /// <summary>
    ///     Describes a Socket's flow (Input/Output)
    /// </summary>
    public enum SocketDirection
    {
        /// <summary>
        ///     An input Socket can only connect to an output Socket
        /// </summary>
        Input,

        /// <summary>
        ///     An output Socket can only connect to an input Socket
        /// </summary>
        Output
    }
}