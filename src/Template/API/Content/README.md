
# XXX 简介
XXX是一个Webapi服务,由netproapi脚手架生成，依赖于.NetCore6.0
...(待补充)
---

## 环境

### 开发

swagger地址：[swagger]("http://swagger")

cicd地址：[Jenkins]("http://Jenkins")

database：
- 地址：
- 账号：
- 密码：

### 测试

swagger地址：[swagger]("http://swagger")

cicd地址：[Jenkins]("http://Jenkins")

database：
- 地址：
- 账号：
- 密码：

### 生产

swagger地址：[swagger]("http://swagger")

cicd地址：[Jenkins]("http://Jenkins")

---

## 支持以下几种开发方式

### 1、简化版
 API主程序集下每个业务模块一个文件夹隔离，业务模块下包含此业务模块的Controller、Service、Entity等等

### 2、程序集隔离

每个业务模块一个程序集，每个程序集中包含当前业务模块需要的所有Controller、Service、Entity等等，再API主程序集引用需要的业务模块，业务模块之间尽可能不互相依赖，引用。

### 3、插件方式

在`2、程序集隔离`的基础上，API主程序集不直接引用需要的业务模块程序集，而是将各业务模块程序集放置在指定的插件文件夹中，插件路径配置如下：
```json
"TypeFinderOption": {
		"MountePath": ""//windows默认目录： C:/opt/netpro ; linux环境：/opt/netpro
	},
```

---

### 按需要可配置以下节点来实现相应需求
appsetting.json

```json
      "NetProOption": {
		//"UseResponseCompression": false, //是否启用响应压缩
		//"ThreadMinCount": 5, //最小线程数
		//"ApplicationName": "", //应用名称
		//"RequestWarningThreshold": 5, //请求时长的警告临界值
		"RoutePrefix": "api" //全局路由前缀
	},

```

