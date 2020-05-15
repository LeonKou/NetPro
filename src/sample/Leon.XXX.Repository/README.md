# Repository层


### 🕰️ 说明


#### 简要
Repository层主要做数据库相关，包含数据库表的定义与数据库操作的定义

#### 建议结构：

```
++Data
	DapperContext  //数据库上下文
++TableXXX
	IXXXRepository	//XXX主表数据操作，强制Repository结尾以实现自动依赖注入
	XXXRepository
	XXXTable		//表实体
```

#### 引用关系
依赖 NetPro.Dapper

#### 使用
```csharp
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
        ///  重写切换数据库逻辑
        /// </summary>
        /// <param name="serverId"></param>
        public override void SetMySqlConnectioin(int serverId)
        {
            //数据库从Apollo读取
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
