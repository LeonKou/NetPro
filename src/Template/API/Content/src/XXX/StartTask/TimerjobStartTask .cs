using System.Threading;

namespace XXX.StartTask
{
    /// <summary>
    /// 通过初始化一次后的定时执行作业示例
    /// 固定周期的定时器作业示例代码
    /// </summary>
    public class TimerjobStartTask : IStartupTaskAsync
    {
        public int Order => 0;

        private readonly IConfiguration _configuration;
        public TimerjobStartTask(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task ExecuteAsync()
        {
            //间隔3秒执行一次
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            try
            {
                //不会发生重入，只允许有一个消费者
                while (await timer.WaitForNextTickAsync())
                {
                    //Console.WriteLine($"【定时任务被触发】Tick {DateTime.Now}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("【定时任务被取消】Operation cancelled");
            }
        }
    }
}
