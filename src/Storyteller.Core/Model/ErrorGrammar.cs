﻿using System;
using FubuCore;
using Storyteller.Core.Conversion;
using Storyteller.Core.Grammars;

namespace Storyteller.Core.Model
{
    public class ErrorGrammar : GrammarModel, IGrammar
    {
        private readonly string _key;
        private readonly string _message;

        public ErrorGrammar(string key, string error) : base("error")
        {
            _key = key;
            _message = "Grammar '{0}' is in an invalid state. See the grammar errors".ToFormat(_key);
            AddError(new GrammarError{error = error});
        }

        public IExecutionStep CreatePlan(Step step, FixtureLibrary library)
        {
            return new InvalidGrammarStep(new StepValues(step.Id), _message);
        }

        GrammarModel IGrammar.Compile(Fixture fixture, CellHandling cells)
        {
            return this;
        }

        public string Key { get; set; }
    }
}