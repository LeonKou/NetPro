using FluentValidation;
using FluentValidation.Attributes;
using NetPro.Web.Core;
using NetPro.Web.Core.Validators;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 这是Input
    /// </summary>
    public class XXXRequest
    {
        /// <summary>
        /// 这是名称
        /// </summary>
        //[SwaggerDefaultValue(3)]
        [Range(6,20)]
        //[RegularExpression("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z\\d]{4,20}$")]
        public int Age { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// 这是	FileTestInput
    /// </summary>
    [Validator(typeof(XXXValidator))]
    public class FileTestInput
    {
        /// <summary>
        /// 这是名称注释
        /// </summary>
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Name { get; set; }

        /// <summary>
        /// 单文件
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        public class XXXValidator : BaseValidator<FileTestInput>
        {
            /// <summary>
            /// 验证
            /// </summary>
            public XXXValidator(IXXXService xXXService)
            {
                RuleFor(x => x.Name).Must(s => xXXService.GetFalse(s))
                .WithMessage("这是false");
                RuleFor(t => t.Name).NotEmpty().WithMessage("名称不能为空").Length(1, 20).WithMessage("名称长度在1-20个字符之间");
            }
        }
    }
}
