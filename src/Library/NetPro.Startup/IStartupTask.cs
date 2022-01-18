using System.Threading.Tasks;

namespace System.NetPro
{
    /// <summary>
    /// Interface which should be implemented by tasks run on startup
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        void Execute();

        /// <summary>
        /// Gets order of this startup task implementation
        /// </summary>
        int Order { get; }
    }

    /// <summary>
    /// Interface which should be implemented by tasks run on startup(Async)
    /// </summary>
    public interface IStartupTaskAsync
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Gets order of this startup task implementation
        /// </summary>
        int Order { get; }
    }
}
