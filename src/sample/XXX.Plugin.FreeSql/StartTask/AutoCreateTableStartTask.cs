using System.Threading;

namespace XXX.Plugin.FreeSql
{
    /// <summary>
    /// 每天一次自动创建下个月表示例代码
    /// </summary>
    public class AutoCreateTableStartTask : IStartupTaskAsync
    {
        public AutoCreateTableStartTask(IConfiguration configuration)
        {
        }
        public int Order => 0;

        public async Task ExecuteAsync()
        {
            await Task.Yield();
            //间隔1天执行一次
            using var timer = new PeriodicTimer(TimeSpan.FromDays(1));
            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    Console.WriteLine($"【定时任务被触发】Tick {DateTime.Now}");
                    var asTableServie = EngineContext.Current.Resolve<IFreeSQLAsTableByDependency>();
                    var tableName = asTableServie.CreateTable(typeof(Log));
                    Console.WriteLine($"创建表:{tableName}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("【定时任务被取消】Operation cancelled");
            }
        }
    }

}
