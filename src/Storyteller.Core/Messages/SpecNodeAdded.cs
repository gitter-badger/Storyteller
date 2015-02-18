﻿using Storyteller.Core.Model.Persistence;

namespace Storyteller.Core.Messages
{
    public class SpecNodeAdded : ClientMessage
    {
        public SpecNodeAdded() : base("spec-node-added")
        {
        }

        public string suite;
        public SpecNode node;
    }
}