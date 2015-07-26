using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public static class Definitions
	{
		public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long UnixMillisecondTimestamp => (long)(DateTimeOffset.UtcNow - UnixEpoch).TotalMilliseconds;
        public static long UnixTimestamp => (long)(DateTimeOffset.UtcNow - UnixEpoch).TotalSeconds;
    }
}
