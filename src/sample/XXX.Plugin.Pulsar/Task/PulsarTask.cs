using Microsoft.Extensions.Hosting;
using NetPro.Pulsar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.Pulsar.TaskService
{
    public class PulsarTask : BackgroundService
    {
        private readonly IPulsarQueneService _pulsarQuene;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pulsarQuene">服务</param>
        public PulsarTask(IPulsarQueneService pulsarQuene)
        {
            _pulsarQuene = pulsarQuene;
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="event"></param>
        private async Task HandleEventAsync(string topic, byte[] @event)
        {
            try
            {
                Console.WriteLine($"订阅到的pulsar消息：{Encoding.UTF8.GetString(@event)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"message:{ex.Message},Source:{ex.Source},Trace:{ex.StackTrace}");
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _pulsarQuene.ConsumeAsync("persistent://public/default/service-test-pulsar", "test-group", HandleEventAsync, stoppingToken);
            await Task.CompletedTask;
        }
    }
}
