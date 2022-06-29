using System.Threading;

namespace XXX.API.StartTask
{
    /// <summary>
    /// 通过初始化一次后的定时执行作业示例
    /// 固定周期的定时器作业示例代码
    /// </summary>
    public class TimerjobStartTask //: IStartupTaskAsync
    {
        private readonly IConfiguration _configuration;
        public TimerjobStartTask(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public int Order => 0;

        public async Task ExecuteAsync()
        {
            await Task.Yield();
            //间隔3秒执行一次
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            try
            {
                //不会发生重入，只允许有一个消费者
                while (await timer.WaitForNextTickAsync())
                {
                    Console.WriteLine($"【定时任务被触发】Tick {DateTime.Now}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("【定时任务被取消】Operation cancelled");
            }
        }
    }

    /// <summary>
    /// 后台服务示例，3秒执行一次方法
    /// 需在startup中注册services.AddHostedService<TimedHostedService>();
    /// </summary>
    public class TimedHostedService : BackgroundService
    {
        private readonly PeriodicTimer _timer;
        private readonly ILogger<TimedHostedService> _logger;

        public TimedHostedService(
            ILogger<TimedHostedService> logger)
        {
            _logger = logger;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    Console.WriteLine($"【定时任务被触发】Tick {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Execute exception");
                }
                finally
                {
                    _logger.LogInformation("Execute finished");
                }
            }
            //while (!stoppingToken.IsCancellationRequested)
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
