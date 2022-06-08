﻿using System;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Foundatio.Tests.Messaging;
using Xunit;
using Xunit.Abstractions;

namespace Foundatio.Kafka.Tests.Messaging;

public class KafkaMessageBusFanOutTests : KafkaMessageBusTestBase {

    public KafkaMessageBusFanOutTests(ITestOutputHelper output) : base(output) { }

    protected override IMessageBus GetMessageBus(Func<SharedMessageBusOptions, SharedMessageBusOptions> config = null) {
        return new KafkaMessageBus(o => o
            .BootstrapServers("127.0.0.1:9092")
            .Topic(_topic)
            .TopicReplicationFactor(1)
            .TopicNumberOfPartitions(1)
            .GroupId(Guid.NewGuid().ToString("N"))
            .EnableAutoCommit(false)
            .EnableAutoOffsetStore(false)
            .AllowAutoCreateTopics(true)
            .LoggerFactory(Log)
        );
    }

    [Fact]
    public override Task CanReceiveFromMultipleSubscribersAsync() {
        return base.CanReceiveFromMultipleSubscribersAsync();
    }

    [Fact]
    public override Task CanTolerateSubscriberFailureAsync() {
        return base.CanTolerateSubscriberFailureAsync();
    }

    [Fact]
    public override Task CanSubscribeConcurrentlyAsync() {
        return base.CanSubscribeConcurrentlyAsync();
    }

    [Fact]
    public override Task CanReceiveMessagesConcurrentlyAsync() {
        return base.CanReceiveMessagesConcurrentlyAsync();
    }

    [Fact]
    public override Task CanSendMessageToMultipleSubscribersAsync() {
        return base.CanSendMessageToMultipleSubscribersAsync();
    }

    [Fact]
    public override Task WillOnlyReceiveSubscribedMessageTypeAsync() {
        return base.WillOnlyReceiveSubscribedMessageTypeAsync();
    }

    [Fact]
    public override Task CanCancelSubscriptionAsync() {
        return base.CanCancelSubscriptionAsync();
    }
}