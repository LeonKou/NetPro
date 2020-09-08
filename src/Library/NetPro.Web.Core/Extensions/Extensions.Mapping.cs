using NetPro.Core.Infrastructure.Mapper;
using NetPro.Utility;
using NetPro.Web.Core.Models;
using NetPro.Web.Core.PagedList;
using System;
using System.Collections.Generic;

namespace NetPro.Web.Core
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
        public static D ToModel<S,D>(this S entity)
        {
            if (entity == null)
                return default(D);

            return AutoMapperConfiguration.Mapper.Map<D>(entity);
        }

        /// <summary>
        /// 集合列表类型映射 Domain To ViewModel
        /// </summary>
        public static List<D> ToModels<S,D>(this IEnumerable<S> entities)
        {
            if (entities == null) return null;
            try
            {
                return AutoMapperConfiguration.Mapper.Map<List<D>>(entities);
            }
            catch (Exception ex)
            {
                throw new NetProException("数据Model转换错误", ex);
            }
            
        }

        #endregion

        #region toEntity

        /// <summary>
        ///  类型映射 viewmodel to Domain
        /// </summary>
        public static T ToDomain<T>(this BaseDto entity)
        {
            if (entity == null)
                return default(T);

            return AutoMapperConfiguration.Mapper.Map<T>(entity);
        }

        /// <summary>
        /// model => Domains
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> ToDomains<T>(this IEnumerable<BaseDto> models)
        {
            return AutoMapperConfiguration.Mapper.Map<ICollection<T>>(models);
        }

        #endregion

        /// <summary>
        /// ipagedlist 转pagedlistmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PagedListModel<T> ToModel<T>(this IPagedList<T> data) where T : class
        {
            if (data == null)
                throw new NetProException("对象不能为空");

            return new PagedListModel<T>(data.Items, data.PageIndex, data.PageSize, data.TotalCount, data.TotalPages);
        }
        /// <summary>
        /// pagedlist 转PagedListModel
        /// </summary>
        /// <typeparam name="S">Domain对象</typeparam>
        /// <typeparam name="D">api返回对象</typeparam>
        /// <param name="data">wcf服务返回数据</param>
        /// <returns>api需要的对象</returns>
        public static PagedListModel<D> ToModel<S, D>(this IPagedList<S> data) 
        {
            if (data == null)
                throw new NetProException("对象不能为空");
            
            return new PagedListModel<D>(data.Items.ToModels<S,D>(), data.PageIndex, data.PageSize, data.TotalCount, data.TotalPages);
        }
    }
}