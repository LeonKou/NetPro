using System.Runtime.InteropServices;

namespace Leon.XXXV2.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class XXXAo
    {
        /// <summary>
        /// 这是Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 这是Type
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 结构体属性（一字节对齐），不写的话默认4字节对齐
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ControlData
    {
        public byte type;
        /// <summary>
        /// 定义字符串时需定义字符串的长度，SizeConst为字符串的长度
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string name;

        /// <summary>
        /// C#结构体中的数组必须规定他的大小，SizeConst数组大小
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] data1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] data2;

        public int testInt;

        public string testString;

        /// <summary>
        /// 结构体数组需加上下边这句，SizeConst为大小
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
        public PointStruct[] pos;
    }


    public struct PointStruct
    {
        public byte pointId;
        public float Longtitude;
        public float latitude;
    }
}
