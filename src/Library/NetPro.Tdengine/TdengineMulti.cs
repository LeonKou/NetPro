
using Maikebing.Data.Taos;
using NetPro.Tdengine;
using System;
using System.Linq;
using System.NetPro;

/// <summary>
/// 
/// </summary>
public class TdengineMulti
{
    internal static TdengineOption? TdengineOption;
    private TdengineMulti()
    {
    }

    /// <summary>
    /// 创建实例
    /// </summary>
    internal static TdengineMulti Instance
    {
        get
        {
            return new TdengineMulti();
        }
    }

    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TaosConnection Get(string key)
    {
        //find tdengine connectionString by key
        return CreateInstanceByKey(key);
    }

    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TaosConnection this[string key]
    {
        get
        {
            return CreateInstanceByKey(key);
        }
    }

    /// <summary>
    /// find tdengine connectionString by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static TaosConnection CreateInstanceByKey(string key)
    {
        if (TdengineOption == null)
        {
            TdengineOption = EngineContext.Current.Resolve<TdengineOption>();
        }

        var value = TdengineOption.ConnectionString.Where(s => s.Key == key).FirstOrDefault()?.Value;

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Could not find tdengine connection string for key = {key}");
        }
        return new TaosConnection(value);
    }
}