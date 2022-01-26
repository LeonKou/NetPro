// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TDengineDriver;

namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Provides methods for reading the result of a command executed against a Taos database.
    /// </summary>
    public class TaosDataReader : DbDataReader
    {
        private readonly TaosCommand _command;
        private bool _hasRows;
        private bool _closed;
        private bool _closeConnection;
        private IntPtr _taosResult;
        private int _fieldCount;
        private IntPtr _taos = IntPtr.Zero;
        IntPtr rowdata;
        List<TDengineMeta> _metas = null;
        private double _date_max_1970;
        private DateTime _dt1970;

        internal TaosDataReader(TaosCommand taosCommand, List<TDengineMeta> metas, bool closeConnection, IntPtr res)
        {
            _taos = taosCommand.Connection._taos;
            _command = taosCommand;
            _closeConnection = closeConnection;
            _fieldCount = TDengine.FieldCount(res);
            _hasRows = TDengine.AffectRows(res) > 0;
            _closed = _closeConnection;
            _taosResult = res;
            _metas = metas;
            _dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            _date_max_1970 = DateTime.MaxValue.Subtract(_dt1970).TotalMilliseconds;
        }

        /// <summary>
        ///     Gets the depth of nesting for the current row. Always zero.
        /// </summary>
        /// <value>The depth of nesting for the current row.</value>
        public override int Depth => 0;

        /// <summary>
        ///     Gets the number of columns in the current row.
        /// </summary>
        /// <value>The number of columns in the current row.</value>
        public override int FieldCount => _fieldCount;

        /// <summary>
        ///     Gets a value indicating whether the data reader contains any rows.
        /// </summary>
        /// <value>A value indicating whether the data reader contains any rows.</value>
        public override bool HasRows
            => _hasRows;

        public TaosException LastTaosException => TDengine.ErrorNo(_taosResult) == 0 ? null : new TaosException(new TaosErrorResult() { Code = TDengine.ErrorNo(_taosResult), Error = TDengine.Error(_taosResult) }, null);

        int ErrorNo => TDengine.ErrorNo(_taosResult);
        /// <summary>
        ///     Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value>A value indicating whether the data reader is closed.</value>
        public override bool IsClosed
            => _closed;

        /// <summary>
        ///     Gets the number of rows inserted, updated, or deleted. -1 for SELECT statements.
        /// </summary>
        /// <value>The number of rows inserted, updated, or deleted.</value>
        public override int RecordsAffected
        {
            get
            {
                return TDengine.AffectRows(_taosResult);
            }
        }

        /// <summary>
        ///     Gets the value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column. The value is case-sensitive.</param>
        /// <returns>The value.</returns>
        public override object this[string name]
            => this[GetOrdinal(name)];

        /// <summary>
        ///     Gets the value of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value.</returns>
        public override object this[int ordinal]
            => GetValue(ordinal);

        /// <summary>
        ///     Gets an enumerator that can be used to iterate through the rows in the data reader.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public override IEnumerator GetEnumerator()
            => new DbEnumerator(this, closeReader: false);


        /// <summary>
        ///     Advances to the next row in the result set.
        /// </summary>
        /// <returns>true if there are more rows; otherwise, false.</returns>
        public override bool Read()
        {
            if (_closed)
            {
                throw new InvalidOperationException($"DataReaderClosed{nameof(Read)}");
            }

            rowdata = TDengine.FetchRows(_taosResult);
            return rowdata != IntPtr.Zero;

        }

        /// <summary>
        ///     Advances to the next result set for batched statements.
        /// </summary>
        /// <returns>true if there are more result sets; otherwise, false.</returns>
        public override bool NextResult()
        {
            return Read();
        }

        /// <summary>
        ///     Closes the data reader.
        /// </summary>
        public override void Close()
            => Dispose(true);

        /// <summary>
        ///     Releases any resources used by the data reader and closes it.
        /// </summary>
        /// <param name="disposing">
        ///     true to release managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _command.DataReader = null;

            _closed = true;

            if (_closeConnection)
            {
                _command.Connection.Close();
            }
            TDengine.FreeResult(_taosResult);
        }

        /// <summary>
        ///     Gets the name of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the column.</returns>
        public override string GetName(int ordinal)
        {
            return _metas[ordinal].name;
        }

        /// <summary>
        ///     Gets the ordinal of the specified column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name)
            => _metas.IndexOf(_metas.FirstOrDefault(m => m.name == name));

        public override string GetDataTypeName(int ordinal)
        {
            return GetFieldType(ordinal).Name;
        }

        /// <summary>
        ///     Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the column.</returns>
        public override Type GetFieldType(int ordinal)
        {
            if (_metas == null || ordinal >= _metas.Count)
            {
                throw new InvalidOperationException($"DataReaderClosed{nameof(GetFieldType)}");
            }
            TDengineMeta meta = _metas[ordinal];
            Type type = typeof(DBNull);
            switch ((TDengineDataType)meta.type)
            {
                case TDengineDataType.TSDB_DATA_TYPE_BOOL:
                    type = typeof(bool);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_TINYINT:
                    type = typeof(sbyte);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_UTINYINT:
                    type = typeof(byte);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_SMALLINT:
                    type = typeof(short);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_USMALLINT:
                    type = typeof(ushort);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_INT:
                    type = typeof(int);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_UINT:
                    type = typeof(uint);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_BIGINT:
                    type = typeof(long);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_UBIGINT:
                    type = typeof(ulong);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_FLOAT:
                    type = typeof(float);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_DOUBLE:
                    type = typeof(double);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_BINARY:
                    type = typeof(string);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_TIMESTAMP:
                    type = typeof(DateTime);
                    break;
                case TDengineDataType.TSDB_DATA_TYPE_NCHAR:
                    type = typeof(string);
                    break;
            }

            return type;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified column is <see cref="DBNull" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>true if the specified column is <see cref="DBNull" />; otherwise, false.</returns>
        public override bool IsDBNull(int ordinal)
                => GetValue(ordinal) == DBNull.Value;

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="bool" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override bool GetBoolean(int ordinal) => Marshal.ReadByte(GetValuePtr(ordinal)) == 0 ? false : true;

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="byte" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override byte GetByte(int ordinal) => Marshal.ReadByte(GetValuePtr(ordinal));

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="char" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override char GetChar(int ordinal) => GetFieldValue<char>(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="DateTime" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override DateTime GetDateTime(int ordinal)
        {
            return GetDateTimeFrom(GetValuePtr(ordinal));
        }

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="DateTimeOffset" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public virtual DateTimeOffset GetDateTimeOffset(int ordinal)
        {
            return GetDateTime(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="TimeSpan" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public virtual TimeSpan GetTimeSpan(int ordinal)
        {
            var val = Marshal.ReadInt64(GetValuePtr(ordinal));
            var _dateTimePrecision = (TSDB_TIME_PRECISION)TDengine.ResultPrecision(_taosResult);
            switch (_dateTimePrecision)
            {
                /*
                * ticks为100纳秒，必须乘以10才能达到微秒级的区分度
                * 1秒s    = 1000毫秒ms
                * 1毫秒ms = 1000微秒us
                * 1微秒us = 1000纳秒ns
                * 因此， 1毫秒ms = 1000000纳秒ns = 10000ticks
                */
                case TSDB_TIME_PRECISION.TSDB_TIME_PRECISION_NANO:
                    return TimeSpan.FromTicks(val / 100);
                case TSDB_TIME_PRECISION.TSDB_TIME_PRECISION_MICRO:
                    return TimeSpan.FromTicks(val * 10);
                case TSDB_TIME_PRECISION.TSDB_TIME_PRECISION_MILLI:
                default:
                    return TimeSpan.FromTicks(val * 10000);
            }
        }

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="decimal" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="double" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override double GetDouble(int ordinal) => (double)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="float" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override float GetFloat(int ordinal) => (float)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="Guid" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override Guid GetGuid(int ordinal) => GetFieldValue<Guid>(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="short" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override short GetInt16(int ordinal) => (short)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="ushort" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public ushort GetUInt16(int ordinal) => (ushort)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="int" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override int GetInt32(int ordinal) => (int)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="uint" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public uint GetUInt32(int ordinal) => (uint)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="long" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override long GetInt64(int ordinal) => (long)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="ulong" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public ulong GetUInt64(int ordinal) => (ulong)GetValue(ordinal);

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="string" />.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        /// <summary>
        ///     Reads a stream of bytes from the specified column. Not supported.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which the data is copied.</param>
        /// <param name="bufferOffset">The index to which the data will be copied.</param>
        /// <param name="length">The maximum number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            byte[] buffer1 = new byte[length + bufferOffset];
            Marshal.Copy(GetValuePtr(ordinal), buffer1, (int)dataOffset, length + bufferOffset);
            Array.Copy(buffer1, bufferOffset, buffer, 0, length);
            return length;
        }

        /// <summary>
        ///     Reads a stream of characters from the specified column. Not supported.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which the data is copied.</param>
        /// <param name="bufferOffset">The index to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
           => throw new NotSupportedException();

        /// <summary>
        ///     Retrieves data as a Stream. If the reader includes rowid (or any of its aliases), a
        ///     <see cref="TaosBlob"/> is returned. Otherwise, the all of the data is read into memory and a
        ///     <see cref="MemoryStream"/> is returned.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override Stream GetStream(int ordinal)
        {
            MemoryStream result = null;
            TDengineMeta meta = _metas[ordinal];
            int offset = IntPtr.Size * ordinal;
            IntPtr data = Marshal.ReadIntPtr(rowdata, offset);
            if (data != IntPtr.Zero)
            {
                byte[] bf = new byte[meta.size];
                Marshal.Copy(data, bf, 0, meta.size);
                result = new MemoryStream(bf);
            }
            return result;
        }

        /// <summary>
        ///     Gets the value of the specified column.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override T GetFieldValue<T>(int ordinal) => (T)Convert.ChangeType(GetValue(ordinal), typeof(T));


        public IntPtr GetValuePtr(int ordinal)
        {
            int offset = IntPtr.Size * ordinal;
            return Marshal.ReadIntPtr(rowdata, offset);
        }

        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

    
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX…1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        ///     Gets the value of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public override object GetValue(int ordinal)
        {
            object result = DBNull.Value;
            TDengineMeta meta = _metas[ordinal];
            int offset = IntPtr.Size * ordinal;
            IntPtr data = Marshal.ReadIntPtr(rowdata, offset);
            if (data != IntPtr.Zero)
            {
                switch ((TDengineDataType)meta.type)
                {
                    case TDengineDataType.TSDB_DATA_TYPE_BOOL:
                        bool v1 = Marshal.ReadByte(data) == 0 ? false : true;
                        result = v1;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_TINYINT:
                        sbyte v2s = (sbyte)Marshal.ReadByte(data);
                        result = v2s;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_UTINYINT:
                        byte v2 = Marshal.ReadByte(data);
                        result = v2;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_SMALLINT:
                        short v3 = Marshal.ReadInt16(data);
                        result = v3;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_USMALLINT:
                        ushort v12 = (ushort)Marshal.ReadInt16(data);
                        result = v12;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_INT:
                        int v4 = Marshal.ReadInt32(data);
                        result = v4;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_UINT:
                        uint v13 = (uint)Marshal.ReadInt32(data);
                        result = v13;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_BIGINT:
                        long v5 = Marshal.ReadInt64(data);
                        result = v5;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_UBIGINT:
                        ulong v14 = (ulong)Marshal.ReadInt64(data);
                        result = v14;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_FLOAT:
                        float v6 = (float)Marshal.PtrToStructure(data, typeof(float));
                        result = v6;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_DOUBLE:
                        double v7 = (double)Marshal.PtrToStructure(data, typeof(double));
                        result = v7;
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_BINARY:
                        {
                            result = Marshal.PtrToStringAnsi(data, meta.size)?.RemoveNull();
                        }
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_TIMESTAMP:
                        {
                            result = GetDateTimeFrom(data);
                        }
                        break;
                    case TDengineDataType.TSDB_DATA_TYPE_NCHAR:
                        {
                            string v10 = string.Empty;
                            if (meta.size > 0)// https://github.com/maikebing/Maikebing.EntityFrameworkCore.Taos/issues/99
                            {
                                byte[] bf = new byte[meta.size];
                                Marshal.Copy(data, bf, 0, meta.size);
                              
                                if (IsUTF8Bytes(bf) || (bf[0] == 0xEF && bf[1] == 0xBB && bf[2] == 0xBF))
                                {
                                    v10 = System.Text.Encoding.UTF8.GetString(bf)?.RemoveNull();
                                }
                                else
                                {
                                    v10 = System.Text.Encoding.GetEncoding(936).GetString(bf)?.RemoveNull();
                                }
                            }
                            result = v10;
                        }
                        break;
                }

            }
            else
            {
                result = DBNull.Value;
            }
            return result;
        }
    
        private DateTime GetDateTimeFrom(IntPtr data)
        {
            var val = Marshal.ReadInt64(data);
            //double tsp;
            var _dateTimePrecision = (TSDB_TIME_PRECISION)TDengine.ResultPrecision(_taosResult);
            switch (_dateTimePrecision)
            {
                /*
                * ticks为100纳秒，必须乘以10才能达到微秒级的区分度
                * 1秒s    = 1000毫秒ms
                * 1毫秒ms = 1000微秒us
                * 1微秒us = 1000纳秒ns
                * 因此， 1毫秒ms = 1000000纳秒ns = 10000ticks
                */
                case TSDB_TIME_PRECISION.TSDB_TIME_PRECISION_NANO:
                    val /= 100;
                    break;
                case TSDB_TIME_PRECISION.TSDB_TIME_PRECISION_MICRO:
                    val *= 10;
                    break;
                case TSDB_TIME_PRECISION.TSDB_TIME_PRECISION_MILLI:
                default:
                    val *= 10000;
                    break;
            }
            var v9 = _dt1970.AddTicks(val);
            return v9.ToLocalTime();
        }

        /// <summary>
        ///     Gets the column values of the current row.
        /// </summary>
        /// <param name="values">An array into which the values are copied.</param>
        /// <returns>The number of values copied into the array.</returns>
        public override int GetValues(object[] values)
        {
            int count = 0;
            for (int i = 0; i < _fieldCount; i++)
            {
                var obj = GetValue(i);
                if (obj != null)
                {
                    values[i] = obj;
                    count++;
                }
            }
            return count;
        }


        /// <summary>
        ///     Returns a System.Data.DataTable that describes the column metadata of the System.Data.Common.DbDataReader.
        /// </summary>
        /// <returns>A System.Data.DataTable that describes the column metadata.</returns>
        public override DataTable GetSchemaTable()
        {
            var schemaTable = new DataTable("SchemaTable");
            if (_metas != null && _metas.Count > 0)
            {
                var ColumnName = new DataColumn(SchemaTableColumn.ColumnName, typeof(string));
                var ColumnOrdinal = new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int));
                var ColumnSize = new DataColumn(SchemaTableColumn.ColumnSize, typeof(int));
                var NumericPrecision = new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short));
                var NumericScale = new DataColumn(SchemaTableColumn.NumericScale, typeof(short));

                var DataType = new DataColumn(SchemaTableColumn.DataType, typeof(Type));
                var DataTypeName = new DataColumn("DataTypeName", typeof(string));

                var IsLong = new DataColumn(SchemaTableColumn.IsLong, typeof(bool));
                var AllowDBNull = new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool));

                var IsUnique = new DataColumn(SchemaTableColumn.IsUnique, typeof(bool));
                var IsKey = new DataColumn(SchemaTableColumn.IsKey, typeof(bool));
                var IsAutoIncrement = new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));

                var BaseCatalogName = new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
                var BaseSchemaName = new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string));
                var BaseTableName = new DataColumn(SchemaTableColumn.BaseTableName, typeof(string));
                var BaseColumnName = new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string));

                var BaseServerName = new DataColumn(SchemaTableOptionalColumn.BaseServerName, typeof(string));
                var IsAliased = new DataColumn(SchemaTableColumn.IsAliased, typeof(bool));
                var IsExpression = new DataColumn(SchemaTableColumn.IsExpression, typeof(bool));

                var columns = schemaTable.Columns;

                columns.Add(ColumnName);
                columns.Add(ColumnOrdinal);
                columns.Add(ColumnSize);
                columns.Add(NumericPrecision);
                columns.Add(NumericScale);
                columns.Add(IsUnique);
                columns.Add(IsKey);
                columns.Add(BaseServerName);
                columns.Add(BaseCatalogName);
                columns.Add(BaseColumnName);
                columns.Add(BaseSchemaName);
                columns.Add(BaseTableName);
                columns.Add(DataType);
                columns.Add(DataTypeName);
                columns.Add(AllowDBNull);
                columns.Add(IsAliased);
                columns.Add(IsExpression);
                columns.Add(IsAutoIncrement);
                columns.Add(IsLong);

                for (var i = 0; i < _metas.Count; i++)
                {
                    var schemaRow = schemaTable.NewRow();

                    schemaRow[ColumnName] = GetName(i);
                    schemaRow[ColumnOrdinal] = i;
                    schemaRow[ColumnSize] = _metas[i].size;
                    schemaRow[NumericPrecision] = DBNull.Value;
                    schemaRow[NumericScale] = DBNull.Value;
                    schemaRow[BaseServerName] = _command.Connection.DataSource;
                    var databaseName = _command.Connection.Database;
                    schemaRow[BaseCatalogName] = databaseName;
                    var columnName = GetName(i);
                    schemaRow[BaseColumnName] = columnName;
                    schemaRow[BaseSchemaName] = DBNull.Value;
                    var tableName = string.Empty;
                    schemaRow[BaseTableName] = tableName;
                    schemaRow[DataType] = GetFieldType(i);
                    schemaRow[DataTypeName] = GetDataTypeName(i);
                    schemaRow[IsAliased] = columnName != GetName(i);
                    schemaRow[IsExpression] = columnName == null;
                    schemaRow[IsLong] = DBNull.Value;
                    if (i == 0)
                    {
                        schemaRow[IsKey] = true;
                        schemaRow[DataType] = GetFieldType(i);
                        schemaRow[DataTypeName] = GetDataTypeName(i);
                    }
                    schemaTable.Rows.Add(schemaRow);
                }

            }

            return schemaTable;
        }
    }
}