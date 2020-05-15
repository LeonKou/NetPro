using WebApiClient;
using WebApiClient.Attributes;

namespace Leon.XXX.Proxy
{
    public interface IExampleProxy : IHttpApi
    {
        [HttpGet("")]
        ITask<dynamic> GetAsync(string account);

        [HttpPost("api/v1/NetProgoods/list")]
        ITask<dynamic> GetGoodsList(int gameId, string gameVersion);

        // POST api/user 
        [HttpPost("api/user")]
        ITask<dynamic> AddAsync([FormContent] dynamic user);
    }
}
