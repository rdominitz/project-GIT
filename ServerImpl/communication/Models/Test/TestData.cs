using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.Test
{
    public class TestData
    {
        public string testString;
        public int testId;


        public TestData(int _testId, string _testString)
        {
            testId = _testId;
            testString = _testString;
        }
    }
}