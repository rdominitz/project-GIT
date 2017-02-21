using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.GetTest
{
    public class GetTestData
    {
        public string key;
        public List<string> vals;

        public GetTestData(string _key, List<string> _vals)
        {
            key = _key;
            vals = _vals;
        }
    }
}