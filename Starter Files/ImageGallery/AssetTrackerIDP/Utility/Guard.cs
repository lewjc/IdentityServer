using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP.Utility
{
    public static class Guard
    {
    
        /// <summary>
        /// Object Null Check
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Object cannot be null check
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static bool ErrorIfNull(object obj, string paramName)
        {

            return obj != null ? true : throw new ArgumentNullException($"{paramName} Cannot be Null");
        }


    }


}
