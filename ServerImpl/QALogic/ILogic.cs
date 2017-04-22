﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QALogic
{
    public interface ILogic
    {
        /// <summary>
        /// generate a series of questions based on requested subject and topic
        /// for each question there is a pre-defined probability, NORMAL_PROBABILITY, that the diagnosis should be normal
        /// the mentioned probability is only relevant for a large enough DB
        /// </summary>
        /// <param name="subject"> requested subject </param>
        /// <param name="topic"> requested topic </param>
        /// <param name="UserId"> user eMAil address </param>
        /// <param name="numOfQuestions"> number of questions to be returned </param>
        /// <returns>
        /// relevant error message if there are not enough questions (both normal and abnormal)
        /// success message and a list of numOfQuestions questions
        /// </returns>
        Tuple<string, List<Question>> getAutoGeneratedTest(string subject, Topic topic, User u, int numOfQuestions);

        /// <summary>
        /// saves a user's answer to a specific question
        /// if the user does not have a level for a certain topic (diagnosis) create it
        /// update the user and/or question levels if necessary
        /// </summary>
        /// <param name="userId"> user eMail address </param>
        /// <param name="q"> the question </param>
        /// <param name="isNormal"> user's decision whether the question diagnosis is a normal condition or not </param>
        /// <param name="normalityCertainty"> the user's certainty level of the normality decision </param>
        /// <param name="diagnoses"> all of the user's diagnoses of the question </param>
        /// <param name="diagnosisCertainties"> user's certainty levels of each diagnosis by order or diagnoses </param>
        /// <returns>
        /// success message after saving the relevant data to the DB
        /// </returns>
        Tuple<string, int> answerAQuestion(User u, Question q, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties);
    }
}
