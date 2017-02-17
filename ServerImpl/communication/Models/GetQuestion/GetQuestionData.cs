using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.GetQuestion
{
    public class GetQuestionData  
    {
            public string key;
            public List<string> vals;
        
        public GetQuestionData(string _key, List<string> _vals)
        {
            key = _key;
            vals = _vals;

        }

        

        
    }
}