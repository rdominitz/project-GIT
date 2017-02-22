﻿using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QALogic
{
    public class LogicImpl : ILogic
    {
        private IMedTrainDBContext _db;
        private const double NORMAL_PROBABILITY = 0.7;

        public LogicImpl(IMedTrainDBContext db)
        {
            _db = db;
        }

        public Tuple<string, List<Question>> getAutoGeneratedTest(string subject, Topic topic, User u, int numOfQuestions)
        {
            // get all questions with a matching topic and subject
            List<Question> questions = _db.getQuestions(subject, topic.TopicId);
            List<Question> normalQuestions = _db.getNormalQuestions(subject);
            // if there is no question matching the given subject and topic
            if (questions.Count + normalQuestions.Count < numOfQuestions)
            {
                return new Tuple<string, List<Question>>("Error - the provided subject and topic does not have enough questions.", null);
            }
            // select the appropriate amount of random questions
            UserLevel uLvl = _db.getUserLevel(u.UserId, subject, topic.TopicId);
            if (uLvl == null)
            {
                uLvl = new UserLevel { userId = u.UserId, user = u, level = Levels.DEFAULT_LVL, SubjectId = subject, TopicId = topic.TopicId, Topic  = topic, timesAnswered = 0, timesAnsweredCorrectly = 0};
            }
            List<Question> chooseFrom = new List<Question>();
            Random rnd = new Random();
            int subLevels = 0;
            int addLevels = 0;
            while (chooseFrom.Count < numOfQuestions)
            {
                List<Question> select = questions;
                if (rnd.NextDouble() < NORMAL_PROBABILITY || questions.Count == 0)
                {
                    select = normalQuestions;
                }
                if (normalQuestions.Count == 0)
                {
                    select = questions;
                }
                foreach (Question q in select)
                {
                    if (q.level == uLvl.level - subLevels || q.level == uLvl.level + addLevels)
                    {
                        chooseFrom.Add(q);
                    }
                }
                subLevels++;
                addLevels++;
            }
            List<Question> test = new List<Question>();
            for (int i = 0; i < numOfQuestions; i++)
            {
                // should see if there are questions of the relevant level
                // if so, prefer those the user did not answer
                // if answered all of them, select from those answered over a week ago
                // if none apply, increase level range from [a,b] to [a-1,b+1]
                Question q = selectRandomObject<Question>(chooseFrom);
                chooseFrom.Remove(q);
                test.Add(q);
            }
            return new Tuple<string, List<Question>>(Replies.SUCCESS, test);
        }

        public string answerAQuestion(User u, Question q, bool isNormal, int normalityCertaintyLVL, List<string> diagnoses, List<int> diagnosisCertainties)
        {
            bool correctAnswer = isNormal == q.normal;
            List<Tuple<string, int>> userLevels = new List<Tuple<string, int>>();
            foreach (Topic s in q.diagnoses)
            {
                UserLevel userLevel = _db.getUserLevel(u.UserId, q.subjectName, s.TopicId);
                bool alreadtExist = userLevel != null;
                if (userLevel == null)
                {
                    userLevel = new UserLevel { userId = u.UserId, SubjectId = q.subjectName, TopicId = s.TopicId, level = Levels.DEFAULT_LVL, timesAnswered = 0, timesAnsweredCorrectly = 0, user = u, Topic = s };
                }
                userLevels.Add(new Tuple<string, int>(s.TopicId, userLevel.level));
                userLevel.timesAnswered++;
                if (!isNormal && diagnoses.Contains(s.TopicId))
                {
                    userLevel.timesAnsweredCorrectly++;
                }
                else if (!isNormal)
                {
                    correctAnswer = false;
                }
                if (alreadtExist)
                {
                    _db.updateUserLevel(userLevel);
                }
                else
                {
                    _db.addUserLevel(userLevel);
                }
            }
            // create a new answer instance and save to DB
            ICollection<Tuple<string, int>> diagnosesCertaintyLVL = new List<Tuple<string, int>>();
            for (int i = 0; !isNormal && i < diagnoses.Count; i++)
            {
                diagnosesCertaintyLVL.Add(new Tuple<string, int>(diagnoses.ElementAt(i), diagnosisCertainties.ElementAt(0)));
            }
            Answer a = new Answer
            {
                questionId = q.QuestionId,
                userId = u.UserId,
                timeAdded = DateTime.Now,
                questionLevel = q.level,
                userLevelPerTopic = userLevels,
                isCorrectAnswer = correctAnswer,
                normal = isNormal,
                normalityCertainty = normalityCertaintyLVL,
                diagnosesAndCertaintyLevels = null,
                user = u,
                question = q
            };
            _db.addAnswer(a);
            // update question's fields
            q.timesAnswered++;
            if (correctAnswer)
            {
                q.timesAnsweredCorrectly++;
            }
            // update users level's fields
            _db.updateQuestion(q);
            _db.SaveChanges();
            return Replies.SUCCESS;
            // update user and question level if needed
        }

        private T selectRandomObject<T>(List<T> l)
        {
            Random rnd = new Random();
            return l.ElementAt(rnd.Next(l.Count));
        }
    }
}
