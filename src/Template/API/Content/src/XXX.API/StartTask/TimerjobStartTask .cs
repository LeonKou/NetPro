using System.Threading;

namespace XXX.API.StartTask
{
    /// <summary>
    /// 固定周期的定时器作业示例代码
    /// </summary>
    public class TimerjobStartTask //: IStartupTaskAsync
    {
        public int Order => 0;

        public async Task ExecuteAsync()
        {
            //间隔3秒执行一次
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            try
            {
                //不会发生重入，只允许有一个消费者
                while (await timer.WaitForNextTickAsync())
                {
                    Console.WriteLine($"Tick {DateTime.Now}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation cancelled");
            }
        }
    }
}
