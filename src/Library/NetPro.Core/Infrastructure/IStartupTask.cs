using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Core.Infrastructure
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
}
