﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BaroModCLI.Util;

namespace BaroModCLI.PatchOperations
{
    public static class ApplyPatchOperation
    {
        public static XDocument ApplyAll(XDocument diff)
        {
            return ApplyAll(diff, Directory.GetCurrentDirectory());
        }

        public static XDocument ApplyAll(XDocument diff, string relativeDirectory)
        {
            string file = diff.Root.GetAttributeSafe("file");
            if (file == null) return null;

            file = DiffUtils.RelativeToAbsoluteFilepath(file, relativeDirectory);
            XDocument document = XDocument.Load(file);
            document = ApplyAll(diff, document);
            return document;
        }

        private static XDocument ApplyAll(XDocument diff, XDocument document)
        {
            DiffUtils.CleanHeader(diff, document, out RootBehaviour overrideRoot);

            HashSet<string> overrideXpaths =
                XmlUtils.GetFilteredXPaths(diff, document,
                                           (patch, elt) =>
                                               patch.ParseBoolAttribute("override"));
            HashSet<string> saveXpaths =
                XmlUtils.GetFilteredXPaths(diff, document,
                                           (patch, elt) =>
                                               !patch.ParseBoolAttribute("override") &&
                                               (patch.Name.LocalName != "remove" || elt.Ancestors().Count() != 1));

            document = diff.Root?.Elements()
                           .Aggregate(document,
                                      (current, patch) => Apply(patch, current));

            diff.Root.ParseBoolAttribute("cleanup", out bool cleanup);

            if (cleanup && document?.Root != null)
                foreach (XElement element in document.Root.Elements().Reverse())
                {
                    string xpath = element.GetAbsoluteXPath();
                    if (overrideXpaths.Contains(xpath))
                        ConstructOverride.Override(element);
                    else if (!saveXpaths.Contains(xpath)) element.Remove();
                }

            switch (overrideRoot)
            {
                case RootBehaviour.Override:
                    document = ConstructOverride.OverrideRoot(document);
                    break;
                case RootBehaviour.OverrideReplace:
                    document.Root.Name = "Override";
                    break;
                case RootBehaviour.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(overrideRoot));
            }

            return document;
        }

        private static XDocument Apply(XElement patch, XDocument document)
        {
            document = patch.Name.LocalName switch
                       {
                           "add" => new PatchOperationAdd(patch, document).Apply(),
                           "remove" => new PatchOperationRemove(patch, document).Apply(),
                           "replace" => new PatchOperationEdit(patch, document).Apply(),
                           _ => document
                       };
            return document;
        }
    }
}
