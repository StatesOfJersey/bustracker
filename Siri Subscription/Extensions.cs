using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Siri_Subscription
{
    public static class XmlExtensions
    {
        /// <summary>
        /// XPaths are really clunky with namespaces in so let's get rid of them
        /// </summary>
        /// <param name="document"></param>
        public static void StripSiriNamespace(this XDocument document)
        {
            if (document.Root == null) return;

            foreach (var element in document.Root.DescendantsAndSelf())
            {
                element.Name = element.Name.LocalName;
                element.ReplaceAttributes(GetAttributesWithoutNamespace(element));
            }
        }

        static IEnumerable GetAttributesWithoutNamespace(XElement xElement)
        {
            return xElement.Attributes()
                .Where(x => !x.IsNamespaceDeclaration)
                .Select(x => new XAttribute(x.Name.LocalName, x.Value));
        }
    }
}
