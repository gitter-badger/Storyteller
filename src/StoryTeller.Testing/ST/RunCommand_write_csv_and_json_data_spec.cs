﻿using System;
using System.IO;
using FubuCore;
using NUnit.Framework;
using Shouldly;
using ST.CommandLine;

namespace StoryTeller.Testing.ST
{
    [TestFixture]
    public class RunCommand_write_csv_and_json_data_spec
    {
        public readonly string Path = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
               .AppendPath("Storyteller.Samples");


        [Test]
        public void write_csv_results()
        {
            var file = "perf-" + Guid.NewGuid().ToString() + ".csv";

            var input = new RunInput
            {
                Path = Path,
                CsvFlag = file
            };

            new RunCommand().Execute(input);

            File.Exists(file).ShouldBe(true);
        }

        [Test]
        public void write_json_results()
        {
            var file = "perf-" + Guid.NewGuid().ToString() + ".csv";

            var input = new RunInput
            {
                Path = Path,
                JsonFlag = file
            };

            new RunCommand().Execute(input);

            File.Exists(file).ShouldBe(true);
        }
    }
}