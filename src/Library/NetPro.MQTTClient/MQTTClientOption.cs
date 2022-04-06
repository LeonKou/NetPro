/*
summary: 
//https://www.cnblogs.com/znlgis/p/4930990.html
//https://github.com/SeppPenner/NetCoreMQTTExampleCluster/tree/master/src/NetCoreMQTTExampleCluster.Cluster
共享订阅由三部分组成：

静态共享标识符 （$queue 与 $share）
组标识符（可选）
实际接收消息的主题
$queue和$share的差异：

$queue 之后的主题中所有消息将轮流发送到客户端，
$queue/topic

$share 之后，您可以添加不同的组，例如:

$share/group_1/topic
$share/group_2/topic
$share/group_3/topic

主题格式：
topic通过'/'分割层级，
订阅者支持'+','#'通配符

'+':通配一个层级，例如a/+,匹配 a/x,a/y
'#':统配多个层级，例如a/#,匹配a/x,a/b/c/d

消息质量Qos：
常用0和1,level2非必须情况请勿使用
*/
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.MQTTClient
{
    /// <summary>
    /// MQTT配置
    /// </summary>
    public class MQTTClientOption
    {
        /// <summary>
        /// 
        /// </summary>
        public MQTTClientOption()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public MQTTClientOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(MQTTClientOption)).Bind(this);

        }

        /// <summary>
        /// 连接串集合
        /// </summary>
        public List<ConnectionString> ConnectionString { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ConnectionString
    {
        /// <summary>
        /// 连接串别名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 连接串
        /// 格式：
        /// clientid=netpro;host=127.0.0.1;port=6379;username=netpro;password=netpro;
        /// clientid:客户端id唯一标识
        /// host:broker主机地址
        /// port:broker主机端口
        /// username:账户名
        /// password:密码
        /// </summary>
        public string Value { get; set; }
    }
}
