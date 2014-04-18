﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. http://blog.marcel-kloubert.de

using System.Reflection;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.ClrDocToMediaWiki.Classes
{
    /// <summary>
    /// Documentation of a property parameter.
    /// </summary>
    public sealed class PropertyParameterDocumentation : MemberItemDocumentationBase<PropertyInfo, PropertyDocumentation, ParameterInfo>
    {
        #region Constructors (1)

        internal PropertyParameterDocumentation(PropertyDocumentation parent, ParameterInfo parameter, XElement xml)
            : base(parent, parameter, xml ?? new XElement("param"))
        {

        }

        #endregion Constructors

        #region Methods (1)

        // Public Methods (1) 

        /// <inheriteddoc />
        public override string ToString()
        {
            return this.ClrItem.Name;
        }

        #endregion Methods
    }
}