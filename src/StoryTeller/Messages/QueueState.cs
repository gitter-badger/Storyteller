﻿using System;

namespace StoryTeller.Messages
{
    [Serializable]
    public class QueueState : ClientMessage
    {
        public QueueState() : base("queue-state")
        {
        }

        public string[] queued = new string[0];
        public string running = null;
    }
}