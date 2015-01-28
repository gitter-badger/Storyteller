﻿using System;
using System.Collections.Generic;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using Storyteller.Core.Model;

namespace Storyteller.Core.Persistence
{
    public class XmlReader : XmlConstants
    {
        public static Specification ReadFromFile(string path)
        {
            var document = new XmlDocument().FromFile(path);
            return ReadFromXml(document);
        }

        public static Specification ReadFromXml(XmlDocument document)
        {
            var spec = new Specification();
            var top = document.DocumentElement;
            spec.Name = top.GetAttribute("name");

            var lifecycle = top.GetAttribute(LifecycleAtt);
            spec.Lifecycle = lifecycle.IsEmpty()
                ? Lifecycle.Acceptance
                : Enum.Parse(typeof (Lifecycle), lifecycle).As<Lifecycle>();

            spec.Id = top.ReadId();
            var maxRetries = top.GetAttribute(MaxRetries);
            spec.MaxRetries = maxRetries.IsEmpty() ? 0 : int.Parse(maxRetries);

            var tags = top.GetAttribute(TagsAtt);
            if (tags.IsNotEmpty())
            {
                spec.Tags.AddRange(tags.ToDelimitedArray());
            }


            top.ForEachElement(element =>
            {
                if (element.Name == Comment)
                {
                    spec.Children.Add(ReadComment(element));
                }
                else
                {
                    spec.Children.Add(ReadSection(element));
                }
            });

            return spec;
        }

        public static Section ReadSection(XmlElement element)
        {
            var section = new Section(element.Name) {Id = element.GetAttribute(Id)};

            element.ForEachElement(child =>
            {
                if (child.Name == Comment)
                {
                    section.Children.Add(ReadComment(child));
                }
                else
                {
                    section.Children.Add(ReadStep(child));                    
                }
            });


            return section;
        }

        public static Step ReadStep(XmlElement child)
        {
            var step = new Step(child.Name) {Id = child.ReadId()};

            foreach (XmlAttribute att in child.Attributes)
            {
                if (att.Name == "isStep") break; // Old detritus

                step.Values[att.Name] = att.Value;
            }

            child.ForEachElement(collection =>
            {
                var section = ReadSection(collection);
                step.Collections[collection.Name] = section;
            });

            return step;
        }

        private static Comment ReadComment(XmlElement element)
        {
            return new Comment{Id = element.ReadId(), Text = element.InnerText};
        }
    }
}