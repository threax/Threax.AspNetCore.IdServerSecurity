using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.AuthCore
{
    public static class CookieUtils
    {
        /// <summary>
        /// This function will fix the incoming path to be a valid cookie path. It can take any
        /// kind of input, even null, and will give a correct cookie path result.
        /// </summary>
        /// <param name="cookiePath">The path to fix.</param>
        /// <returns>A cookie path that is valid.</returns>
        public static String FixPath(String cookiePath)
        {
            //Check the cookie path format
            if (!String.IsNullOrEmpty(cookiePath))
            {
                cookiePath = cookiePath.Replace('\\', '/');
                if (cookiePath[0] != '/')
                {
                    cookiePath = '/' + cookiePath;
                }
                int lastIndex = cookiePath.Length - 1;
                if (cookiePath[lastIndex] == '/')
                {
                    cookiePath = cookiePath.Substring(0, lastIndex);
                }
            }
            else
            {
                cookiePath = "/";
            }

            //Set back on the options
            return cookiePath;
        }
    }
}
