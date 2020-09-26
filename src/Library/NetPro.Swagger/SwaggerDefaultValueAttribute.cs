using System;

namespace NetPro.Swagger
{
    /// <summary>
    /// Swagger默认值设置
    /// </summary>
    public class SwaggerDefaultValueAttribute : Attribute
    {
        public object Value { get; set; }

        public SwaggerDefaultValueAttribute(object value)
        {
            this.Value = value;
        }
    }
}
