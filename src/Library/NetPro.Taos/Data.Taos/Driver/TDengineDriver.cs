/*
 * Copyright (c) 2019 TAOS Data, Inc. <jhtao@taosdata.com>
 *
 * This program is free software: you can use, redistribute, and/or modify
 * it under the terms of the GNU Affero General Public License, version 3
 * or later ("AGPL"), as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace TDengineDriver
{
    using TAOS = System.IntPtr;
    using TAOS_STMT = System.IntPtr;
    using TAOS_RES = System.IntPtr;
    using TAOS_STREAM = System.IntPtr;
    using TAOS_SUB = System.IntPtr;
    using TAOS_ROW = System.IntPtr;

    public enum TSDB_CODE : int
    {

        // rpc
        RPC_ACTION_IN_PROGRESS = 0x0001,//Action in progress
        RPC_AUTH_REQUIRED = 0x0002,//Authentication required
        RPC_AUTH_FAILURE = 0x0003,//Authentication failure
        RPC_REDIRECT = 0x0004,//Redirect
        RPC_NOT_READY = 0x0005,//System not ready    // peer is not ready to process data
        RPC_ALREADY_PROCESSED = 0x0006,//Message already processed
        RPC_LAST_SESSION_NOT_FINISHED = 0x0007,//Last session not finished
        RPC_MISMATCHED_LINK_ID = 0x0008,//Mismatched meter id
        RPC_TOO_SLOW = 0x0009,//Processing of request timed out
        RPC_MAX_SESSIONS = 0x000A,//Number of sessions reached limit    // too many sessions
        RPC_NETWORK_UNAVAIL = 0x000B,//Unable to establish connection
        RPC_APP_ERROR = 0x000C,//Unexpected generic error in RPC
        RPC_UNEXPECTED_RESPONSE = 0x000D,//Unexpected response
        RPC_INVALID_VALUE = 0x000E,//Invalid value
        RPC_INVALID_TRAN_ID = 0x000F,//Invalid transaction id
        RPC_INVALID_SESSION_ID = 0x0010,//Invalid session id
        RPC_INVALID_MSG_TYPE = 0x0011,//Invalid message type
        RPC_INVALID_RESPONSE_TYPE = 0x0012,//Invalid response type
        RPC_INVALID_TIME_STAMP = 0x0013,//Invalid timestamp
        APP_NOT_READY = 0x0014,//Database not ready
        RPC_FQDN_ERROR = 0x0015,//Unable to resolve FQDN

        //common & util
        COM_OPS_NOT_SUPPORT = 0x0100,//Operation not supported
        COM_MEMORY_CORRUPTED = 0x0101,//Memory corrupted
        COM_OUT_OF_MEMORY = 0x0102,//Out of memory
        COM_INVALID_CFG_MSG = 0x0103,//Invalid config message
        COM_FILE_CORRUPTED = 0x0104,//Data file corrupted
        REF_NO_MEMORY = 0x0105,//Ref out of memory
        REF_FULL = 0x0106,//too many Ref Objs
        REF_ID_REMOVED = 0x0107,//Ref ID is removed
        REF_INVALID_ID = 0x0108,//Invalid Ref ID
        REF_ALREADY_EXIST = 0x0109,//Ref is already there
        REF_NOT_EXIST = 0x010A,//Ref is not there

        //client
        TSC_INVALID_SQL = 0x0200,//Invalid SQL statement
        TSC_INVALID_QHANDLE = 0x0201,//Invalid qhandle
        TSC_INVALID_TIME_STAMP = 0x0202,//Invalid combination of client/service time
        TSC_INVALID_VALUE = 0x0203,//Invalid value in client
        TSC_INVALID_VERSION = 0x0204,//Invalid client version
        TSC_INVALID_IE = 0x0205,//Invalid client ie
        TSC_INVALID_FQDN = 0x0206,//Invalid host name
        TSC_INVALID_USER_LENGTH = 0x0207,//Invalid user name
        TSC_INVALID_PASS_LENGTH = 0x0208,//Invalid password
        TSC_INVALID_DB_LENGTH = 0x0209,//Database name too long
        TSC_INVALID_TABLE_ID_LENGTH = 0x020A,//Table name too long
        TSC_INVALID_CONNECTION = 0x020B,//Invalid connection
        TSC_OUT_OF_MEMORY = 0x020C,//System out of memory
        TSC_NO_DISKSPACE = 0x020D,//System out of disk space
        TSC_QUERY_CACHE_ERASED = 0x020E,//Query cache erased
        TSC_QUERY_CANCELLED = 0x020F,//Query terminated
        TSC_SORTED_RES_TOO_MANY = 0x0210,//Result set too large to be sorted      // too many result for ordered super table projection query
        TSC_APP_ERROR = 0x0211,//Application error
        TSC_ACTION_IN_PROGRESS = 0x0212,//Action in progress
        TSC_DISCONNECTED = 0x0213,//Disconnected from service
        TSC_NO_WRITE_AUTH = 0x0214,//No write permission
        TSC_CONN_KILLED = 0x0215,//Connection killed
        TSC_SQL_SYNTAX_ERROR = 0x0216,//Syntax error in SQL
        TSC_DB_NOT_SELECTED = 0x0217,//Database not specified or available
        TSC_INVALID_TABLE_NAME = 0x0218,//Table does not exist

        // mnode
        MND_MSG_NOT_PROCESSED = 0x0300,//Message not processed
        MND_ACTION_IN_PROGRESS = 0x0301,//Message is progressing
        MND_ACTION_NEED_REPROCESSED = 0x0302,//Messag need to be reprocessed
        MND_NO_RIGHTS = 0x0303,//Insufficient privilege for operation
        MND_APP_ERROR = 0x0304,//Unexpected generic error in mnode
        MND_INVALID_CONNECTION = 0x0305,//Invalid message connection
        MND_INVALID_MSG_VERSION = 0x0306,//Incompatible protocol version
        MND_INVALID_MSG_LEN = 0x0307,//Invalid message length
        MND_INVALID_MSG_TYPE = 0x0308,//Invalid message type
        MND_TOO_MANY_SHELL_CONNS = 0x0309,//Too many connections
        MND_OUT_OF_MEMORY = 0x030A,//Out of memory in mnode
        MND_INVALID_SHOWOBJ = 0x030B,//Data expired
        MND_INVALID_QUERY_ID = 0x030C,//Invalid query id
        MND_INVALID_STREAM_ID = 0x030D,//Invalid stream id
        MND_INVALID_CONN_ID = 0x030E,//Invalid connection id

        MND_SDB_OBJ_ALREADY_THERE = 0x0320,//Object already there
        MND_SDB_ERROR = 0x0321,//Unexpected generic error in sdb
        MND_SDB_INVALID_TABLE_TYPE = 0x0322,//Invalid table type
        MND_SDB_OBJ_NOT_THERE = 0x0323,//Object not there
        MND_SDB_INVAID_META_ROW = 0x0324,//Invalid meta row
        MND_SDB_INVAID_KEY_TYPE = 0x0325,//Invalid key type

        MND_DNODE_ALREADY_EXIST = 0x0330,//DNode already exists
        MND_DNODE_NOT_EXIST = 0x0331,//DNode does not exist
        MND_VGROUP_NOT_EXIST = 0x0332,//VGroup does not exist
        MND_NO_REMOVE_MASTER = 0x0333,//Master DNode cannot be removed
        MND_NO_ENOUGH_DNODES = 0x0334,//Out of DNodes
        MND_CLUSTER_CFG_INCONSISTENT = 0x0335,//Cluster cfg inconsistent
        MND_INVALID_DNODE_CFG_OPTION = 0x0336,//Invalid dnode cfg option
        MND_BALANCE_ENABLED = 0x0337,//Balance already enabled
        MND_VGROUP_NOT_IN_DNODE = 0x0338,//Vgroup not in dnode
        MND_VGROUP_ALREADY_IN_DNODE = 0x0339,//Vgroup already in dnode
        MND_DNODE_NOT_FREE = 0x033A,//Dnode not avaliable
        MND_INVALID_CLUSTER_ID = 0x033B,//Cluster id not match
        MND_NOT_READY = 0x033C,//Cluster not ready
        MND_DNODE_ID_NOT_CONFIGURED = 0x033D,//Dnode Id not configured
        MND_DNODE_EP_NOT_CONFIGURED = 0x033E,//Dnode Ep not configured

        MND_ACCT_ALREADY_EXIST = 0x0340,//Account already exists
        MND_INVALID_ACCT = 0x0341,//Invalid account
        MND_INVALID_ACCT_OPTION = 0x0342,//Invalid account options
        MND_ACCT_EXPIRED = 0x0343,//Account authorization has expired

        MND_USER_ALREADY_EXIST = 0x0350,//User already exists
        MND_INVALID_USER = 0x0351,//Invalid user
        MND_INVALID_USER_FORMAT = 0x0352,//Invalid user format
        MND_INVALID_PASS_FORMAT = 0x0353,//Invalid password format
        MND_NO_USER_FROM_CONN = 0x0354,//Can not get user from conn
        MND_TOO_MANY_USERS = 0x0355,//Too many users

        MND_TABLE_ALREADY_EXIST = 0x0360,//Table already exists
        MND_INVALID_TABLE_ID = 0x0361,//Table name too long
        MND_INVALID_TABLE_NAME = 0x0362,//Table does not exist
        MND_INVALID_TABLE_TYPE = 0x0363,//Invalid table type in tsdb
        MND_TOO_MANY_TAGS = 0x0364,//Too many tags
        MND_TOO_MANY_TIMESERIES = 0x0366,//Too many time series
        MND_NOT_SUPER_TABLE = 0x0367,//Not super table           // operation only available for super table
        MND_COL_NAME_TOO_LONG = 0x0368,//Tag name too long
        MND_TAG_ALREAY_EXIST = 0x0369,//Tag already exists
        MND_TAG_NOT_EXIST = 0x036A,//Tag does not exist
        MND_FIELD_ALREAY_EXIST = 0x036B,//Field already exists
        MND_FIELD_NOT_EXIST = 0x036C,//Field does not exist
        MND_INVALID_STABLE_NAME = 0x036D,//Super table does not exist

        MND_DB_NOT_SELECTED = 0x0380,//Database not specified or available
        MND_DB_ALREADY_EXIST = 0x0381,//Database already exists
        MND_INVALID_DB_OPTION = 0x0382,//Invalid database options
        MND_INVALID_DB = 0x0383,//Invalid database name
        MND_MONITOR_DB_FORBIDDEN = 0x0384,//Cannot delete monitor database
        MND_TOO_MANY_DATABASES = 0x0385,//Too many databases for account
        MND_DB_IN_DROPPING = 0x0386,//Database not available

        // dnode
        DND_MSG_NOT_PROCESSED = 0x0400,//Message not processed
        DND_OUT_OF_MEMORY = 0x0401,//Dnode out of memory
        DND_NO_WRITE_ACCESS = 0x0402,//No permission for disk files in dnode
        DND_INVALID_MSG_LEN = 0x0403,//Invalid message length

        // vnode 
        VND_ACTION_IN_PROGRESS = 0x0500,//Action in progress
        VND_MSG_NOT_PROCESSED = 0x0501,//Message not processed
        VND_ACTION_NEED_REPROCESSED = 0x0502,//Action need to be reprocessed
        VND_INVALID_VGROUP_ID = 0x0503,//Invalid Vgroup ID
        VND_INIT_FAILED = 0x0504,//Vnode initialization failed
        VND_NO_DISKSPACE = 0x0505,//System out of disk space
        VND_NO_DISK_PERMISSIONS = 0x0506,//No write permission for disk files
        VND_NO_SUCH_FILE_OR_DIR = 0x0507,//Missing data file
        VND_OUT_OF_MEMORY = 0x0508,//Out of memory
        VND_APP_ERROR = 0x0509,//Unexpected generic error in vnode
        VND_INVALID_VRESION_FILE = 0x050A,//Invalid version file
        VND_NOT_SYNCED = 0x0511,//Database suspended
        VND_NO_WRITE_AUTH = 0x0512,//Write operation denied

        // tsdb
        TDB_INVALID_TABLE_ID = 0x0600,//Invalid table ID
        TDB_INVALID_TABLE_TYPE = 0x0601,//Invalid table type
        TDB_IVD_TB_SCHEMA_VERSION = 0x0602,//Invalid table schema version
        TDB_TABLE_ALREADY_EXIST = 0x0603,//Table already exists
        TDB_INVALID_CONFIG = 0x0604,//Invalid configuration
        TDB_INIT_FAILED = 0x0605,//Tsdb init failed
        TDB_NO_DISKSPACE = 0x0606,//No diskspace for tsdb
        TDB_NO_DISK_PERMISSIONS = 0x0607,//No permission for disk files
        TDB_FILE_CORRUPTED = 0x0608,//Data file(s) corrupted
        TDB_OUT_OF_MEMORY = 0x0609,//Out of memory
        TDB_TAG_VER_OUT_OF_DATE = 0x060A,//Tag too old
        TDB_TIMESTAMP_OUT_OF_RANGE = 0x060B,//Timestamp data out of range
        TDB_SUBMIT_MSG_MSSED_UP = 0x060C,//Submit message is messed up
        TDB_INVALID_ACTION = 0x060D,//Invalid operation
        TDB_INVALID_CREATE_TB_MSG = 0x060E,//Invalid creation of table
        TDB_NO_TABLE_DATA_IN_MEM = 0x060F,//No table data in memory skiplist
        TDB_FILE_ALREADY_EXISTS = 0x0610,//File already exists
        TDB_TABLE_RECONFIGURE = 0x0611,//Need to reconfigure table
        TDB_IVD_CREATE_TABLE_INFO = 0x0612,//Invalid information to create table

        // query
        QRY_INVALID_QHANDLE = 0x0700,//Invalid handle
        QRY_INVALID_MSG = 0x0701,//Invalid message    // failed to validate the sql expression msg by vnode
        QRY_NO_DISKSPACE = 0x0702,//No diskspace for query
        QRY_OUT_OF_MEMORY = 0x0703,//System out of memory
        QRY_APP_ERROR = 0x0704,//Unexpected generic error in query
        QRY_DUP_JOIN_KEY = 0x0705,//Duplicated join key
        QRY_EXCEED_TAGS_LIMIT = 0x0706,//Tag conditon too many
        QRY_NOT_READY = 0x0707,//Query not ready
        QRY_HAS_RSP = 0x0708,//Query should response
        QRY_IN_EXEC = 0x0709,//Multiple retrieval of this query
        QRY_TOO_MANY_TIMEWINDOW = 0x070A,//Too many time window in query
        QRY_NOT_ENOUGH_BUFFER = 0x070B,//Query buffer limit has reached

        // grant
        GRANT_EXPIRED = 0x0800,//License expired
        GRANT_DNODE_LIMITED = 0x0801,//DNode creation limited by licence
        GRANT_ACCT_LIMITED = 0x0802,//Account creation limited by license
        GRANT_TIMESERIES_LIMITED = 0x0803,//Table creation limited by license
        GRANT_DB_LIMITED = 0x0804,//DB creation limited by license
        GRANT_USER_LIMITED = 0x0805,//User creation limited by license
        GRANT_CONN_LIMITED = 0x0806,//Conn creation limited by license
        GRANT_STREAM_LIMITED = 0x0807,//Stream creation limited by license
        GRANT_SPEED_LIMITED = 0x0808,//Write speed limited by license
        GRANT_STORAGE_LIMITED = 0x0809,//Storage capacity limited by license
        GRANT_QUERYTIME_LIMITED = 0x080A,//Query time limited by license
        GRANT_CPU_LIMITED = 0x080B,//CPU cores limited by license

        // sync
        SYN_INVALID_CONFIG = 0x0900,//Invalid Sync Configuration
        SYN_NOT_ENABLED = 0x0901,//Sync module not enabled
        SYN_INVALID_VERSION = 0x0902,//Invalid Sync version

        // wal
        WAL_APP_ERROR = 0x1000,//Unexpected generic error in wal
        WAL_FILE_CORRUPTED = 0x1001,//WAL file is corrupted

        // http
        HTTP_SERVER_OFFLINE = 0x1100,//http server is not onlin
        HTTP_UNSUPPORT_URL = 0x1101,//url is not support
        HTTP_INVLALID_URL = 0x1102,//invalid url format
        HTTP_NO_ENOUGH_MEMORY = 0x1103,//no enough memory
        HTTP_REQUSET_TOO_BIG = 0x1104,//request size is too big
        HTTP_NO_AUTH_INFO = 0x1105,//no auth info input
        HTTP_NO_MSG_INPUT = 0x1106,//request is empty
        HTTP_NO_SQL_INPUT = 0x1107,//no sql input
        HTTP_NO_EXEC_USEDB = 0x1108,//no need to execute use db cmd
        HTTP_SESSION_FULL = 0x1109,//session list was full
        HTTP_GEN_TAOSD_TOKEN_ERR = 0x110A,//generate taosd token error
        HTTP_INVALID_MULTI_REQUEST = 0x110B,//size of multi request is 0
        HTTP_CREATE_GZIP_FAILED = 0x110C,//failed to create gzip
        HTTP_FINISH_GZIP_FAILED = 0x110D,//failed to finish gzip
        HTTP_LOGIN_FAILED = 0x110E,//failed to login

        HTTP_INVALID_VERSION = 0x1120,//invalid http version
        HTTP_INVALID_CONTENT_LENGTH = 0x1121,//invalid content length
        HTTP_INVALID_AUTH_TYPE = 0x1122,//invalid type of Authorization
        HTTP_INVALID_AUTH_FORMAT = 0x1123,//invalid format of Authorization
        HTTP_INVALID_BASIC_AUTH = 0x1124,//invalid basic Authorization
        HTTP_INVALID_TAOSD_AUTH = 0x1125,//invalid taosd Authorization
        HTTP_PARSE_METHOD_FAILED = 0x1126,//failed to parse method
        HTTP_PARSE_TARGET_FAILED = 0x1127,//failed to parse target
        HTTP_PARSE_VERSION_FAILED = 0x1128,//failed to parse http version
        HTTP_PARSE_SP_FAILED = 0x1129,//failed to parse sp
        HTTP_PARSE_STATUS_FAILED = 0x112A,//failed to parse status
        HTTP_PARSE_PHRASE_FAILED = 0x112B,//failed to parse phrase
        HTTP_PARSE_CRLF_FAILED = 0x112C,//failed to parse crlf
        HTTP_PARSE_HEADER_FAILED = 0x112D,//failed to parse header
        HTTP_PARSE_HEADER_KEY_FAILED = 0x112E,//failed to parse header key
        HTTP_PARSE_HEADER_VAL_FAILED = 0x112F,//failed to parse header val
        HTTP_PARSE_CHUNK_SIZE_FAILED = 0x1130,//failed to parse chunk size
        HTTP_PARSE_CHUNK_FAILED = 0x1131,//failed to parse chunk
        HTTP_PARSE_END_FAILED = 0x1132,//failed to parse end section
        HTTP_PARSE_INVALID_STATE = 0x1134,//invalid parse state
        HTTP_PARSE_ERROR_STATE = 0x1135,//failed to parse error section

        HTTP_GC_QUERY_NULL = 0x1150,//query size is 0
        HTTP_GC_QUERY_SIZE = 0x1151,//query size can not more than 100
        HTTP_GC_REQ_PARSE_ERROR = 0x1152,//parse grafana json error

        HTTP_TG_DB_NOT_INPUT = 0x1160,//database name can not be null
        HTTP_TG_DB_TOO_LONG = 0x1161,//database name too long
        HTTP_TG_INVALID_JSON = 0x1162,//invalid telegraf json fromat
        HTTP_TG_METRICS_NULL = 0x1163,//metrics size is 0
        HTTP_TG_METRICS_SIZE = 0x1164,//metrics size can not more than 1K
        HTTP_TG_METRIC_NULL = 0x1165,//metric name not find
        HTTP_TG_METRIC_TYPE = 0x1166,//metric name type should be string
        HTTP_TG_METRIC_NAME_NULL = 0x1167,//metric name length is 0
        HTTP_TG_METRIC_NAME_LONG = 0x1168,//metric name length too long
        HTTP_TG_TIMESTAMP_NULL = 0x1169,//timestamp not find
        HTTP_TG_TIMESTAMP_TYPE = 0x116A,//timestamp type should be integer
        HTTP_TG_TIMESTAMP_VAL_NULL = 0x116B,//timestamp value smaller than 0
        HTTP_TG_TAGS_NULL = 0x116C,//tags not find
        HTTP_TG_TAGS_SIZE_0 = 0x116D,//tags size is 0
        HTTP_TG_TAGS_SIZE_LONG = 0x116E,//tags size too long
        HTTP_TG_TAG_NULL = 0x116F,//tag is null
        HTTP_TG_TAG_NAME_NULL = 0x1170,//tag name is null
        HTTP_TG_TAG_NAME_SIZE = 0x1171,//tag name length too long
        HTTP_TG_TAG_VALUE_TYPE = 0x1172,//tag value type should be number or string
        HTTP_TG_TAG_VALUE_NULL = 0x1173,//tag value is null
        HTTP_TG_TABLE_NULL = 0x1174,//table is null
        HTTP_TG_TABLE_SIZE = 0x1175,//table name length too long
        HTTP_TG_FIELDS_NULL = 0x1176,//fields not find
        HTTP_TG_FIELDS_SIZE_0 = 0x1177,//fields size is 0
        HTTP_TG_FIELDS_SIZE_LONG = 0x1178,//fields size too long
        HTTP_TG_FIELD_NULL = 0x1179,//field is null
        HTTP_TG_FIELD_NAME_NULL = 0x117A,//field name is null
        HTTP_TG_FIELD_NAME_SIZE = 0x117B,//field name length too long
        HTTP_TG_FIELD_VALUE_TYPE = 0x117C,//field value type should be number or string
        HTTP_TG_FIELD_VALUE_NULL = 0x117D,//field value is null
        HTTP_TG_HOST_NOT_STRING = 0x117E,//host type should be string
        HTTP_TG_STABLE_NOT_EXIST = 0x117F,//stable not exist

        HTTP_OP_DB_NOT_INPUT = 0x1190,//database name can not be null
        HTTP_OP_DB_TOO_LONG = 0x1191,//database name too long
        HTTP_OP_INVALID_JSON = 0x1192,//invalid opentsdb json fromat
        HTTP_OP_METRICS_NULL = 0x1193,//metrics size is 0
        HTTP_OP_METRICS_SIZE = 0x1194,//metrics size can not more than 10K
        HTTP_OP_METRIC_NULL = 0x1195,//metric name not find
        HTTP_OP_METRIC_TYPE = 0x1196,//metric name type should be string
        HTTP_OP_METRIC_NAME_NULL = 0x1197,//metric name length is 0
        HTTP_OP_METRIC_NAME_LONG = 0x1198,//metric name length can not more than 22
        HTTP_OP_TIMESTAMP_NULL = 0x1199,//timestamp not find
        HTTP_OP_TIMESTAMP_TYPE = 0x119A,//timestamp type should be integer
        HTTP_OP_TIMESTAMP_VAL_NULL = 0x119B,//timestamp value smaller than 0
        HTTP_OP_TAGS_NULL = 0x119C,//tags not find
        HTTP_OP_TAGS_SIZE_0 = 0x119D,//tags size is 0
        HTTP_OP_TAGS_SIZE_LONG = 0x119E,//tags size too long
        HTTP_OP_TAG_NULL = 0x119F,//tag is null
        HTTP_OP_TAG_NAME_NULL = 0x11A0,//tag name is null
        HTTP_OP_TAG_NAME_SIZE = 0x11A1,//tag name length too long
        HTTP_OP_TAG_VALUE_TYPE = 0x11A2,//tag value type should be boolean, number or string
        HTTP_OP_TAG_VALUE_NULL = 0x11A3,//tag value is null
        HTTP_OP_TAG_VALUE_TOO_LONG = 0x11A4,//tag value can not more than 64
        HTTP_OP_VALUE_NULL = 0x11A5,//value not find
        HTTP_OP_VALUE_TYPE = 0x11A6,//value type should be boolean, number or string


        ODBC_OOM = 0x2101,//out of memory
        ODBC_CONV_UNDEF = 0x2102,//convertion undefined
        ODBC_CONV_TRUNC = 0x2103,//convertion truncated
        ODBC_CONV_NOT_SUPPORT = 0x2104,//convertion not supported
        ODBC_OUT_OF_RANGE = 0x2105,//out of range
        ODBC_NOT_SUPPORT = 0x2106,//not supported yet
        ODBC_INVALID_HANDLE = 0x2107,//invalid handle
        ODBC_NO_RESULT = 0x2108,//no result set
        ODBC_NO_FIELDS = 0x2109,//no fields returned
        ODBC_INVALID_CURSOR = 0x2110,//invalid cursor
        ODBC_STATEMENT_NOT_READY = 0x2111,//statement not ready
        ODBC_CONNECTION_BUSY = 0x2112,//connection still busy
        ODBC_BAD_CONNSTR = 0x2113,//bad connection string
        ODBC_BAD_ARG = 0x2114,//bad argument
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct taosField
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
        public byte[] name;

        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte type;

        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort bytes;
    }

    enum TDengineDataType
    {
        TSDB_DATA_TYPE_NULL = 0,     // 1 bytes
        TSDB_DATA_TYPE_BOOL = 1,     // 1 bytes
        TSDB_DATA_TYPE_TINYINT = 2,  // 1 bytes
        TSDB_DATA_TYPE_SMALLINT = 3, // 2 bytes
        TSDB_DATA_TYPE_INT = 4,      // 4 bytes
        TSDB_DATA_TYPE_BIGINT = 5,   // 8 bytes
        TSDB_DATA_TYPE_FLOAT = 6,    // 4 bytes
        TSDB_DATA_TYPE_DOUBLE = 7,   // 8 bytes
        TSDB_DATA_TYPE_BINARY = 8,   // string
        TSDB_DATA_TYPE_TIMESTAMP = 9,// 8 bytes
        TSDB_DATA_TYPE_NCHAR = 10,   // unicode string
        TSDB_DATA_TYPE_UTINYINT = 11,// 1 byte
        TSDB_DATA_TYPE_USMALLINT = 12,// 2 bytes
        TSDB_DATA_TYPE_UINT = 13,    // 4 bytes
        TSDB_DATA_TYPE_UBIGINT = 14   // 8 bytes
    }
    public enum TSDB_TIME_PRECISION : int
    {
        TSDB_TIME_PRECISION_MILLI = 0,
        TSDB_TIME_PRECISION_MICRO = 1,
        TSDB_TIME_PRECISION_NANO = 2
    }
    internal enum TDengineInitOption
    {
        TSDB_OPTION_LOCALE = 0,
        TSDB_OPTION_CHARSET = 1,
        TSDB_OPTION_TIMEZONE = 2,
        TDDB_OPTION_CONFIGDIR = 3,
        TDDB_OPTION_SHELL_ACTIVITY_TIMER = 4
    }

    internal class TDengineMeta
    {
        public string name;
        public ushort size;
        public byte type;

        public string TypeName()
        {
            switch ((TDengineDataType)type)
            {
                case TDengineDataType.TSDB_DATA_TYPE_BOOL:
                    return "BOOL";
                case TDengineDataType.TSDB_DATA_TYPE_TINYINT:
                    return "TINYINT";
                case TDengineDataType.TSDB_DATA_TYPE_SMALLINT:
                    return "SMALLINT";
                case TDengineDataType.TSDB_DATA_TYPE_INT:
                    return "INT";
                case TDengineDataType.TSDB_DATA_TYPE_BIGINT:
                    return "BIGINT";
                case TDengineDataType.TSDB_DATA_TYPE_UTINYINT:
                    return "TINYINT UNSIGNED";
                case TDengineDataType.TSDB_DATA_TYPE_USMALLINT:
                    return "SMALLINT UNSIGNED";
                case TDengineDataType.TSDB_DATA_TYPE_UINT:
                    return "INT UNSIGNED";
                case TDengineDataType.TSDB_DATA_TYPE_UBIGINT:
                    return "BIGINT UNSIGNED";
                case TDengineDataType.TSDB_DATA_TYPE_FLOAT:
                    return "FLOAT";
                case TDengineDataType.TSDB_DATA_TYPE_DOUBLE:
                    return "DOUBLE";
                case TDengineDataType.TSDB_DATA_TYPE_BINARY:
                    return "STRING";
                case TDengineDataType.TSDB_DATA_TYPE_TIMESTAMP:
                    return "TIMESTAMP";
                case TDengineDataType.TSDB_DATA_TYPE_NCHAR:
                    return "NCHAR";
                default:
                    return "undefine";
            }
        }
    }
    internal class TDengine
    {


        public const int TSDB_CODE_SUCCESS = 0;

        [DllImport("taos", EntryPoint = "taos_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init();

        [DllImport("taos", EntryPoint = "taos_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Options(int option, string value);

        [DllImport("taos", EntryPoint = "taos_connect", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Connect(string ip, string user, string password, string db, short port);

        [DllImport("taos", EntryPoint = "taos_errstr", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr taos_errstr(IntPtr res);

        public static string Error(IntPtr conn)
        {
            IntPtr errPtr = taos_errstr(conn);
            return Marshal.PtrToStringAnsi(errPtr);
        }

        [DllImport("taos", EntryPoint = "taos_errno", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ErrorNo(IntPtr taos);

        [DllImport("taos", EntryPoint = "taos_query", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Query(IntPtr taos, string sqlstr);

        [DllImport("taos", EntryPoint = "taos_stop_query", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopQuery(IntPtr taos);

        [DllImport("taos", EntryPoint = "taos_affected_rows", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AffectRows(IntPtr res);

        [DllImport("taos", EntryPoint = "taos_field_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FieldCount(IntPtr res);

        [DllImport("taos", EntryPoint = "taos_fetch_fields", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr taos_fetch_fields(IntPtr res);

        public static List<TDengineMeta> FetchFields(IntPtr res)
        {
            const int fieldSize = 68;

            List<TDengineMeta> metas = new List<TDengineMeta>();

            int fieldCount = FieldCount(res);
            IntPtr fieldsPtr = taos_fetch_fields(res);

            for (int i = 0; i < fieldCount; ++i)
            {
                int offset = i * fieldSize;
#if NET45
                taosField field = (taosField)Marshal.PtrToStructure(fieldsPtr + offset,typeof(taosField));
#else 
                taosField field = Marshal.PtrToStructure<taosField>(fieldsPtr + offset);
#endif 
                TDengineMeta meta = new TDengineMeta() { name = Encoding.Default.GetString(field.name)?.TrimEnd('\0'), size = field.bytes, type = field.type };
                metas.Add(meta);
            }

            return metas;
        }

        [DllImport("taos", EntryPoint = "taos_fetch_row", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FetchRows(IntPtr res);

        [DllImport("taos", EntryPoint = "taos_free_result", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FreeResult(IntPtr res);

        [DllImport("taos", EntryPoint = "taos_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Close(IntPtr taos);

        [DllImport("taos", EntryPoint = "taos_cleanup", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup();

        [DllImport("taos", EntryPoint = "taos_get_client_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetClientInfo();

        [DllImport("taos", EntryPoint = "taos_get_server_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetServerInfo(TAOS taos);

        [DllImport("taos", EntryPoint = "taos_select_db", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SelectDatabase(TAOS taos, string db);

        [DllImport("taos", EntryPoint = "taos_result_precision", CallingConvention = CallingConvention.Cdecl)]
        public static extern  int ResultPrecision(TAOS taos);

        [DllImport("taos", EntryPoint = "taos_is_null", CallingConvention = CallingConvention.Cdecl)]
        public  static extern  bool IsNull(TAOS_RES res,int row,int col );

        [DllImport("taos", EntryPoint = "taos_validate_sql", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ValidateSQL(TAOS taos,  string sql);

        [DllImport("taos", EntryPoint = "taos_num_fields", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NumFields(TAOS_RES res);

    }
}