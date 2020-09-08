
## Dapper使用（废弃，建议使用Freesql）


对Dapper的封装，简化操作与映射

### 使用
#### 定义数据库上下文
```csharp
 /// <summary>
    /// Admin数据库
    /// </summary>
    public class DefaultDapperContext : DapperContext
    {
        public DefaultDapperContext(string connectionStringName, DataProvider dataProvider) : base(connectionStringName, dataProvider)
        {

        }
    }
       //继续追加其他数据库上下文
```

#### 添加dapper中间件
```csharp
 public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null)
  {
      services = services.AddDapperRepository();
      services.AddScoped<IXXXRepository,XXXRepository>();//有定义Repository便添加数据表Repository，没有使用可不添加
      ...//建议统一命名批量自动注入
      
  }
```
#### 定义数据库映射的实体

```csharp
[Table("rank")]
 public class XXXDo
 {
 	[Key]
 	[Column("id")]
 	public int Id { get; set; }
 	[Column("type")]
 	public int Type { get; set; }
 }

 注意：Table所在命名空间为NetPro.Dapper
```
#### 定义Repository(可选择使用)
业务简单可不定义Repository，直接使用IDapperRepository<T> 接口直接操作数据库
``` csharp

/// <summary>
    /// 
    /// </summary>
    public class XXXRepository : GeneralRepository<DefaultDapperContext, XXXDo>, IXXXRepository
    {
        private readonly IDapperRepository<DefaultDapperContext> _dapperRepository;
        private readonly IUnitOfWorkFactory<DefaultDapperContext> _unitOfWorkFactoryNew;
        public XXXRepository(IDapperRepository<DefaultDapperContext> dapperRepository,
            IUnitOfWorkFactory<DefaultDapperContext> _unitOfWorkFactoryNew) : base(dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        /// <summary>
        /// 插入(带事务)
        /// </summary>
        public void Insert()
        {
            SetMySqlConnectioin(2);
            var unit = _unitOfWorkFactoryNew.Create();

            Insert(new XXXDo());
            unit.SaveChanges();
        }

        /// <summary>
        ///  自定义切换数据库逻辑
        /// </summary>
        /// <param name="serverId"></param>
        public override void SetMySqlConnectioin(int serverId)
        {
            var context = EngineContext.Current.Resolve<DefaultDapperContext>();
            if (serverId == 1)
            {
                context.SetTempConnection("Server=");
                return;
            }
            if (serverId == 2)
            {
                context.SetTempConnection("Server=");
                return;
            }
        }
    }
```
#### 构造函数注入之前定义好的Repository
```csharp
 public class XXXService : IXXXService
    {
        private readonly IXXXRepository _rankRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rankRepository"></param>
        /// <param name="mapper"></param>
        public XXXService(IXXXRepository rankRepository)
        {
            _rankRepository = rankRepository;
        }
        public XXXAo GetList()
        {
           return _rankRepository.QueryList<XXXDo>("", new DynamicParameters());
        }
    }
```

