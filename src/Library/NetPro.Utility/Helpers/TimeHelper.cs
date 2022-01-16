using System.Runtime.InteropServices;
using TimeZoneConverter;

namespace System.NetPro
{
    /// <summary>
    /// 时间操作
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// 服务器时区ID(兼容linux与windows时区格式)
        /// </summary>
        private static string ServerZoneId
        {
            get
            {
                return GetWindowsZoneIdToOtherZoneByPlatform("China Standard Time");
            }
        }

        /// <summary>
        /// 获取windows时区转换为其他时区依据操作系统平台
        /// </summary>
        /// <param name="windowsTimeZoneId"></param>
        /// <returns></returns>
        public static string GetWindowsZoneIdToOtherZoneByPlatform(string windowsTimeZoneId = "China Standard Time")
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return TZConvert.WindowsToIana(windowsTimeZoneId?.TrimEnd());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return windowsTimeZoneId;
            }
            else
            {
                return "无法识别操作系统";
            }
        }

        /// <summary>
        /// 日期
        /// </summary>
        private static DateTime? _dateTime;

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="dateTime">时间</param>
        public static void SetTime(DateTime? dateTime)
        {
            _dateTime = dateTime;
        }

        /// <summary>
        /// 重置时间
        /// </summary>
        public static void Reset()
        {
            _dateTime = null;
        }

        /// <summary>
        /// 获取当前日期时间
        /// </summary>
        public static DateTime GetDateTime()
        {
            if (_dateTime == null)
                return DateTime.Now;
            return _dateTime.Value;
        }

        /// <summary>
        /// 获取当前日期,不带时间
        /// </summary>
        public static DateTime GetDate()
        {
            return GetDateTime().Date;
        }

        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return GetUnixTimestamp(DateTime.Now);
        }

        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        /// <param name="time">时间</param>
        public static long GetUnixTimestamp(DateTime time)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long ticks = (time - start.Add(new TimeSpan(8, 0, 0))).Ticks;
            return ConvertHelper.ToLong(ticks / TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// 从Unix时间戳获取时间
        /// </summary>
        /// <param name="timestamp">Unix时间戳</param>
        public static DateTime GetTimeFromUnixTimestamp(long timestamp)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            TimeSpan span = new TimeSpan(long.Parse(timestamp + "0000000"));
            return start.Add(span).Add(new TimeSpan(8, 0, 0));
        }

        /// <summary>
        /// 毫秒转string
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string ConvertMilliSecondToString(long ms)
        {
            long h = 0;
            long m = 0;
            long s = 0;
            if (ms > 3600000)
            {
                h = ms / 3600000;
                m = (ms - 3600000 * h) / 60000;
                s = (ms - 3600000 * h - 60000 * m) / 1000;
            }
            else if (ms > 60000)
            {
                m = ms / 60000;
                s = (ms - 60000 * m) / 1000;
            }
            else
            {
                s = ms / 1000;
            }

            string hh = h < 10 ? "0" + h : h.ToString();
            string mm = m < 10 ? "0" + m : m.ToString();
            string ss = s < 10 ? "0" + s : s.ToString();

            string cn = string.Format("{0}:{1}:{2}", hh, mm, ss);
            return cn;
        }

        /// <summary>
        /// 服务器时间转换为用户时间
        /// 内部会对UserZoneID依据平台自动转换为对应平台Id
        /// </summary>
        /// <param name="ServerTime">服务器时间</param>
        /// <param name="UserZoneID">用户时区ID(微软的提供的ID)必须windows的时区Id</param>
        /// <returns>用户所在时区时间</returns>
        public static DateTime ConverServerSaveTimeToUser(DateTime ServerTime, string UserZoneID)
        {
            if (string.IsNullOrEmpty(UserZoneID))
                return ServerTime;

            UserZoneID = GetWindowsZoneIdToOtherZoneByPlatform(UserZoneID);
            ServerTime = DateTime.SpecifyKind(ServerTime, DateTimeKind.Unspecified);
            try
            {
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(ServerTime, ServerZoneId.Trim(), UserZoneID.Trim());
            }
            catch
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(UserZoneID.Trim());
                return ServerTime.AddHours(timeZoneInfo.BaseUtcOffset.Hours);
            }
        }

        /// <summary>
        /// 用户时间转换为服务器时间
        /// 内部会对UserZoneID依据平台自动转换为对应平台Id
        /// </summary>
        /// <param name="UserTime">用户时间</param>
        /// <param name="UserZoneID">用户时区ID(微软的提供的ID)必须windows的时区Id</param>
        /// <returns>服务器所在时区时间</returns>
        public static DateTime ConverUserTimeToServer(DateTime UserTime, string UserZoneID)
        {
            if (UserZoneID == null)
                return UserTime;

            UserZoneID = GetWindowsZoneIdToOtherZoneByPlatform(UserZoneID);
            UserTime = DateTime.SpecifyKind(UserTime, DateTimeKind.Unspecified);
            try
            {
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(UserTime, UserZoneID.Trim(), ServerZoneId.Trim());
            }
            catch
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(UserZoneID.Trim());
                return UserTime.AddHours(timeZoneInfo.BaseUtcOffset.Hours);
            }
        }

        /// <summary>
        /// 计算本周起始日期（礼拜一的日期）
        /// </summary>
        /// <param name="someDate">该周中任意一天</param>
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns>
        public static DateTime CalculateFirstDateOfWeek(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }

        /// <summary>
        /// 计算本周结束日期（礼拜日的日期）
        /// </summary>
        /// <param name="someDate">该周中任意一天</param>
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns>
        public static DateTime CalculateLastDateOfWeek(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }

        /// <summary>
        /// 判断选择的日期是否是本周（根据系统当前时间决定的‘本周’比较而言）
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static bool IsThisWeek(DateTime someDate)
        {
            //得到someDate对应的周一
            DateTime someMon = CalculateFirstDateOfWeek(someDate);
            //得到本周一
            DateTime nowMon = CalculateFirstDateOfWeek(DateTime.Now);

            TimeSpan ts = someMon - nowMon;
            if (ts.Days < 0)
                ts = -ts;//取正
            if (ts.Days >= 7)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 获取该年中是第几周
        /// </summary>
        /// <param name="day">日期</param>
        /// <returns></returns>
        public static int WeekOfYear(System.DateTime day)
        {
            int weeknum;
            System.DateTime fDt = DateTime.Parse(day.Year.ToString() + "-01-01");
            int k = Convert.ToInt32(fDt.DayOfWeek);//得到该年的第一天是周几 
            if (k == 0)
            {
                k = 7;
            }
            int l = Convert.ToInt32(day.DayOfYear);//得到当天是该年的第几天 
            l = l - (7 - k + 1);
            if (l <= 0)
            {
                weeknum = 1;
            }
            else
            {
                if (l % 7 == 0)
                {
                    weeknum = l / 7 + 1;
                }
                else
                {
                    weeknum = l / 7 + 2;//不能整除的时候要加上前面的一周和后面的一周 
                }
            }
            return weeknum;
        }

        #region 
        /// <summary>
        /// 验证时间是否为指定的格式
        /// </summary>
        /// <param name="time">待验证的时间</param>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public static bool IsValidFormat(DateTime time, string format)
        {
            try
            {
                if (time == DateTime.Parse(time.ToString(format)))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        #endregion

        /// <summary>
        /// 得到当前时区 1970.1.1 到现在的所有秒
        /// </summary>
        /// <returns></returns>
        public static long GetUnixSeconds()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 得到指定时区 1970.1.1 到现在的所有秒
        /// </summary>
        /// <returns></returns>
        public static long GetUnixSeconds(TimeZoneInfo zoneInfo)
        {
            DateTime specifiedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, zoneInfo);
            DateTimeOffset timeoffset = new DateTimeOffset(specifiedTime);
            return timeoffset.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 当前时间转换为指定时区的时间
        /// </summary>
        /// <param name="localTime"></param>
        /// <param name="zoneInfo"></param>
        /// <returns></returns>
        public static DateTime ZoneTime(DateTime localTime, TimeZoneInfo zoneInfo)
        {
            DateTime specifiedTime = TimeZoneInfo.ConvertTimeFromUtc(localTime, zoneInfo);
            return specifiedTime;
        }
    }
}