﻿using System.Xml.Linq;
using BaroModCLI.Util;

namespace BaroModCLI.PatchOperations
{
    internal static class ConstructOverride
    {
        public static void Override(XElement elt)
        {
            XElement itemElement = elt.GetSecondLevelAncestor();
            if (itemElement.Name.LocalName == "Override") return;

            var overrideElement = new XElement("Override");
            overrideElement.Add(itemElement);
            elt.Document?.Root?.Add(overrideElement);
            itemElement.Remove();
        }

        public static XDocument OverrideRoot(XDocument doc)
        {
            return new XDocument(new XElement("Override", doc.Root));
        }
    }
}
