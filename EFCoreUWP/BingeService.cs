using EFCoreUWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreUWP
{
    public class BingeService
    {
        public static void RecordBinge(int count, bool worthIt)
        {
            //TODO
            throw new NotImplementedException();
        }

        public static IEnumerable<CookieBinge> GetLast5Binges()
        {
            using (var context = new BingeContext())
            {
                var latestBinges = context.Binges
                    .OrderByDescending(b => b.TimeOccurred)
                    .Take(5).ToList();
                return latestBinges;
            }
        }

        public static void ClearHistory()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}