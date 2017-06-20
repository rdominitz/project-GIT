using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.Test
{
    public class TestData
    {
        public string testString;
        public int TestId;
        public string subject;


        public TestData(int _TestId, string _subject, string _testString)
        {
            TestId = _TestId;
            subject = _subject;
            testString = _testString;
        }
    }
}