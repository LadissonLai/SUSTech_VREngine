// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Nody.Models
{
    /// <summary>
    ///     Describes a Socket's connection behavior (if it can have multiple Connections or just one)
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        ///    Socket can have only one Connection at a time (overriding any existing connection upon establishing a new connection)
        /// </summary>
        Override,

        /// <summary>
        ///     Socket can have multiple connections
        /// </summary>
        Multiple
    }
}