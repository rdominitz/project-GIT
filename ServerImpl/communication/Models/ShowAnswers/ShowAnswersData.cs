using communication.Core;
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
            dignosis = ServerWiring.getInstance().getQuestionDiagnoses(q.QuestionId);
            pics = ServerWiring.getInstance().getQuestionImages(q.QuestionId); 
            
          
            //question = q;
            /*foreach(Topic t in q.diagnoses)
            {
                dignosis.Add(t.TopicId);
            }
            foreach (Image i in q.images)
            {
                pics.Add(i.ImageId);
            }*/

        }
    }
}