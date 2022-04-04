## NetPro.Tdengine使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Tdengine.svg)](https://nuget.org/packages/NetPro.Tdengine)

对[涛思数据库](https://www.taosdata.com/cn/)连接对象[Maikebing.Data.Taos](https://www.nuget.org/packages/Maikebing.Data.Taos/)的简易封装，使用单例TaosConnection对象

> [客户端驱动下载地址](https://www.taosdata.com/all-downloads#tdengine_win-list) 客户端与服务器版本必须强一致；

> 即使使用ip连接，也必须配置fqdn:
- 服务器执行以下命令获取服务器的fqdn值

```
taosd | grep -i fqdn
```
- 修改hosts域名解析 添加服务器ip对应的fqdn值作为域名
- 修改C:\TDengine\cfg 配置

```yaml
# local fully qualified domain name (FQDN)
fqdn                      h26.taosdata.com  #taos数据库远程hostname，既fqdn

# first port number for the connection (12 continuous UDP/TCP port number are used) 
serverPort                6030 #客户端连接端口，默认6030，udp
```
> windows本地客户端测试

```
.\taos.exe -h h26.taosdata.com -P 6030
```
响应以下即成功，显示客户端版本为2.4.0.7,服务器必定也是2.4.0.7版本
```
Welcome to the TDengine shell from Windows, Client Version:2.4.0.7
Copyright (c) 2020 by TAOS Data, Inc. All rights reserved.
```
> Tdengine时区 修改

修改C:\TDengine\cfg下的taos.cfg配置文件中的timezone节点与服务器保持一致；
特别注意Tdengine执行的时区标准为 unix的时间标准，与传统的东8 是+8不一样，在Tdengine需配置为UTC-8

```
# system time zone
timezone              UTC+0  #0时区(伦敦时间)

#timezone              UTC-8  #东八区(北京时间)
```
> restful 方式访问Tdengine需注意

[官方restful调用文档](https://www.taosdata.com/docs/cn/v2.0/connector#restful)

调用 rest/sql ，响应时间以服务器时区为准，并且格式为 `2022-02-17 12:31:45.375`
调用 rest/sqlt，响应时间为unix时间戳

### 使用

#### appsetting.json 

```json
"TdengineOption": {
    "ConnectionString": [
      {
        "Key": "taos1", //连接串key别名，唯一
        "Value": "Data Source=h26.taosdata.com;DataBase=db_20220120141621;Username=root;Password=taosdata;Port=6030" //别名key对应的连接串
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
     services.AddTdengine(Configuration);
}
```

基于NetPro.Web.Api的使用，只需要添加引用后配置以上appsetting.josn配置TaosOption节点即可

> 当想自定义连接字符串获取方式时，无论是否基于NetPro.Web.Api, 都能通过传入委托来自定义连接字符串获取方式：

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddTdengineDb(GetConnectionString);
}

public List<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
{
    return new List<ConnectionString>
    {
        new ConnectionString()
        {
            Key ="remotekey",
            Value = "Data Source=h26.taosdata.com;DataBase=db_netpro;Username=root;Password=taosdata;Port=6030"
        }
    };
}
```


### 使用说明
```csharp
 public class TaosService: ITaosService
    {
        private readonly TdengineMulti _taosdbMulti;
        public TaosService(TdengineMulti taosdbMulti)
        {
            _taosdbMulti = taosdbMulti;
        }

        /// <summary>
        /// 执行Sql
        /// </summary>
        public void Executesql(string sql)
        {
            using  var taos= _taosdbMulti.Get("taos1");
            using var command= taos.CreateCommand(@"INSERT INTO  data_history_67 
                                 USING datas TAGS (mongo, 67) 
                                 values ( 1608173534840 2 false 'Channel1.窑.烟囱温度' '烟囱温度' '122.00' );");
            using var reader= command.ExecuteReader();//command和reader都必须using
        }
    }
```

#### 更新中...

