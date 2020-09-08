using NetPro.Core.Infrastructure;
using NetPro.Web.Core.Validators;
using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;
using NetPro.Web.Core.PagedList;

namespace NetPro.Web.Core.Models
{
    /// <summary>
    /// 分页查询请求基类
    /// </summary>
    public abstract class PaginationSearchModel : SortSearchModel
    {
        private int _pageSize;

        /// <summary>
        ///页码 从0开始
        /// </summary>
        //[SwaggerDefaultValue(0)]
        [Required]
        [Display(Order = 0)]
        public virtual int PageIndex { get; set; }
        /// <summary>
        /// 每页显示数量.小于0则返回所有
        /// </summary>
        //[SwaggerDefaultValue(20)]
        [Required]
        [Display(Order = 0)]
        public virtual int PageSize
        {

            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value <= 0 ? int.MaxValue : value;
            }
        }

        /// <summary>
        /// 根据页数与每页大小求出跳过个数
        /// </summary>
        public int Skip
        {
            get { return this.PageSize * Math.Max((this.PageIndex - 1), 0); }
        }

        /// <summary>
        /// 生成分页返回对象
        /// </summary>
        /// <typeparam name="TItems"></typeparam>
        /// <param name="totalCount"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public IPagedList<TItems> ToPagedList<TItems>(int totalCount, System.Collections.Generic.IList<TItems> items)
        {
            return new PagedList<TItems>()
            {
                Items = items,
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                TotalCount = totalCount,
                TotalPages = this.PageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)this.PageSize)
            };
        }

    }

    /// <summary>
    /// 排序查询请求
    /// </summary>
    public abstract class SortSearchModel
    {
        /// <summary>
        /// 排序方式 asc 升序，desc降序
        /// </summary>
        //[SwaggerDefaultValue("desc")]
        [Required]
        [Display(Order = -1)]
        public virtual string OrderByType
        {
            get
            {
                return _sort;
            }
            set
            {
                if (value.Trim().Equals("asc", StringComparison.CurrentCultureIgnoreCase)) _sort = value;
                else _sort = "desc";
            }
        }

        private string _sort;
        /// <summary>
        /// 排序字段
        /// </summary>
        //[SwaggerDefaultValue("id")]
        [Required]
        [Display(Order = -2)]
        public virtual string OrderByField { get; set; }

        /// <summary>
        /// 排序对象组合
        /// </summary>
        /// <returns></returns>
        public Sort ToSort()
        {
            return new Sort()
            {
                Dir = OrderByType,
                Field = OrderByField
            };
        }

        public virtual bool HasSort => !string.IsNullOrWhiteSpace(this.OrderByField);
    }

    public class PaginationSearchModelValidator : BaseValidator<PaginationSearchModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public PaginationSearchModelValidator()
        {
            RuleFor(t => t.PageIndex).GreaterThanOrEqualTo(0).WithMessage("PageIndex必须大于等于0的整数");
            RuleFor(t => t.PageSize).GreaterThan(0).WithMessage("PageSize必须为大于0");
        }
    }
}
