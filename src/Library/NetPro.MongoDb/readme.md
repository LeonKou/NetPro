
## Mongodb使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.MongoDb.svg)](https://nuget.org/packages/NetPro.MongoDb)

对Mongodb的封装，简化使用，支持多库

### 使用

#### appsetting.json 

```json
"MongoDbOption": {
    "ConnectionString": [
      {
        "Key": "mongo1", //连接串key别名，唯一
        "Value": "mongodb://192.168.100.187:27017/netprodemo" //别名key对应的连接串
      },
      {
        "Key": "mongo2", //连接串key别名，唯一
        "Value": "mongodb://192.168.100.187:27017/netprodemo2" //别名key对应的连接串
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
      //MongoDb 连接配置文件
       services.AddMongoDb(Configuration);
}
```

基于NetPro.Web.Api的使用，只需要添加引用后配置以上appsetting.josn配置MongoDbOption节点即可

> 当想自定义连接字符串获取方式时，无论是否基于NetPro.Web.Api, 都能通过传入委托来自定义连接字符串获取方式：

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMongoDb(GetConnectionString);
}

public IList<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
{
    var connector = new List<ConnectionString>();
    connector.Add(new ConnectionString { Key = "2", Value = "mongodb://192.168.100.187:27017/netprodemo2" });
    return connector;
}
```


### 使用说明
 entity 实体示例
```chsarp
 [CollectionName("hu")]
    public class Product : Document
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public int Price { get; set; }
    }
```
```csharp
 public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDBMulti _mongoDBMulti;
        public MongoDBService(IMongoDBMulti mongoDBMulti)
        {
            _mongoDBMulti = mongoDBMulti;
        }

        /// <summary>
        /// 查询
        /// </summary>
        public Product Find(string key = "mongo1")
        {
            var mongodb = _mongoDBMulti[key];
            var filter = Builders<BsonDocument>.Filter;
            var product = mongodb.GetCollection<Product>().Find(s => s.Category == "").FirstOrDefault();

            return product;
        }
    }
```

#### 更新中...

