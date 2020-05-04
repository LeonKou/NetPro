using NetPro.Utility.Helpers;
using System;
using System.Collections.Generic;

namespace NetPro.Core.Configuration
{
	/// <summary>
	/// 关系数据库连接串
	/// </summary>
	public class ConnectionConfig
	{
		/// <summary>
		/// 数据库默认连接字符串
		/// </summary>
		public string DefaultConnection { get; set; }

		/// <summary>
		/// 游戏世界连接串
		/// </summary>
		public Dictionary<string, string> ServerIdConnection { get; set; }

		/// <summary>
		/// 解密后的连接字符串
		/// </summary>
		/// <returns></returns>
		public string DecryptDefaultConnection
		{
			get
			{
				try
				{
					if (!string.IsNullOrEmpty(DefaultConnection))
					{
						return EncryptHelper.DesDecrypt8(DefaultConnection, "20208956");
					}
					return string.Empty;
				}
				catch (Exception)
				{
					return DefaultConnection;
				}
			}
		}
	}
}
