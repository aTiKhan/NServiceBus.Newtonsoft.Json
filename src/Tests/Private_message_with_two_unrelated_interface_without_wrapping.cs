﻿using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Private_message_with_two_unrelated_interface_without_wrapping
{

    [Test]
    public void Deserialize()
    {
        var messageMapper = new MessageMapper();
        messageMapper.Initialize(new[]
        {
            typeof(IMyEventA),
            typeof(IMyEventB)
        });
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null);

        using (var stream = new MemoryStream())
        {
            var msg = new CompositeMessage
            {
                IntValue = 42,
                StringValue = "Answer"
            };

            serializer.Serialize(msg, stream);

            stream.Position = 0;

            var result = serializer.Deserialize(stream, new[]
            {
                typeof(IMyEventA),
                typeof(IMyEventB)
            });
            var a = (IMyEventA) result[0];
            var b = (IMyEventB) result[1];
            Assert.AreEqual(42, b.IntValue);
            Assert.AreEqual("Answer", a.StringValue);
        }
    }

    class CompositeMessage : IMyEventA, IMyEventB
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }
    }

    public interface IMyEventA
    {
        string StringValue { get; set; }
    }
    public interface IMyEventB
    {
        int IntValue { get; set; }
    }
}