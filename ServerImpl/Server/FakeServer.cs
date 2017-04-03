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
                Question q = new Question
                {
                    QuestionId = 1,
                    SubjectId = "subject",
                    normal = true,
                    text = "",
                    level = Levels.DEFAULT_LVL,
                    points = Questions.QUESTION_INITAL_POINTS,
                    timeAdded = DateTime.Now,
                };
                return new Tuple<string,Question>(Replies.SUCCESS, q);
            }
            return new Tuple<string,Question>("Error", null);
        }

        public Tuple<string, List<Question>> getAnsweres(int userUniqueInt)
        {
            if (userUniqueInt == Users.USER_UNIQUE_INT)
            {
                Question q = new Question
                {
                    QuestionId = 1,
                    SubjectId = "subject",
                    normal = true,
                    text = "",
                    level = Levels.DEFAULT_LVL,
                    points = Questions.QUESTION_INITAL_POINTS,
                    timeAdded = DateTime.Now,
                };
                return new Tuple<string, List<Question>>(Replies.SUCCESS, new List<Question>() { q });
            }
            return new Tuple<string, List<Question>>("Error", null);
        }

        public List<string> getAllSubjects()
        {
            return new List<string>() { "subject" };
        }

        public List<string> getSubjectTopics(string subject)
        {
            List<string> l = null;
            try
            {
                if (subject.Equals("subject"))
                {
                    l = new List<string>() { "topic" };
                }
            }
            catch
            {
                l = new List<string>();
            }
            return l;
        }

        public string addSubject(int userUniqueInt, string subject)
        {
            string s = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT && subject.Equals("fake subject"))
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

        public string addTopic(int userUniqueInt, string subject, string topic)
        {
            string s = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT && subject.Equals("subject") && topic.Equals("fake topic"))
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

        public string addQuestion(int userUniqueInt, string subject, bool isNormal, string text, List<string> qDiagnoses)
        {
            throw new NotImplementedException();
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            string s = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT && usernameToTurnToAdmin.Equals("user@gmail.com"))
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

        public bool hasMoreQuestions(int userUniqueInt)
        {
            return userUniqueInt == Users.USER_UNIQUE_INT;
        }

        public void logout(int userUniqueInt) { }

        public bool isLoggedIn(int userUniqueInt)
        {
            return userUniqueInt == Users.USER_UNIQUE_INT;
        }

        public string getUserName(int userUniqueInt)
        {
            return userUniqueInt == Users.USER_UNIQUE_INT ? "fake user" : null;
        }

        public List<string> getQuestionImages(int questionId)
        {
            return questionId == 1 ? new List<string>() { "../Images/q1_2_lat.jpg" } : null;
        }

        public List<string> getQuestionDiagnoses(int questionId)
        {
            return questionId == 1 ? new List<string>() { "topic" } : null;
        }

        public string createGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent)
        {
            throw new NotImplementedException();
        }

        public string inviteToGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent)
        {
            throw new NotImplementedException();
        }

        public Tuple<string, List<string>> getAllAdminsGroups(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public string removeGroup(int userUniqueInt, string groupName)
        {
            throw new NotImplementedException();
        }

        public Tuple<string, List<Question>> createTest(int userUniqueInt, string testName, string subject, List<string> topics)
        {
            throw new NotImplementedException();
        }

        public bool isAdmin(int userUniqueInt)
        {
            return userUniqueInt == Users.USER_UNIQUE_INT;
        }

        public Tuple<string, List<Test>> getAllTests(int userUniqueInt)
        {
            throw new NotImplementedException();
        }
        public string addTestToGroup(int userUniqueInt, string groupName, int testId)
        {
            throw new NotImplementedException();
        }
        public List<string> getUsersGroups(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public List<string> getUsersGroupsInvitations(int userUniqueInt)
        {
            throw new NotImplementedException();
        }

        public void acceptUsersGroupsInvitations(int userUniqueInt, List<string> groups)
        {
            throw new NotImplementedException();
        }

        public string saveSelectedGroup(int userUniqueInt, string groupName)
        {
            string s = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT && groupName.Equals("fake group"))
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

        public Tuple<string, string> getSavedGroup(int userUniqueInt)
        {
            string s = "";
            string s2 = "";
            try
            {
                if (userUniqueInt == Users.USER_UNIQUE_INT)
                {
                    s = Replies.SUCCESS;
                    s2 = "fake group";
                }
            }
            catch
            {
                s = "Error";
                s2 = null;
            }
            return new Tuple<string, string>(s, s2);
        }
    }
}
