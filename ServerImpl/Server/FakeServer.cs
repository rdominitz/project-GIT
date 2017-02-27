﻿using Constants;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class FakeServer : IServer
    {
        public Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName)
        {
            throw new NotImplementedException();
        }

        public Tuple<string, int> login(string eMail, string password)
        {
            Tuple<string, int> t = null;
            try
            {
                if (eMail.Equals("user@gmail.com") && password.Equals("password"))
                {
                    t = new Tuple<string, int>(Replies.SUCCESS, Users.USER_UNIQUE_INT);
                }
            }
            catch
            {
                t = new Tuple<string, int>("Error", -1);
            }
            return t;
        }

        public string restorePassword(string eMail)
        {
            string s = "";
            try
            {
                if (eMail.Equals("user@gmail.com"))
                {
                    s = Replies.SUCCESS;
                }
            }
            catch
            {
                s = "Error";
            }
            return s;
        }

        public string AnswerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties)
        {
            throw new NotImplementedException();
        }

        public string getAutoGeneratedQuesstion(int userUniqueInt, string subject, string topic)
        {
            string s = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT && subject.Equals("subject") && topic.Equals("topic"))
                {
                    s = Replies.SUCCESS;
                }
            }
            catch
            {
                s = "Error";
            }
            return s;
        }

        public string getAutoGeneratedTest(int userUniqueInt, string subject, string topic, int numOfQuestions, bool answerEveryTime)
        {
            string s = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT && subject.Equals("subject") && topic.Equals("topic") && numOfQuestions > 0)
                {
                    s = Replies.SUCCESS;
                }
            }
            catch
            {
                s = "Error";
            }
            return s;
        }

        public Tuple<string, Question> getNextQuestion(int userUniqueInt)
        {
            if (userUniqueInt == Users.USER_UNIQUE_INT)
            {
                Topic t = new Topic { TopicId = "topic", SubjectId = "subject", timeAdded = DateTime.Now };
                Question q = new Question
                {
                    QuestionId = 1,
                    subjectName = "subject",
                    normal = true,
                    text = "",
                    level = Levels.DEFAULT_LVL,
                    diagnoses = new List<Topic>() { t },
                    timesAnswered = 0,
                    timesAnsweredCorrectly = 0,
                    timeAdded = DateTime.Now,
                    images = new List<Image>()
                };
                Image i = new Image
                {
                    ImageId = "../Images/q1_2_lat.jpg",
                    QuestionId = 1,
                };
                q.images.Add(i);
                return new Tuple<string,Question>(Replies.SUCCESS, q);
            }
            return new Tuple<string,Question>("Error", null);
        }

        public Tuple<string, List<Question>> getAnsweres(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public List<string> getAllSubjects()
        {
            throw new NotImplementedException();
        }

        public List<string> getSubjectTopics(string subject)
        {
            throw new NotImplementedException();
        }

        public string addSubject(int userUniqueInt, string subject)
        {
            throw new NotImplementedException();
        }

        public string addTopic(int userUniqueInt, string subject, string topic)
        {
            throw new NotImplementedException();
        }

        public string addQuestion(int userUniqueInt, string subject, bool isNormal, string text, List<string> qDiagnoses)
        {
            throw new NotImplementedException();
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            throw new NotImplementedException();
        }

        public bool hasMoreQuestions(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public void logout(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public bool isLoggedIn(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public string getUserName(int userUniqueInt)
        {
            throw new NotImplementedException();
        }
    }
}