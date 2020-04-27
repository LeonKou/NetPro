using System;
using NetPro.Core.Infrastructure;
using FluentValidation;
using FluentValidation.Attributes;


namespace NetPro.Web.Core.FluentValidation
{
    /// <summary>
    /// 自定义FluentValidation 工厂
    /// </summary>
    public class NetProValidatorFactory : AttributedValidatorFactory
    {
        /// <summary>
        /// Gets a validator for the appropriate type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Created IValidator instance; null if a validator cannot be created</returns>
        public override IValidator GetValidator(Type type)
        {
            if (type == null)
                return null;

            //get a custom attribute applied to a member of a type
            var validatorAttribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
            if (validatorAttribute == null || validatorAttribute.ValidatorType == null)
                return null;

            //try to create instance of the validator
            var instance = EngineContext.Current.ResolveUnregistered(validatorAttribute.ValidatorType);

            return instance as IValidator;
        }
    }
}