# Domain层


### 🕰️ 说明


#### 简要
Domain层主要做从领域业务实现

#### 建议结构：

```
++XXX
	++Ao  //Ao实体
++Mapper
	XXXMapper.cs
++Request
	XXXRequest.cs	//请求类建议Request结尾
++Service
	XXXService.cs	//业务Service强制Service结尾以实现自动实现依赖注入
	IXXXService.cs	
```

#### 引用关系
依赖 XXX.Repository层

#### 使用

Service:
```csharp
/// <summary>
	/// 
	/// </summary>
	public class XXXService : IXXXService
	{
		private readonly IXXXRepository _rankRepository;
		private readonly IMapper _mapper;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rankRepository"></param>
		/// <param name="mapper"></param>
		public XXXService(IXXXRepository rankRepository
			, IMapper mapper)//构造函数注入；IXXXRepository由于Repository结尾故自动依赖注入，此处可直接构造函数注入
		{
			_rankRepository = rankRepository;
			_mapper = mapper;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public XXXAo GetList()
		{
			var xxxdo = _rankRepository.GetList<XXXDo>("", new DynamicParameters());
			return _mapper.Map<XXXAo>(xxxdo.First());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool GetFalse(string name)
		{
			return true;
		}
	}
```
请求参数验证：

```csharp
/// <summary>
	/// 这是Input
	/// </summary>
	public class XXXRequest
	{
		/// <summary>
		/// 这是名称
		/// </summary>
		[SwaggerDefaultValue(3)] //swagger默认值
		[Required(ErrorMessage = "必填项")]
		[Range(0, 100)]
		public int Age { get; set; }
	}

	/// <summary>
	/// 这是FileTestInput
	/// </summary>
	[Validator(typeof(XXXValidator))]
	public class FileTestInput
	{
		/// <summary>
		/// 这是名称注释
		/// </summary>
		[StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
		public string Name { get; set; }

		/// <summary>
		/// 单文件
		/// </summary>
		public IFormFile File { get; set; }

		/// <summary>
		/// 验证
		///将此验证类用attribute[]放于待验证实体上方即可
		/// </summary>
		public class XXXValidator : BaseValidator<FileTestInput>
		{
			/// <summary>
			/// 验证
			/// </summary>
			public XXXValidator(IXXXService xXXService)
			{
				RuleFor(x => x.Name).Must(s => xXXService.GetFalse(s))
				.WithMessage("这是false");   //调用service进行验证
				RuleFor(t => t.Name).NotEmpty().WithMessage("名称不能为空").Length(1, 20).WithMessage("名称长度在1-20个字符之间");  //简单参数验证
			}
		}
```

