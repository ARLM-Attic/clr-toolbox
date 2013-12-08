﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de


using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// Describes a HTTP request.
    /// </summary>
    public interface IHttpRequest : ITMObject
    {
        #region Data Members (5)

        /// <summary>
        /// Gets the request address.
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// Gets the list of request headers.
        /// </summary>
        IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the upper case name of the HTTP method if available.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the endpoint of the requesting client.
        /// </summary>
        ITcpAddress RemoteAddress { get; }

        /// <summary>
        /// Gets the requesting user if available.
        /// </summary>
        IPrincipal User { get; }

        #endregion Data Members

        #region Operations (1)

        /// <summary>
        /// Returns a new stream with the body data.
        /// </summary>
        /// <returns>The stream with the body data.</returns>
        Stream GetBody();

        #endregion Operations
    }
}