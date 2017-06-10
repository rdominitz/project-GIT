using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.TestStatistics
{
    public class TestStatisticsData
    {
        public List<Tuple<string, int>> gradesInTest { get; set; }
        public Dictionary<string, double> rangeCount { get; set; }

    }
}