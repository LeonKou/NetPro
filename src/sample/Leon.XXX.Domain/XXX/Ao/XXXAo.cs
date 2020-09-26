using FluentValidation;
using NetPro.Web.Core.Validators;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class XXXAo
    {
        /// <summary>
        /// 这是Id
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// 这是Type
        /// </summary>
        public ulong CreateTime { get; set; }

        /// <summary>
        ///  用户Id
        /// </summary>
        public uint UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 模型验证
        /// </summary>
        public class XXXAoValidator : BaseValidator<XXXAo>
        {
            /// <summary>
            /// 模型验证
            /// </summary>
            public XXXAoValidator(IDataBaseOptionService dataBaseOptionService)
            {
                //Id必须
                RuleFor(x => x.Id).GreaterThan((uint)1).WithMessage("Id不能小于1")
                    .MustAsync(async (s, a, x) =>
                    {
                        bool IssuerValidate = false;
                        if ((await dataBaseOptionService.FindAsync(s.Id)).Result != null)
                        {
                            IssuerValidate = true;
                        }
                        return IssuerValidate;
                    }).WithMessage("数据库未找到对应数据数据不存在");

                RuleFor(t => t.UserName).NotEmpty().WithMessage("名称不能为空")
                    .Length(1, 20).WithMessage("名称长度在1-20个字符之间");
            }
        }
    }
}
