using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.ShowAnswers
{
    public class ShowAnswersData
    {
        //Question question;
        public List<string> pics;
        public List<string> dignosis;
        

        public ShowAnswersData(Question q)
        {
            dignosis = new List<string>();
            pics = new List<string>();
            //question = q;
            foreach(Topic t in q.diagnoses)
            {
                dignosis.Add(t.TopicId);
            }
            foreach(string p in q.images)
            {
                pics.Add(p);
            }
        }
    }
}