// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.Common;

namespace Maikebing.Data.Taos
{
    // TODO: Truncate to specified size
    // TODO: Infer type and size from value
    /// <summary>
    ///     Represents a parameter and its value in a <see cref="TaosCommand" />.
    /// </summary>
    /// <remarks>Due to Taos's dynamic type system, parameter values are not converted.</remarks>
    /// <seealso href="http://Taos.org/datatype3.html">Datatypes In Taos Version 3</seealso>
    public class TaosParameter : DbParameter
    {
        private string _parameterName = string.Empty;
        private object _value;
        private int? _size;
        private TaosType? _TaosType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosParameter" /> class.
        /// </summary>
        public TaosParameter()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosParameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter. Can be null.</param>
        public TaosParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _parameterName = name;
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosParameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        public TaosParameter(string name, TaosType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _parameterName = name;
            TaosType = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosParameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="size">The maximum size, in bytes, of the parameter.</param>
        public TaosParameter(string name, TaosType type, int size)
            : this(name, type)
            => Size = size;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosParameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="size">The maximum size, in bytes, of the parameter.</param>
        /// <param name="sourceColumn">The source column used for loading the value. Can be null.</param>
        public TaosParameter(string name, TaosType type, int size, string sourceColumn)
            : this(name, type, size)
            => SourceColumn = sourceColumn;

        /// <summary>
        ///     Gets or sets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        /// <remarks>Due to Taos's dynamic type system, parameter values are not converted.</remarks>
        /// <seealso href="http://Taos.org/datatype3.html">Datatypes In Taos Version 3</seealso>
        public override DbType DbType { get; set; } = DbType.String;

        /// <summary>
        ///     Gets or sets the Taos type of the parameter.
        /// </summary>
        /// <value>The Taos type of the parameter.</value>
        /// <remarks>Due to Taos's dynamic type system, parameter values are not converted.</remarks>
        /// <seealso href="http://Taos.org/datatype3.html">Datatypes In Taos Version 3</seealso>
        public virtual TaosType TaosType
        {
            get => _TaosType.GetValueOrDefault();//?? TaosValueBinder.GetTaosType(_value);
            set => _TaosType = value;
        }

        /// <summary>
        ///     Gets or sets the direction of the parameter. Only <see cref="ParameterDirection.Input" /> is supported.
        /// </summary>
        /// <value>The direction of the parameter.</value>
        public override ParameterDirection Direction
        {
            get => ParameterDirection.Input;
            set
            {
                if (value != ParameterDirection.Input)
                {
                    throw new ArgumentException($"InvalidParameterDirection{value}");
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the parameter is nullable.
        /// </summary>
        /// <value>A value indicating whether the parameter is nullable.</value>
        public override bool IsNullable { get; set; }

        /// <summary>
        ///     Gets or sets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public override string ParameterName
        {
            get => _parameterName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _parameterName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum size, in bytes, of the parameter.
        /// </summary>
        /// <value>The maximum size, in bytes, of the parameter.</value>
        public override int Size
        {
            get => _size
                ?? (_value is string stringValue
                    ? stringValue.Length
                    : _value is byte[] byteArray
                        ? byteArray.Length
                        : 0);

            set
            {
                if (value < -1)
                {
                    // NB: Message is provided by the framework
                    throw new ArgumentOutOfRangeException(nameof(value), value, message: null);
                }

                _size = value;
            }
        }

        /// <summary>
        ///     Gets or sets the source column used for loading the value.
        /// </summary>
        /// <value>The source column used for loading the value.</value>
        public override string SourceColumn { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the source column is nullable.
        /// </summary>
        /// <value>A value indicating whether the source column is nullable.</value>
        public override bool SourceColumnNullMapping { get; set; }

        /// <summary>
        ///     Gets or sets the value of the parameter.
        /// </summary>
        /// <value>The value of the parameter.</value>
        /// <remarks>Due to Taos's dynamic type system, parameter values are not converted.</remarks>
        /// <seealso href="http://Taos.org/datatype3.html">Datatypes In Taos Version 3</seealso>
        public override object Value
        {
            get => _value;
            set { _value = value; }
        }
#if NET45
        public override DataRowVersion SourceVersion { get; set; }
#endif
        /// <summary>
        ///     Resets the <see cref="DbType" /> property to its original value.
        /// </summary>
        public override void ResetDbType()
            => ResetTaosType();

        /// <summary>
        ///     Resets the <see cref="TaosType" /> property to its original value.
        /// </summary>
        public virtual void ResetTaosType()
        {
            DbType = DbType.String;
            TaosType = TaosType.Text;
        }

        
    }
}
