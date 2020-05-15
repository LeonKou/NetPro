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

