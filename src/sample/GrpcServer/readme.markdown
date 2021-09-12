
## Grpc示例

Grpc使用示例

### 使用

- 创建Grpc项目，并引用`NetPro.Grpc`.nuget
默认只有Server端，手动增加Client端方便快速demo
 csproj文件中会生成

``` xml

  <ItemGroup>
  <Protobuf Include="Protos/greet.proto" GrpcServices="Server" />
  </ItemGroup>

  <ItemGroup>
  <Protobuf Include="Protos/greet.proto" GrpcServices="Clinet" />
  </ItemGroup>

  <ItemGroup>
  <PackageReference Include="Grpc.AspNetCore" Version="2.27.0" />
  <PackageReference Include="Grpc.AspNetCore.Server.Reflection" Version="2.37.0" />
  </ItemGroup>
  
  <ItemGroup>
  <ProjectReference Include="NetPro.Grpc" Version="3.27.0" />
  </ItemGroup>

```

- 修改工程文件
为了proto协议文件可以集中一处，增加Link属性

``` xml
<Protobuf Include="/src/Protos/greet.proto" GrpcServices="Server" Link="Protos\%(RecursiveDir)%(Filename)%(Extension)"/>
  </ItemGroup>

  <ItemGroup>
  <Protobuf Include="/src/Protos/greet.proto" GrpcServices="Clinet" Link="Protos\%(RecursiveDir)%(Filename)%(Extension)"/>
  </ItemGroup> 
  <!--以上会生成服务端与客户端的grpc代理代码，并且将src路径下的proto移动到项目下的Protos目录下-->

```

### proto定义

``` proto
syntax = "proto3";

option csharp_namespace = "GrpcServer";
//import "google/api/annotations.proto";

package greet;

service Greeter {

	//简单rpc
  rpc SayHello (HelloRequest) returns (HelloReply);

  //定义双向流
  rpc SubSayHello (stream BathTheCatReq) returns (stream BathTheCatResp);
}

message BathTheCatReq{
    int32 id=1;
}

message BathTheCatResp{
	string message=1;
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}


```

### 双向流
此处双向流都要客户端发起

```csharp
  public override async Task BathTheCat(IAsyncStreamReader<BathTheCatReq> requestStream, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            var bathQueue = new Queue<int>();
            while (await requestStream.MoveNext())
            {
              //通过requestStream.Current取请求参数
                bathQueue.Enqueue(requestStream.Current.Id);

                _logger.LogInformation($"Cat {requestStream.Current.Id} Enqueue.");
            }

            //遍历队列开始洗澡
            while (bathQueue.TryDequeue(out var catId))
            {
                await responseStream.WriteAsync(new BathTheCatResp() { Message = $"铲屎的成功给一只{Cats[catId]}洗了澡！" });

                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }
            return null;
        }
```
