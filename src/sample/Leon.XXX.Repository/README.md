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

