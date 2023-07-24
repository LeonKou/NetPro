using Pulsar.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.Pulsar
{
    public interface IPulsarQueneService
    {
        /// <summary>
        /// 消息订阅(正则topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumePatternAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);

        /// <summary>
        /// 批量消息订阅(正则topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumePatternBatchAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);

        /// <summary>
        /// 消息订阅(完整topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumeAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);


        /// <summary>
        /// 批量消息订阅(完整topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumeBatchAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);


        /// <summary>
        /// 消息订阅(正则topic)
        /// </summary>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumePatternAsync(string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);

        /// <summary>
        /// 批量消息订阅(正则topic)
        /// </summary>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumePatternBatchAsync(string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);

        /// <summary>
        /// 消息订阅(完整topic)
        /// </summary>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumeAsync(string topic, string subscriptiont, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);


        /// <summary>
        /// 批量消息订阅(完整topic)
        /// </summary>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConsumeBatchAsync(string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest);

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="topic">主题</param>
        /// <param name="data">消息实例</param>
        /// <returns></returns>
        Task ProduceMessagesAsync<T>(string topic, T? data);

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="topic">主题</param>
        /// <param name="data">消息实例</param>
        /// <returns></returns>
        Task ProduceMessagesServiceAsync<T>(string topic, T? data, int producerNum = 1);


    }
}
