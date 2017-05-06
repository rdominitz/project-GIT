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
        public string subject;


        public TestData(int _testId, string _subject, string _testString)
        {
            testId = _testId;
            subject = _subject;
            testString = _testString;
        }
    }
}