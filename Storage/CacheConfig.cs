using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Inec.StateMachine.Storage
{
    public static class CacheConfig
    {
        public static int ExpireTime
        {
            get
            {
                var tmp = ConfigurationManager.AppSettings["ExpireTime"];
                if (string.IsNullOrEmpty(tmp))
                    return 60;
                int res;
                if (int.TryParse(tmp, out res))
                    return res;
                return 60;
            }
        }
        public static int RefreshCache
        {
            get
            {
                var tmp = ConfigurationManager.AppSettings["RefreshCache"];
                if (string.IsNullOrEmpty(tmp))
                    return 10;
                int res;
                if (int.TryParse(tmp, out res))
                    return res;
                return 10;
            }
        }
    }
}
