using communication.Core;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace communication.Models.Questions
{
    public class QuestionData
    {
      public List<string> pics;
      public List<string> dignosis;

      public QuestionData(Question q)
        {
            dignosis = ServerWiring.getInstance().getQuestionDiagnoses(q.QuestionId);
            pics = ServerWiring.getInstance().getQuestionImages(q.QuestionId); 
        }
    }
}