using NetPro.AutoMapper;
using System.Collections.Generic;

namespace System.NetPro
{
    /// <summary>
    /// 对象转换 
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }
        #region ToModel
        /// <summary>
        ///  类型映射 Domain To ViewModel
        /// </summary>
        public static D ToModel<S, D>(this S entity)
        {
            if (entity == null)
                return default(D);

            return AutoMapperConfiguration.Mapper.Map<D>(entity);
        }

        /// <summary>
        /// 集合列表类型映射 Domain To ViewModel
        /// </summary>
        public static List<D> ToModels<S, D>(this IEnumerable<S> entities)
        {
            if (entities == null) return null;
            try
            {
                return AutoMapperConfiguration.Mapper.Map<List<D>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("数据Model转换错误", ex);
            }

        }

        #endregion
    }
}