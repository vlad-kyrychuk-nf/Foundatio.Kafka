﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.AsyncEx;
using Foundatio.Messaging;
using Foundatio.Tests.Extensions;
using Foundatio.Tests.Messaging;
using Foundatio.Utility;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Foundatio.Kafka.Tests.Messaging; 

public class KafkaMessageBusTests : MessageBusTestBase {
    private readonly string _topic = $"test_topic_{DateTime.Now.Ticks}";
    public KafkaMessageBusTests(ITestOutputHelper output) : base(output) { }

    protected override IMessageBus GetMessageBus(Func<SharedMessageBusOptions, SharedMessageBusOptions> config = null) {
        return new KafkaMessageBus(o => {
            o.LoggerFactory(Log);
            o.BootStrapServers("localhost:9092");
            o.AutoCommitIntervalMs(100);
            o.Topic(_topic);
            //o.GroupId("GroupId");
            o.GroupId(Guid.NewGuid().ToString());
            return o;
        });
    }

    //protected override Task CleanupMessageBusAsync(IMessageBus messageBus) {
      
    //    return base.CleanupMessageBusAsync(messageBus);
    //}

    [Fact]
    public override Task CanSendMessageAsync() {
        Log.MinimumLevel = LogLevel.Trace;
        return base.CanSendMessageAsync();
    }

    [Fact]
    public override Task CanHandleNullMessageAsync() {
        return base.CanHandleNullMessageAsync();
    }

    [Fact]
    public override Task CanSendDerivedMessageAsync() {
        return base.CanSendDerivedMessageAsync();
    }

    [Fact]
    public override Task CanSendDelayedMessageAsync() {
        Log.MinimumLevel = LogLevel.Trace;
        return base.CanSendDelayedMessageAsync();
    }

    [Fact]
    public override  Task CanSubscribeConcurrentlyAsync() {
        return base.CanSubscribeConcurrentlyAsync();
    }

    [Fact]
    public override Task CanReceiveMessagesConcurrentlyAsync() {
        Log.MinimumLevel = LogLevel.Trace;
        return base.CanReceiveMessagesConcurrentlyAsync();
    }

    [Fact]
    public override Task CanSendMessageToMultipleSubscribersAsync() {
        return base.CanSendMessageToMultipleSubscribersAsync();
    }

    [Fact]
    public override Task CanTolerateSubscriberFailureAsync() {
        return base.CanTolerateSubscriberFailureAsync();
    }

    [Fact]
    public override Task WillOnlyReceiveSubscribedMessageTypeAsync() {
        return base.WillOnlyReceiveSubscribedMessageTypeAsync();
    }

    [Fact]
    public override Task WillReceiveDerivedMessageTypesAsync() {
        return base.WillReceiveDerivedMessageTypesAsync();
    }

    [Fact]
    public override Task CanSubscribeToAllMessageTypesAsync() {
        return base.CanSubscribeToAllMessageTypesAsync();
    }

    [Fact]
    public override Task CanSubscribeToRawMessagesAsync() {
        return base.CanSubscribeToRawMessagesAsync();
    }

    [Fact]
    public override Task CanCancelSubscriptionAsync() {
        return base.CanCancelSubscriptionAsync();
    }

    [Fact]
    public override Task WontKeepMessagesWithNoSubscribersAsync() {
        return base.WontKeepMessagesWithNoSubscribersAsync();
    }

    [Fact]
    public override Task CanReceiveFromMultipleSubscribersAsync() {
        Log.MinimumLevel = LogLevel.Trace;
        return base.CanReceiveFromMultipleSubscribersAsync();
    }

    [Fact]
    public override void CanDisposeWithNoSubscribersOrPublishers() {
        base.CanDisposeWithNoSubscribersOrPublishers();
    }

    [Fact]
    public void MessageQueueWillPersistAndNotLoseMessages() {
        Log.MinimumLevel = LogLevel.Trace;
        var messageBus1 = new KafkaMessageBus(o => o
            .LoggerFactory(Log));
        var t = new AutoResetEvent(false);
        var cts = new CancellationTokenSource();
        messageBus1.SubscribeAsync<SimpleMessageA>(msg => {
            _logger.LogTrace("Got message {Data}", msg.Data);
            t.Set();
        }, cts.Token);
        messageBus1.PublishAsync(new SimpleMessageA {Data = "Audit message 1"});
        t.WaitOne(TimeSpan.FromSeconds(5));
        cts.Cancel();

        messageBus1.PublishAsync(new SimpleMessageA {Data = "Audit message 2"});

        cts = new CancellationTokenSource();
        messageBus1.SubscribeAsync<SimpleMessageA>(msg => {
            _logger.LogTrace("Got message {Data}", msg.Data);
            t.Set();
        }, cts.Token);
        t.WaitOne(TimeSpan.FromSeconds(5));
        cts.Cancel();

        messageBus1.PublishAsync(new SimpleMessageA {Data = "Audit offline message 1"});
        messageBus1.PublishAsync(new SimpleMessageA {Data = "Audit offline message 2"});
        messageBus1.PublishAsync(new SimpleMessageA {Data = "Audit offline message 3"});

        messageBus1.Dispose();
        var messageBus2 = new KafkaMessageBus(o => o.LoggerFactory(Log));
        cts = new CancellationTokenSource();
        messageBus2.SubscribeAsync<SimpleMessageA>(msg => {
            _logger.LogTrace("Got message {Data}", msg.Data);
            t.Set();
        }, cts.Token);
        messageBus2.PublishAsync(new SimpleMessageA {Data = "Another audit message 4"});
    }
}