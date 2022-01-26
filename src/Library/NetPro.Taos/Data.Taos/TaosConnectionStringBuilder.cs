// Copyright (c)  maikebing All rights reserved.
// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Provides a simple way to create and manage the contents of connection strings used by
    ///     <see cref="TaosConnection" />.
    /// </summary>
    public class TaosConnectionStringBuilder : DbConnectionStringBuilder
    {
        private const string DataSourceKeyword = "Data Source";
        private const string UserNameKeyword = "Username";
        private const string PasswordKeyword = "Password";
        private const string CharsetKeyword = "Charset";
        private const string DataSourceNoSpaceKeyword = "DataSource";
        private const string DataBaseKeyword = "DataBase";
        private const string PortKeyword = "Port";

        private enum Keywords
        {
            DataSource,
            DataBase,
            Username,
            Password,
            Port,
            Charset

        }

        private static readonly IReadOnlyList<string> _validKeywords;
        private static readonly IReadOnlyDictionary<string, Keywords> _keywords;

        private string _dataSource = string.Empty;
        private string _dataBase=string.Empty;
        private string _userName = string.Empty;
        private string _charset = System.Text.Encoding.UTF8.EncodingName;
        private string _password = string.Empty;
        private int  _port =6030;
        static TaosConnectionStringBuilder()
        {
            var validKeywords = new string[6];
            validKeywords[(int)Keywords.DataSource] = DataSourceKeyword;
            validKeywords[(int)Keywords.DataBase] = DataBaseKeyword;
            validKeywords[(int)Keywords.Username] = UserNameKeyword;
            validKeywords[(int)Keywords.Password] = PasswordKeyword;
            validKeywords[(int)Keywords.Charset] =CharsetKeyword;
            validKeywords[(int)Keywords.Port] = PortKeyword;
            _validKeywords = validKeywords;

            _keywords = new Dictionary<string, Keywords>(6, StringComparer.OrdinalIgnoreCase)
            {
                [DataSourceKeyword] = Keywords.DataSource,
                [UserNameKeyword] = Keywords.Username,
                [PasswordKeyword] = Keywords.Password,
                [DataBaseKeyword] = Keywords.DataBase,
                [CharsetKeyword] = Keywords.Charset,
                [DataSourceNoSpaceKeyword] = Keywords.DataSource,
                [PortKeyword] = Keywords.Port
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosConnectionStringBuilder" /> class.
        /// </summary>
        public TaosConnectionStringBuilder()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosConnectionStringBuilder" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The initial connection string the builder will represent. Can be null.
        /// </param>
        public TaosConnectionStringBuilder(string connectionString)
            => ConnectionString = connectionString;

        /// <summary>
        ///     Gets or sets the database file.
        /// </summary>
        /// <value>The database file.</value>
        public virtual string DataSource
        {
            get => _dataSource;
            set => base[DataSourceKeyword] = _dataSource = value;
        }
        public virtual string Username
        {
            get => _userName;
            set => base[UserNameKeyword] = _userName = value;
        }
        /// <summary>
        /// Charset
        /// </summary>
        public virtual string Charset
        {
            get => _charset;
            set => base[CharsetKeyword] = _charset = value;
        }
        public virtual string Password
        {
            get => _password;
            set => base[PasswordKeyword] = _password = value;
        }
        public virtual int  Port
        {
            get => _port;
            set => base[PortKeyword] = _port = value;
        }



        /// <summary>
        ///     Gets a collection containing the keys used by the connection string.
        /// </summary>
        /// <value>A collection containing the keys used by the connection string.</value>
        public override ICollection Keys
            => new ReadOnlyCollection<string>((string[])_validKeywords);

        /// <summary>
        ///     Gets a collection containing the values used by the connection string.
        /// </summary>
        /// <value>A collection containing the values used by the connection string.</value>
        public override ICollection Values
        {
            get
            {
                var values = new object[_validKeywords.Count];
                for (var i = 0; i < _validKeywords.Count; i++)
                {
                    values[i] = GetAt((Keywords)i);
                }

                return new ReadOnlyCollection<object>(values);
            }
        }

      
        public virtual string DataBase
        {
            get => _dataBase;
            set => base[DataBaseKeyword] = _dataBase = value;
        }
        internal bool ForceDatabaseName { get; set; } = false;



        /// <summary>
        ///     Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="keyword">The key.</param>
        /// <returns>The value.</returns>
        public override object this[string keyword]
        {
            get => GetAt(GetIndex(keyword));
            set
            {
                if (value == null)
                {
                    Remove(keyword);

                    return;
                }

                switch (GetIndex(keyword))
                {
                    case Keywords.DataSource:
                        DataSource = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Username:
                        Username= Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Password:
                        Password = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.DataBase:
                        DataBase = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Port:
                        Port = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                        return;
                    case Keywords.Charset:
                        Charset = Convert.ToString(value, CultureInfo.InvariantCulture);
                        return;
                    default:
                        Debug.Assert(false, "Unexpected keyword: " + keyword);
                        return;
                }
            }
        }

        private static TEnum ConvertToEnum<TEnum>(object value)
            where TEnum : struct
        {
            if (value is string stringValue)
            {
                return (TEnum)Enum.Parse(typeof(TEnum), stringValue, ignoreCase: true);
            }

            if (value is TEnum enumValue)
            {
                enumValue = (TEnum)value;
            }
            else if (value.GetType().GetTypeInfo().IsEnum)
            {
                throw new ArgumentException($"ConvertFailed{value.GetType()},{typeof(TEnum)}");
            }
            else
            {
                enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
            }

            if (!Enum.IsDefined(typeof(TEnum), enumValue))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Invalid Enum Value{typeof(TEnum)},{enumValue}");
            }

            return enumValue;
        }

        /// <summary>
        ///     Clears the contents of the builder.
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            for (var i = 0; i < _validKeywords.Count; i++)
            {
                Reset((Keywords)i);
            }
        }

        /// <summary>
        ///     Determines whether the specified key is used by the connection string.
        /// </summary>
        /// <param name="keyword">The key to look for.</param>
        /// <returns>true if it is use; otherwise, false.</returns>
        public override bool ContainsKey(string keyword)
            => _keywords.ContainsKey(keyword);

        /// <summary>
        ///     Removes the specified key and its value from the connection string.
        /// </summary>
        /// <param name="keyword">The key to remove.</param>
        /// <returns>true if the key was used; otherwise, false.</returns>
        public override bool Remove(string keyword)
        {
            if (!_keywords.TryGetValue(keyword, out var index)
                || !base.Remove(_validKeywords[(int)index]))
            {
                return false;
            }

            Reset(index);

            return true;
        }

        /// <summary>
        ///     Determines whether the specified key should be serialized into the connection string.
        /// </summary>
        /// <param name="keyword">The key to check.</param>
        /// <returns>true if it should be serialized; otherwise, false.</returns>
        public override bool ShouldSerialize(string keyword)
            => _keywords.TryGetValue(keyword, out var index) && base.ShouldSerialize(_validKeywords[(int)index]);

        /// <summary>
        ///     Gets the value of the specified key if it is used.
        /// </summary>
        /// <param name="keyword">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if the key was used; otherwise, false.</returns>
        public override bool TryGetValue(string keyword, out object value)
        {
            if (!_keywords.TryGetValue(keyword, out var index))
            {
                value = null;

                return false;
            }

            value = GetAt(index);

            return true;
        }

        private object GetAt(Keywords index)
        {
            switch (index)
            {
                case Keywords.DataSource:
                    return DataSource;
                case Keywords.Password:
                    return Password;
                case Keywords.Username:
                    return Username;
                case Keywords.DataBase:
                    return DataBase;
                case Keywords.Port:
                    return Port;
                case Keywords.Charset:
                    return Charset;
                default:
                    Debug.Assert(false, "Unexpected keyword: " + index);
                    return null;
            }
        }

        private static Keywords GetIndex(string keyword)
            => !_keywords.TryGetValue(keyword, out var index)
                ? throw new ArgumentException($"Keyword Not Supported{keyword}")
                : index;

        private void Reset(Keywords index)
        {
            switch (index)
            {
                case Keywords.DataSource:
                    _dataSource = string.Empty;
                    return;
                case Keywords.Password:
                    _password = string.Empty;
                    return;
                case Keywords.Username:
                    _userName = string.Empty;
                    return;
                case Keywords.DataBase:
                    _dataBase = string.Empty;
                    return;
                case Keywords.Port:
                    _port=6060;
                    return;
                case Keywords.Charset:
                    _charset = System.Text.Encoding.UTF8.EncodingName;
                    return;
                default:
                    Debug.Assert(false, "Unexpected keyword: " + index);
                    return;
            }
        }
    }
}
