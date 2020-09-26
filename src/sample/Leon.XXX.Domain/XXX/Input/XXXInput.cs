using FluentValidation;
using Microsoft.AspNetCore.Http;
using NetPro.Web.Core.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 这是Input
    /// </summary>
    public class XXXInput
    {
        /// <summary>
        /// 年龄
        /// </summary>
        [Range(6, 20)]
        public int Age { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 这是	FileTestInput
    /// </summary>
    public class FileTestInput
    {
        /// <summary>
        /// 名称
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
            public XXXValidator(IDataBaseOptionService xXXService)
            {
                RuleFor(t => t.Name).NotEmpty().WithMessage("名称不能为空")
                    .Length(1, 20).WithMessage("名称长度在1-20个字符之间");
            }
        }
    }
}
