﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de


using System;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Data
{
    /// <summary>
    /// A basic converter.
    /// </summary>
    public abstract class ConverterBase : TMObject, IConverter
    {
        #region Constructors (2)

        /// /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase" /> class.
        /// </summary>
        /// <param name="syncRoot">The value for the <see cref="TMObject._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syncRoot" /> is <see langword="null" />.
        /// </exception>
        protected ConverterBase(object syncRoot)
            : base(syncRoot)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase" /> class.
        /// </summary>
        protected ConverterBase()
            : base()
        {

        }

        #endregion Constructors

        #region Methods (2)

        // Public Methods (2) 

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IConverter.ChangeType{T}(object)" />
        public virtual T ChangeType<T>(object value)
        {
            return this.ChangeType<T>(value, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IConverter.ChangeType{T}(object, IFormatProvider)" />
        public abstract T ChangeType<T>(object value, IFormatProvider provider);

        #endregion Methods
    }
}