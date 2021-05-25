using AutoMapper;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Leon.XXXV2.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class XXXService : IXXXService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rankRepository"></param>
        /// <param name="mapper"></param>
        public XXXService(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XXXAo GetList()
        {
            var controlData = new ControlData();
            controlData.name = "  ";
            controlData.testInt = 12;
            controlData.testString = "这是测试";
            //结构体转字节
            var byteObj = StructToBytes(controlData);

            var resultString = BytesToStringConverted(byteObj);
            //字节转结构体
            ControlData sd = (ControlData)BytesToStruct<ControlData>(byteObj);
            return new XXXAo();
            //return _mapper.Map<XXXAo>(xxxdo.First());
        }

        private string BytesToStringConverted(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetFalse(string name)
        {
            return true;
        }

        /// <summary>
        /// 结构体转数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] StructToBytes(object obj)
        {
            //获取结构大小
            int size = Marshal.SizeOf(obj);
            //创建指定大小的数组
            byte[] bytes = new byte[size];
            //分配 结构体大小的 非托管 内存空间
            IntPtr structptr = Marshal.AllocHGlobal(size);
            //将结构体从托管对象拷贝到非托管分配好的内存空间
            Marshal.StructureToPtr(obj, structptr, false);
            //从非托管内存空间拷贝到byte数组
            Marshal.Copy(structptr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structptr);
            //返回byte数组用于发送
            return bytes;
        }

        /// <summary>
        /// 数组转结构体
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object BytesToStruct<T>(byte[] bytes)
        {
            //得到结构体大小
            var type = typeof(T);
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                //接收到的字节数组 小于 结构体大小 
                Console.WriteLine("接收的数据出错");
                return null;
            }
            //分配目标结构体大小的非托管内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将接收的字节数组拷贝到分配好的非托管内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放非托管内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回目标结构体
            return obj;
        }
    }
}
