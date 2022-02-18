
## NetPro.Taos使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Taos.svg)](https://nuget.org/packages/NetPro.Taos)

对[涛思数据库](https://www.taosdata.com/cn/)连接对象[Maikebing.Data.Taos](https://www.nuget.org/packages/Maikebing.Data.Taos/)的简易封装，使用单例TaosConnection对象


已过时，不推荐使用，请转向[NetPro.Tdengine](https://github.com/LeonKou/NetPro/tree/dev_6.0/src/Library/NetPro.Tdengine)组件

### 使用

#### appsetting.json 

```json
"TaosOption": {
    "Idle": 120,//空闲时间，单位秒
    "ConnectionString": [
      {
        "Key": "taos1", //连接串key别名，唯一
        "Value": "Data Source=taos;DataBase=db_20220120141621;Username=root;Password=taosdata;Port=6030" //别名key对应的连接串
      }
    ]
  },

```
#### 启用服务
没有基于NetPro.Web.Api 的使用场景，必须手动进行初始化，如下：
```csharp
IConfiguration Configuration;

public void ConfigureServices(IServiceCollection services)
{
     services.AddTaos(new TaosOption(configuration));
}
```

基于NetPro.Web.Api的使用，只需要添加引用后配置以上appsetting.josn配置TaosOption节点即可

### 使用说明
```csharp
 public class TaosService: ITaosService
    {
        private readonly IdleBus<TaosConnection> _taosdbMulti;
        public TaosService(IdleBus<TaosConnection> taosdbMulti)
        {
            _taosdbMulti = taosdbMulti;
        }

        /// <summary>
        /// 执行Sql
        /// </summary>
        public void Executesql(string sql)
        {
            var taos= _taosdbMulti.Get("taos1");
            var command= taos.CreateCommand(@"INSERT INTO  data_history_67 
                                 USING datas TAGS (mongo, 67) 
                                 values ( 1608173534840 2 false 'Channel1.窑.烟囱温度' '烟囱温度' '122.00' );");
            command.ExecuteReader();
        }
    }
```

#### 更新中...

