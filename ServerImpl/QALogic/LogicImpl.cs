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

        private int _answerId;
        private readonly object syncLockAnswerId;

        public LogicImpl(IMedTrainDBContext db)
        {
            _db = db;
            _answerId = 1;
            syncLockAnswerId = new object();
        }

        public Tuple<string, List<Question>> getAutoGeneratedTest(string subject, List<Topic> topics, User u, int numOfQuestions)
        {
            List<Question> questions = new List<Question>();
            int avgLevel = 0;
            foreach (Topic topic in topics)
            {
                UserLevel uLvl = _db.getUserLevel(u.UserId, subject, topic.TopicId);
                if (uLvl == null)
                {
                    uLvl = new UserLevel { UserId = u.UserId, level = Levels.DEFAULT_LVL, SubjectId = subject, TopicId = topic.TopicId, timesAnswered = 0, timesAnsweredCorrectly = 0 };
                }
                avgLevel += uLvl.level;
                questions.AddRange(_db.getQuestions(subject, topic.TopicId));
            }
            avgLevel /= topics.Count;
            /*
            UserLevel uLvl = _db.getUserLevel(u.UserId, subject, topic.TopicId);
            if (uLvl == null)
            {
                uLvl = new UserLevel { UserId = u.UserId, level = Levels.DEFAULT_LVL, SubjectId = subject, TopicId = topic.TopicId, timesAnswered = 0, timesAnsweredCorrectly = 0 };
            }
            // get all questions with a matching topic and subject
            List<Question> questions = _db.getQuestions(subject, topic.TopicId);
            */
            List<Question> normalQuestions = _db.getNormalQuestions(subject);
            // if there is no question matching the given subject and topic
            if (questions.Count + normalQuestions.Count < numOfQuestions)
            {
                return new Tuple<string, List<Question>>("Error. The provided subject and topic(s) does not have enough questions.", null);
            }
            // select the appropriate amount of random questions

            List<Question> chooseFrom = new List<Question>();
            List<Question> normal = new List<Question>();
            List<Question> abnormal = new List<Question>();
            int levelDiff = 0;
            while (normal.Count < numOfQuestions * NORMAL_PROBABILITY && normalQuestions.Count != 0)
            {
                foreach (Question q in normalQuestions)
                {
                    if (q.level == avgLevel/*uLvl.level*/ - levelDiff || q.level == avgLevel/*uLvl.level*/ + levelDiff)
                    {
                        normal.Add(q);
                        chooseFrom.Add(q);
                    }
                }
                foreach (Question q in normal)
                {
                    normalQuestions.Remove(q);
                }
                levelDiff++;
            }
            int temp = levelDiff;
            levelDiff = 0;
            while (abnormal.Count < numOfQuestions * (1 - NORMAL_PROBABILITY) && questions.Count != 0)
            {
                foreach (Question q in questions)
                {
                    if (q.level == avgLevel/*uLvl.level*/ - levelDiff || q.level == avgLevel/*uLvl.level*/ + levelDiff)
                    {
                        abnormal.Add(q);
                        chooseFrom.Add(q);
                    }
                }
                foreach (Question q in abnormal)
                {
                    questions.Remove(q);
                }
                levelDiff++;
            }
            int temp2 = levelDiff;
            levelDiff = temp;
            while (normal.Count + abnormal.Count < numOfQuestions && normalQuestions.Count != 0)
            {
                foreach (Question q in normalQuestions)
                {
                    if (q.level == avgLevel/*uLvl.level*/ - levelDiff || q.level == avgLevel/*uLvl.level*/ + levelDiff)
                    {
                        normal.Add(q);
                        chooseFrom.Add(q);
                    }
                }
                foreach (Question q in normal)
                {
                    normalQuestions.Remove(q);
                }
                levelDiff++;
            }
            levelDiff = temp2;
            while (normal.Count + abnormal.Count < numOfQuestions && questions.Count != 0)
            {
                foreach (Question q in questions)
                {
                    if (q.level == avgLevel/*uLvl.level*/ - levelDiff || q.level == avgLevel/*uLvl.level*/ + levelDiff)
                    {
                        abnormal.Add(q);
                        chooseFrom.Add(q);
                    }
                }
                foreach (Question q in abnormal)
                {
                    questions.Remove(q);
                }
                levelDiff++;
            }
            List<Question> test = new List<Question>();
            for (int i = 0; i < numOfQuestions; i++)
            {
                Question q = selectRandomObject<Question>(chooseFrom);
                chooseFrom.Remove(q);
                test.Add(q);
            }
            return new Tuple<string, List<Question>>(Replies.SUCCESS, test);
        }

        public Tuple<string, int> answerAQuestion(User u, Question q, bool isNormal, int normalityCertaintyLVL, List<string> diagnoses, List<int> certainties)
        {
            bool correctAnswer = true;
            Dictionary<string, int> userLevels = new Dictionary<string, int>();
            // get all correct diagnoses
            List<Diagnosis> qDiagnoses = _db.getQuestionDiagnoses(q.QuestionId);
            List<string> qDiagnosesNames = new List<string>();
            foreach (Diagnosis d in qDiagnoses)
            {
                qDiagnosesNames.Add(d.TopicId);
            }
            foreach (string s in diagnoses)
            {
                #region get or create user level for each diagnosis and update fields according to the answer
                UserLevel userLevel = _db.getUserLevel(u.UserId, q.SubjectId, s);
                bool alreadtExist = userLevel != null;
                if (userLevel == null)
                {
                    userLevel = new UserLevel { UserId = u.UserId, SubjectId = q.SubjectId, TopicId = s, level = Levels.DEFAULT_LVL, timesAnswered = 0, timesAnsweredCorrectly = 0 };
                }
                userLevels[s] = userLevel.level;
                userLevel.timesAnswered++;
                if (qDiagnosesNames.Contains(s))
                {
                    userLevel.timesAnsweredCorrectly++;
                }
                else
                {
                    correctAnswer = false;
                }
                if (alreadtExist)
                {
                    if (userLevel.timesAnswered >= UsersLevels.MIN_ANSWERS_BEFORE_LEVEL_CHANGE)
                    {
                        if (userLevel.level < Levels.MAX_LVL && userLevel.timesAnsweredCorrectly / userLevel.timesAnswered >= UsersLevels.LEVEL_UP_SUCCESS_RATE)
                        {
                            userLevel.level++;
                            userLevel.timesAnsweredCorrectly = 0;
                            userLevel.timesAnswered = 0;
                        }
                        if (userLevel.level > Levels.MIN_LVL && userLevel.timesAnsweredCorrectly / userLevel.timesAnswered <= UsersLevels.LEVEL_DOWN_SUCCESS_RATE)
                        {
                            userLevel.level--;
                            userLevel.timesAnsweredCorrectly = 0;
                            userLevel.timesAnswered = 0;
                        }
                    }
                    _db.updateUserLevel(userLevel);
                }
                else
                {
                    _db.addUserLevel(userLevel);
                }
                #endregion
                // update question's fields according to each diagnosis
                double lvl = userLevel.level;
                if (qDiagnosesNames.Contains(s))
                {
                    q.points -= (Levels.MAX_LVL + 1 - lvl) / (double)qDiagnoses.Count;
                }
                else
                {
                    q.points += lvl / (double)qDiagnoses.Count;
                }
            }
            // update question's level if needed
            if (q.points <= 0 && q.level > Levels.MIN_LVL)
            {
                q.level--;
                q.points = Questions.QUESTION_INITAL_POINTS;
            }
            if (q.points >= Questions.QUESTION_INITAL_POINTS * 2 && q.level < Levels.MAX_LVL)
            {
                q.level++;
                q.points = Questions.QUESTION_INITAL_POINTS;
            }
            // create a new answer instance and save to DB
            Answer a = new Answer { };
            int answerId = -1;
            lock (syncLockAnswerId)
            {
                a.AnswerId = _answerId;
                a.QuestionId = q.QuestionId;
                a.UserId = u.UserId;
                a.timeAdded = DateTime.Now;
                a.questionLevel = q.level;
                a.isCorrectAnswer = correctAnswer;
                a.normal = isNormal;
                a.normalityCertainty = normalityCertaintyLVL;
                List<DiagnosisCertainty> diagnosesCertainties = new List<DiagnosisCertainty>();
                for (int i = 0; !isNormal && i < diagnoses.Count; i++)
                {
                    DiagnosisCertainty dc = new DiagnosisCertainty
                    {
                        AnswerId = _answerId,
                        SubjectId = q.SubjectId,
                        TopicId = diagnoses[i],
                        certainty = certainties[i],
                        userLevel = userLevels[diagnoses[i]]
                    };
                    _db.addDiagnosisCertainty(dc);
                }
                answerId = _answerId;
                _answerId++;
            }
            _db.addAnswer(a);
            _db.updateQuestion(q);
            _db.SaveChanges();
            return new Tuple<string,int>(Replies.SUCCESS, answerId);
        }

        private T selectRandomObject<T>(List<T> l)
        {
            Random rnd = new Random();
            return l.ElementAt(rnd.Next(l.Count));
        }
    }
}
