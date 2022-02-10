/// <summary>
/// Mapper profile registrar interface
/// </summary>
public interface IOrderedMapperProfile
{
    /// <summary>
    /// 依赖注册执行顺序
    /// </summary>
    int Order { get; }
}