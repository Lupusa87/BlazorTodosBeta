using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TodosCosmos.ClientClasses
{
    public static class PartitionKeyGenerator
    {
        //09.07.2019
        //http://www.anderbakk.com/designing-a-partition-key-for-cosmos-db/

        //https://docs.microsoft.com/en-us/azure/cosmos-db/synthetic-partition-keys

        private static readonly MD5 _md5 = MD5.Create();

        //public PartitionKeyGenerator()
        //{
        //    _md5 = MD5.Create();
        //}
        public static string Create(string prefix, string id, int numberOfPartitions = 400)
        {
            var hashedValue = _md5.ComputeHash(Encoding.UTF8.GetBytes(id.Substring(0, 8)));
            var asInt = BitConverter.ToInt32(hashedValue, 0);
            asInt = asInt == int.MinValue ? asInt + 1 : asInt;
            return prefix + "." + (Math.Abs(asInt) % numberOfPartitions);
        }
    }
}
