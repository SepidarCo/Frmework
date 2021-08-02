using Sepidar.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public static class CacheHelper
    {
        private static object lockObject = new object();
        private static DateTime _lastUpdateDateTime = DateTime.Now;
        private static object lockToken = new object();

        private static bool DataNeedsToBeUpdated(IList list)
        {
            if (list.IsNull())
            {
                return true;
            }
            if (list.Count == 0)
            {
                return true;
            }
            if ((DateTime.Now - _lastUpdateDateTime).TotalMinutes > Config.CacheUpdateTimeInMinutes)
            {
                return true;
            }
            return false;
        }

        public static void InitiliazeData<T>(List<T> list, Func<List<T>> dataLoader)
        {
            lock (lockToken)
            {
                if (DataNeedsToBeUpdated(list))
                {
                    var typeFullName = typeof(T).FullName;
                    Logger.LogInfo("Reading {0} information into memory...".Fill(typeFullName));
                    try
                    {
                        list = dataLoader();
                    }
                    catch (Exception ex)
                    {
                       Logger.LogError("Exception in reading {0} from database".Fill(typeFullName));
                       Logger.Log(ex);
                    }
                    _lastUpdateDateTime = DateTime.Now;
                    Logger.LogInfo("Read {0} items of type {1} into memory.".Fill(list.Count, typeFullName));
                }
            }
        }
    }
}
