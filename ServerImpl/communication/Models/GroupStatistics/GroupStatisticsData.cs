using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.GroupStatistics
{
    public class GroupStatisticsData
    {
        public string message { get; set; }
        public List<Tuple<string, int, int, int,int>> list { get; set; }

    }
}