﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de


namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Modules
{
    partial class HttpModuleBase
    {
        #region INTERFACE: IAfterHandleRequestContext

        /// <summary>
        /// Stores the data for a <see cref="HttpModuleBase.OnAfterHandleRequest(IAfterHandleRequestContext)" /> method call.
        /// </summary>
        protected interface IAfterHandleRequestContext
        {
            #region Data Members (2)

            /// <summary>
            /// Gets the underlying HTTP request.
            /// </summary>
            IHttpRequestContext HttpRequest { get; }

            /// <summary>
            /// Gets if <see cref="HttpModuleBase.OnHandleRequest(IHandleRequestContext)" />
            /// was invoked or not.
            /// Is <see langword="true" /> by default.
            /// </summary>
            bool RequestWasHandled { get; }

            #endregion Data Members
        }

        #endregion

        #region CLASS: AfterHandleRequestContext

        private sealed class AfterHandleRequestContext : IAfterHandleRequestContext
        {
            #region Fields (2)

            private IHttpRequestContext _httpRequest;
            private bool _requestWasHandled;

            #endregion Fields

            #region Properties (2)

            public IHttpRequestContext HttpRequest
            {
                get { return this._httpRequest; }

                internal set { this._httpRequest = value; }
            }

            public bool RequestWasHandled
            {
                get { return this._requestWasHandled; }

                internal set { this._requestWasHandled = value; }
            }

            #endregion Properties
        }

        #endregion
    }
}
