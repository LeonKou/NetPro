using FluentValidation;

namespace NetPro.Web.Core.Validators
{
    /// <summary>
    /// 实体验证基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected BaseValidator()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {

        }
    }
}
