using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Yzc.Model;

namespace YZC.XXXService.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		private readonly IXXXService _IXXXService;
		public ValuesController(IXXXService IXXXService)
		{
			_IXXXService = IXXXService;
		}

		/// <summary>
		/// 这是我
		/// </summary>
		/// <param name="model">666666666</param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get(Sample model)
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
