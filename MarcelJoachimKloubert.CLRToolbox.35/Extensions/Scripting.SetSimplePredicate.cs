﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de


using System;
using System.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Helpers;
using MarcelJoachimKloubert.CLRToolbox.Scripting;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        // Public Methods (1) 

        /// <summary>
        /// Sets a simple predicate with a variable number of parameters.
        /// </summary>
        /// <typeparam name="TExecutor"></typeparam>
        /// <param name="executor">The script executor.</param>
        /// <param name="predicateName">The name of the function in the script.</param>
        /// <param name="predicate">The predicate delegate to set.</param>
        /// <returns>The object in <paramref name="executor" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executor" /> is <see langword="null" />.
        /// </exception>
        public static TExecutor SetSimplePredicate<TExecutor>(this TExecutor executor, IEnumerable<char> predicateName, ScriptExecutorBase.SimplePredicate predicate)
               where TExecutor : IScriptExecutor
        {
            ScriptHelper.SetSimplePredicate(executor, predicateName, predicate);
            return executor;
        }

        #endregion Methods
    }
}
