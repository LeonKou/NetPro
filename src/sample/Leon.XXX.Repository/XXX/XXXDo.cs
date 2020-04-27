using NetPro.Dapper.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leon.XXX.Repository
{
	[Table("rank")]
	public class XXXDo
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }
		[Column("type")]
		public int Type { get; set; }
	}
}
