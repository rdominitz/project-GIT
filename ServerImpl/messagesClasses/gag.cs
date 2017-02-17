using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messagesClasses
{
    public class gag
    {
       public string subject { get; set; }
        public string topic { get; set; }
        public int numOfQuestions { get; set; }
        public bool ansAfterEachQuestion { get; set; }

        public bool isDataReasonableForQuestion()
        {
            return ((subject != null) && (subject != "") && (topic != null) && (topic != ""));
        }

        public bool isDataReasonableForTest()
        {
            return ((subject != null) && (subject != "") && (topic != null) && (topic != "") && (numOfQuestions > 0));
        }
    }
}
