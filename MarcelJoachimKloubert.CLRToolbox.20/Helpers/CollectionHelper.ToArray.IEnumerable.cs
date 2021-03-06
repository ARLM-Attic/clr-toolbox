﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de

using MarcelJoachimKloubert.CLRToolbox.Collections;
using System;
using System.Collections;

namespace MarcelJoachimKloubert.CLRToolbox.Helpers
{
    static partial class CollectionHelper
    {
        #region Methods (1)

        // Public Methods (1) 

        /// <summary>
        /// Converts a general sequence to an object array.
        /// </summary>
        /// <param name="seq">The sequence to convert / cast.</param>
        /// <returns><paramref name="seq" /> as array.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="seq" /> is <see langword="null" />.</exception>
        public static object[] ToArray(IEnumerable seq)
        {
            if (seq == null)
            {
                throw new ArgumentNullException("seq");
            }

            IGeneralList genList = seq as IGeneralList;
            if (genList != null)
            {
                // use build-in
                return genList.ToArray();
            }

            return ToArray<object>(AsSequence<object>(seq));
        }

        #endregion Methods
    }
}