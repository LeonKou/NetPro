// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Represents a collection of Taos parameters.
    /// </summary>
    public class TaosParameterCollection : DbParameterCollection
    {
        private readonly List<TaosParameter> _parameters = new List<TaosParameter>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosParameterCollection" /> class.
        /// </summary>
        protected internal TaosParameterCollection()
        {
        }

        /// <summary>
        ///     Gets the number of items in the collection.
        /// </summary>
        /// <value>The number of items in the collection.</value>
        public override int Count
            => _parameters.Count;

        /// <summary>
        ///     Gets the object used to synchronize access to the collection.
        /// </summary>
        /// <value>The object used to synchronize access to the collection.</value>
        public override object SyncRoot
            => ((ICollection)_parameters).SyncRoot;

#if NET45
        public override bool IsFixedSize => throw null;

        public override bool IsReadOnly => throw null;

        public override bool IsSynchronized => ((ICollection)_parameters).IsSynchronized;
#endif
        /// <summary>
        ///     Gets or sets the parameter at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter.</param>
        /// <returns>The parameter.</returns>
        public new virtual TaosParameter this[int index]
        {
            get => _parameters[index];
            set
            {
                if (_parameters[index] == value)
                {
                    return;
                }

                _parameters[index] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The parameter.</returns>
        public new virtual TaosParameter this[string parameterName]
        {
            get => this[IndexOfChecked(parameterName)];
            set => this[IndexOfChecked(parameterName)] = value;
        }

        /// <summary>
        ///     Adds a parameter to the collection.
        /// </summary>
        /// <param name="value">The parameter to add. Must be a <see cref="TaosParameter" />.</param>
        /// <returns>The zero-based index of the parameter that was added.</returns>
        public override int Add(object value)
        {
            var tpv = (TaosParameter)value;
            _parameters.Add(tpv);
            return Count - 1;
        }

        /// <summary>
        ///     Adds a parameter to the collection.
        /// </summary>
        /// <param name="value">The parameter to add.</param>
        /// <returns>The parameter that was added.</returns>
        public virtual TaosParameter Add(TaosParameter value)
        {
            _parameters.Add(value);

            return value;
        }

        /// <summary>
        ///     Adds a parameter to the collection.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="type">The Taos type of the parameter.</param>
        /// <returns>The parameter that was added.</returns>
        public virtual TaosParameter Add(string parameterName, TaosType type)
            => Add(new TaosParameter(parameterName, type));

        /// <summary>
        ///     Adds a parameter to the collection.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="type">The Taos type of the parameter.</param>
        /// <param name="size">The maximum size, in bytes, of the parameter.</param>
        /// <returns>The parameter that was added.</returns>
        public virtual TaosParameter Add(string parameterName, TaosType type, int size)
            => Add(new TaosParameter(parameterName, type, size));

        /// <summary>
        ///     Adds a parameter to the collection.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="type">The Taos type of the parameter.</param>
        /// <param name="size">The maximum size, in bytes, of the parameter.</param>
        /// <param name="sourceColumn">
        ///     The source column used for loading the value of the parameter. Can be null.
        /// </param>
        /// <returns>The parameter that was added.</returns>
        public virtual TaosParameter Add(string parameterName, TaosType type, int size, string sourceColumn)
            => Add(new TaosParameter(parameterName, type, size, sourceColumn));

        /// <summary>
        ///     Adds multiple parameters to the collection.
        /// </summary>
        /// <param name="values">
        ///     An array of parameters to add. They must be <see cref="TaosParameter" /> objects.
        /// </param>
        public override void AddRange(Array values)
            => AddRange(values.Cast<TaosParameter>());

        /// <summary>
        ///     Adds multiple parameters to the collection.
        /// </summary>
        /// <param name="values">The parameters to add.</param>
        public virtual void AddRange(IEnumerable<TaosParameter> values)
            => _parameters.AddRange(values);

        /// <summary>
        ///     Adds a parameter to the collection.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value of the parameter. Can be null.</param>
        /// <returns>The parameter that was added.</returns>
        public virtual TaosParameter AddWithValue(string parameterName, object value)
        {
            var parameter = new TaosParameter(parameterName, value);
            Add(parameter);

            return parameter;
        }

        /// <summary>
        ///     Removes all parameters from the collection.
        /// </summary>
        public override void Clear()
            => _parameters.Clear();

        /// <summary>
        ///     Gets a value indicating whether the collection contains the specified parameter.
        /// </summary>
        /// <param name="value">The parameter to look for. Must be a <see cref="TaosParameter" />.</param>
        /// <returns>true if the collection contains the parameter; otherwise, false.</returns>
        public override bool Contains(object value)
            => Contains((TaosParameter)value);

        /// <summary>
        ///     Gets a value indicating whether the collection contains the specified parameter.
        /// </summary>
        /// <param name="value">The parameter to look for.</param>
        /// <returns>true if the collection contains the parameter; otherwise, false.</returns>
        public virtual bool Contains(TaosParameter value)
            => _parameters.Contains(value);

        /// <summary>
        ///     Gets a value indicating whether the collection contains a parameter with the specified name.
        /// </summary>
        /// <param name="value">The name of the parameter.</param>
        /// <returns>true if the collection contains the parameter; otherwise, false.</returns>
        public override bool Contains(string value)
            => IndexOf(value) != -1;

        /// <summary>
        ///     Copies the collection to an array of parameters.
        /// </summary>
        /// <param name="array">
        ///     The array into which the parameters are copied. Must be an array of <see cref="TaosParameter" /> objects.
        /// </param>
        /// <param name="index">The zero-based index to which the parameters are copied.</param>
        public override void CopyTo(Array array, int index)
            => CopyTo((TaosParameter[])array, index);

        /// <summary>
        ///     Copies the collection to an array of parameters.
        /// </summary>
        /// <param name="array">The array into which the parameters are copied.</param>
        /// <param name="index">The zero-based index to which the parameters are copied.</param>
        public virtual void CopyTo(TaosParameter[] array, int index)
            => _parameters.CopyTo(array, index);

        /// <summary>
        ///     Gets an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public override IEnumerator GetEnumerator()
            => _parameters.GetEnumerator();

        /// <summary>
        ///     Gets a parameter at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter.</param>
        /// <returns>The parameter.</returns>
        protected override DbParameter GetParameter(int index)
            => this[index];

        /// <summary>
        ///     Gets a parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The parameter.</returns>
        protected override DbParameter GetParameter(string parameterName)
            => GetParameter(IndexOfChecked(parameterName));

        /// <summary>
        ///     Gets the index of the specified parameter.
        /// </summary>
        /// <param name="value">The parameter. Must be a <see cref="TaosParameter" />.</param>
        /// <returns>The zero-based index of the parameter.</returns>
        public override int IndexOf(object value)
            => IndexOf((TaosParameter)value);

        /// <summary>
        ///     Gets the index of the specified parameter.
        /// </summary>
        /// <param name="value">The parameter.</param>
        /// <returns>The zero-based index of the parameter.</returns>
        public virtual int IndexOf(TaosParameter value)
            => _parameters.IndexOf(value);

        /// <summary>
        ///     Gets the index of the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The zero-based index of the parameter or -1 if not found.</returns>
        public override int IndexOf(string parameterName)
        {
            for (var index = 0; index < _parameters.Count; index++)
            {
                if (_parameters[index].ParameterName == parameterName)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Inserts a parameter into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the parameter should be inserted.</param>
        /// <param name="value">The parameter to insert. Must be a <see cref="TaosParameter" />.</param>
        public override void Insert(int index, object value)
            => Insert(index, (TaosParameter)value);

        /// <summary>
        ///     Inserts a parameter into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the parameter should be inserted.</param>
        /// <param name="value">The parameter to insert.</param>
        public virtual void Insert(int index, TaosParameter value)
            => _parameters.Insert(index, value);

        /// <summary>
        ///     Removes a parameter from the collection.
        /// </summary>
        /// <param name="value">The parameter to remove. Must be a <see cref="TaosParameter" />.</param>
        public override void Remove(object value)
            => Remove((TaosParameter)value);

        /// <summary>
        ///     Removes a parameter from the collection.
        /// </summary>
        /// <param name="value">The parameter to remove.</param>
        public virtual void Remove(TaosParameter value)
            => _parameters.Remove(value);

        /// <summary>
        ///     Removes a parameter from the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter to remove.</param>
        public override void RemoveAt(int index)
            => _parameters.RemoveAt(index);

        /// <summary>
        ///     Removes a parameter with the specified name from the collection.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to remove.</param>
        public override void RemoveAt(string parameterName)
            => RemoveAt(IndexOfChecked(parameterName));

        /// <summary>
        ///     Sets the parameter at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter to set.</param>
        /// <param name="value">The parameter. Must be a <see cref="TaosParameter" />.</param>
        protected override void SetParameter(int index, DbParameter value)
            => this[index] = (TaosParameter)value;

        /// <summary>
        ///     Sets the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The parameter. Must be a <see cref="TaosParameter" />.</param>
        protected override void SetParameter(string parameterName, DbParameter value)
            => SetParameter(IndexOfChecked(parameterName), value);

      

        private int IndexOfChecked(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index == -1)
            {
                throw new IndexOutOfRangeException($"ParameterNotFound{parameterName}");
            }

            return index;
        }
    }
}
