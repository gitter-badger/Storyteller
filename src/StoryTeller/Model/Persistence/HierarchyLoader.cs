﻿using System;
using System.IO;
using System.Linq;
using FubuCore;

namespace StoryTeller.Model.Persistence
{
    public class HierarchyLoader
    {
        public static readonly string SpecDirectory;

        public static string SelectSpecPath(string baseDirectory)
        {
            var specPath = baseDirectory.AppendPath("Specs");
            if (Directory.Exists(specPath)) return specPath;

            var testsPath = baseDirectory.AppendPath("Tests");
            if (Directory.Exists(testsPath))
            {
                return testsPath;
            }

            return specPath;
        }

        static HierarchyLoader()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            SpecDirectory = SelectSpecPath(baseDirectory);
        }
        

        public static readonly IFileSystem FileSystem = new FileSystem();

        public static Suite ReadHierarchy()
        {
            return ReadHierarchy(SpecDirectory);
        }

        public static Suite ReadHierarchy(string folder)
        {
            var suite = ReadSuite(folder);
            suite.name = string.Empty;
            suite.WritePath(string.Empty);

            return suite;
        }

        public static Suite ReadSuite(string folder)
        {
            return new Suite
            {
                name = Path.GetFileName(folder),
                specs = FileSystem.FindFiles(folder, FileSet.Shallow("*.xml")).Select(ReadSpecHeader).ToArray(),
                suites = FileSystem.ChildDirectoriesFor(folder).Select(ReadSuite).ToArray(),
                Folder = folder
            };
        }

        public static Specification ReadSpecHeader(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var reader = System.Xml.XmlReader.Create(stream);
                /* TODO: on mono you need to Read otherwise the XmlReader
                 * is in an Initial state, which msdn says means you shouldn't call
                 * ReadToNextSibling. But calling Read progresses the reader forward so
                 * that ReadToNextSibling skips the test without a <?xml> first line...
                 */
                reader.Read();
                if (!(reader.IsStartElement() || reader.Name == "Test" || reader.Name == "Spec"))
                {
                    reader.ReadToNextSibling("*");
                }

                var spec = new Specification
                {
                    id = reader.GetAttribute("id") ?? Guid.NewGuid().ToString(),
                    name = reader.GetAttribute("name"),
                    Lifecycle = reader.GetAttribute("lifecycle").AsLifecycle(),
                    Filename = filename,
                    SpecType = SpecType.header
                };

                var maxRetries = reader.GetAttribute(XmlConstants.MaxRetries);
                spec.MaxRetries = maxRetries.IsEmpty() ? 0 : int.Parse(maxRetries);

                return spec;
            }
        }
    }
}