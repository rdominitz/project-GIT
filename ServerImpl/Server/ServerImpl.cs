﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Constants;
using System.Threading;
using QALogic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using DB;

namespace Server
{
    public class ServerImpl : IServer
    {
        private const string WEBSITE = "132.72.23.63/com/login";
        private const string GENERAL_INPUT_ERROR = "There is a null, the string \"null\" or an empty string as an input.";
        private const string INVALID_EMAIL = "Invalid email address.";
        private const string ILLEGAL_PASSWORD = "Illegal password. Password must be 5 to 15 characters long and consist of only letters and numbers.";
        private const string INVALID_TEMPORAL_PASSWORD = "Bad cookie. Could not identify user.";
        private const string NOT_LOGGED_IN = "User is not logged in.";
        private const string USER_NOT_REGISTERED = "User is not registered.";
        private const string NOT_AN_ADMIN = "Error. only an admin has the required permissions to perform this action.";
        private const string CERTAINTY_LEVEL_ERROR = "Error. Certainty levels must be between 1 to 10.";
        private const string NON_EXISTING_GROUP = "Error. You have not created a group with that name.";
        private const string INVALID_GROUP_NAME = "Error. Invalid group name.";
        private const string DB_FAULT = "Error. DB fault.";
        
        private const string NON_EXISTING_TEST = "Error. The requested test does not exist.";
        
        private const int USERS_CACHE_LIMIT = 1000;
        private const int HOURS_TO_LOGOUT = 1;
        private const int MILLISECONDS_TO_SLEEP = HOURS_TO_LOGOUT * 60 * 60 * 1000;
        private const int ERROR = -1;

        private Dictionary<User, DateTime> _loggedUsers;

        private Dictionary<User, List<Question>> _usersTestsAnswerEveryTime;
        private Dictionary<User, List<Question>> _usersTestsAnswersAtEndRemainingQuestions;
        private Dictionary<User, List<Question>> _usersTestsAnswersAtEndAnsweredQuestions;
        private Dictionary<string, Tuple<string, List<Question>>> _testQuestions;
        private Dictionary<User, Tuple<string, int>> _testDisplaying;

        private readonly object _syncLockGroup;

        private int _TestId;
        private readonly object _syncLockTestId;

        private Dictionary<string, List<string>> _subjectsTopics;

        private ILogic _logic;
        private IMedTrainDBContext _db;
        private SystemExtensions _se;
        private UsersManager _um;
        private QuestionsManager _qm;

        public ServerImpl(IMedTrainDBContext db)
        {
            _loggedUsers = new Dictionary<User, DateTime>();
            _usersTestsAnswerEveryTime = new Dictionary<User, List<Question>>();
            _usersTestsAnswersAtEndRemainingQuestions = new Dictionary<User, List<Question>>();
            _usersTestsAnswersAtEndAnsweredQuestions = new Dictionary<User, List<Question>>();
            _testQuestions = new Dictionary<string, Tuple<string, List<Question>>>();
            _testDisplaying = new Dictionary<User, Tuple<string, int>>();
            _db = db;

            _logic = new LogicImpl(db);
            _se = new SystemExtensions(db);
            _um = new UsersManager(db);
            _qm = new QuestionsManager(db);
            
            _syncLockGroup = new object();
            _TestId = 1;
            _syncLockTestId = new object();
            Thread.Sleep(_db.getMillisecondsToSleep());
            _subjectsTopics = new Dictionary<string, List<string>>();
            setSubjectsAndTopics();
            Thread removeUsersThread = new Thread(new ThreadStart(removeNonActiveUsers));
            removeUsersThread.Start();
            _um.register("defaultadmin@gmail.com", "password", Users.medicalTrainingLevels[0], "default", "admin");
            Admin a = new Admin { AdminId = "defaultadmin@gmail.com" };
            _um.register("aCohen@post.bgu.ac.il", "password", Users.medicalTrainingLevels[0], "a", "cohen");
            _db.addAdmin(a);
            Admin a1 = new Admin { AdminId = "aCohen@post.bgu.ac.il" };
            _db.addAdmin(a1);
            if (_db.getMillisecondsToSleep() != 0)
            {
                setDB();
            }
        }

        public void setDB()
        {
            Subject chestXRays = new Subject { SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addSubject(chestXRays);
            #region normal
            Topic cxrNormal = new Topic { TopicId = "Normal", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(cxrNormal);
            addQuestions(chestXRays, new List<Topic>() { cxrNormal }, new List<List<string>>()
            {
                new List<string>() { "../Images/q1_2_pa.png", "../Images/q1_2_lat.jpg" },
                new List<string>() { "../Images/q2_17_PA.png", "../Images/q2_17_Lat.png" },
                new List<string>() { "../Images/q3_19_PA.png", "../Images/q3_19_Lat.png" },
                new List<string>() { "../Images/q4_20_PA.png", "../Images/q4_20_Lat.png" },
                new List<string>() { "../Images/q5_21_PA.png", "../Images/q5_21_Lat.png" },
                new List<string>() { "../Images/q6_24_PA.png", "../Images/q6_24_Lat.png" },
                new List<string>() { "../Images/q7_30_PA.png", "../Images/q7_30_Lat.png" },
                new List<string>() { "../Images/q8_35_PA.png", "../Images/q8_35_Lat.png" },
                new List<string>() { "../Images/q9_37_PA.png", "../Images/q9_37_Lat.png" },
                new List<string>() { "../Images/q10_38_Lat.png", "../Images/q10_38_PA.png" },
            });
            #endregion
            #region cavitary lesion
            Topic cxrCavitaryLesion = new Topic { TopicId = "Cavitary Lesion", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(cxrCavitaryLesion);
            addQuestions(chestXRays, new List<Topic>() { cxrCavitaryLesion }, new List<List<string>>()
            {
                new List<string>() { "../Images/q11_9_PA.png" },
                new List<string>() { "../Images/q12_10_PA.png" },
                new List<string>() { "../Images/q13_26_PA.png" },
                new List<string>() { "../Images/q14_39_PA.png" }
            });
            #endregion
            #region interstitial opacities
            Topic cxrInterstitialOpacities = new Topic { TopicId = "Interstitial opacities", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(cxrInterstitialOpacities);
            addQuestions(chestXRays, new List<Topic>() { cxrInterstitialOpacities }, new List<List<string>>()
            {
                new List<string>() { "../Images/q15_34_Lat.png", "../Images/q15_34_PA.png" },
                new List<string>() { "../Images/q16_42_PA.png" },
                new List<string>() { "../Images/q17_43_PA.png" },
                new List<string>() { "../Images/q18_49_PA.png" }
            });
            #endregion
            #region left pleural effusion
            Topic cxrLeftPleuralEffusion = new Topic { TopicId = "Left Pleural Effusion", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(cxrLeftPleuralEffusion);
            addQuestions(chestXRays, new List<Topic>() { cxrLeftPleuralEffusion }, new List<List<string>>()
            {
                new List<string>() { "../Images/q19_1_PA.png", "../Images/q19_1_lat.png" },
                new List<string>() { "../Images/q20_3_PA.png" },
                new List<string>() { "../Images/q21_6_PA.png" },
                new List<string>() { "../Images/q22_50_PA.png" }
            });
            #endregion
            #region median sternotomy
            Topic cxrMedianSternotomy = new Topic { TopicId = "Median Sternotomy", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(cxrMedianSternotomy);
            addQuestions(chestXRays, new List<Topic>() { cxrMedianSternotomy }, new List<List<string>>()
            {
                new List<string>() { "../Images/q23_22_PA.png", "../Images/q23_22_Lat.png" },
                new List<string>() { "../Images/q24_33_PA.png", "../Images/q24_33_Lat.png" },
                new List<string>() { "../Images/q25_41_PA.png" },
                new List<string>() { "../Images/q26_50_PA.png" }
            });
            #endregion
            #region right middle lobe collapse
            Topic cxrRightMiddleLobeCollapse = new Topic { TopicId = "Right Middle Lobe Collapse", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(cxrRightMiddleLobeCollapse);
            addQuestions(chestXRays, new List<Topic>() { cxrRightMiddleLobeCollapse }, new List<List<string>>()
            {
                new List<string>() { "../Images/q27_4_PA.png" },
                new List<string>() { "../Images/q28_13_PA.png", "../Images/q28_13_Lat.png" },
                new List<string>() { "../Images/q29_44_PA.png" },
                new List<string>() { "../Images/q30_51_PA.png", "../Images/q30_51_Lat.png" }
            });
            #endregion
            // add more questions
            #region add user
            _um.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            #endregion
            _subjectsTopics["Chest x-Rays"] = new List<string>()
            {
                "Cavitary Lesion", "Interstitial opacities", "Left Pleural Effusion",
                "Median Sternotomy", "Right Middle Lobe Collapse"
            };
            #region add group
            Group g = new Group
            {
                AdminId = "aCohen@post.bgu.ac.il",
                name = "Cavitary Lesions"
            };
            _db.addGroup(g);
            #endregion
            /*********** Roie added code here*************/
            #region roie
            Group g2 = new Group
            {
                AdminId = "aCohen@post.bgu.ac.il",
                name = "Basic diagnosis"
            };
            _db.addGroup(g2);
            Group g3 = new Group
            {
                AdminId = "aCohen@post.bgu.ac.il",
                name = "Interstitial opacities"
            };
            _db.addGroup(g3);
            Group g4 = new Group
            {
                AdminId = "aCohen@post.bgu.ac.il",
                name = "Pleural Effusions"
            };
            _db.addGroup(g4);
            Group g5 = new Group
            {
                AdminId = "aCohen@post.bgu.ac.il",
                name = "Median Sternotomy"
            };
            _db.addGroup(g5);
            Group g6 = new Group
            {
                AdminId = "aCohen@post.bgu.ac.il",
                name = "Middle Lobe Collapses"
            };
            _db.addGroup(g6);

            GroupMember gm1 = new GroupMember
            {
                GroupName = "Cavitary Lesions",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm1);
            GroupMember gm2 = new GroupMember
            {
                GroupName = "Middle Lobe Collapses",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm2);
            GroupMember gm3 = new GroupMember
            {
                GroupName = "Median Sternotomy",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm3);
            GroupMember gm4 = new GroupMember
            {
                GroupName = "Pleural Effusions",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm4);
            GroupMember gm5 = new GroupMember
            {
                GroupName = "Interstitial opacities",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm5);
            GroupMember gm6 = new GroupMember
            {
                GroupName = "Basic diagnosis",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "user@gmail.com",
                invitationAccepted = true
            };
            _db.addGroupMember(gm6);
            #endregion
            /*********** Roie stopped adding code here*************/
            GroupMember gm7 = new GroupMember
            {
                GroupName = "Basic diagnosis",
                AdminId = "aCohen@post.bgu.ac.il",
                UserId = "defaultadmin@gmail.com",
                invitationAccepted = true
            };
            _db.addGroupMember(gm7);
            #region add test
            Test t = new Test { TestId = _TestId, AdminId = "aCohen@post.bgu.ac.il", testName = "Basic diagnosis - midterm", subject = chestXRays.SubjectId };
            _TestId++;
            _db.addTest(t);
            for (int i = 5; i <= 13; i++)
            {
                TestQuestion tq = new TestQuestion { TestId = 1, QuestionId = i };
                _db.addTestQuestion(tq);
            }
            GroupTest gt = new GroupTest{GroupName = "Basic diagnosis", AdminId = "aCohen@post.bgu.ac.il", TestId = 1};
            _db.addGroupTest(gt);
            Test t2 = new Test { TestId = _TestId, AdminId = "aCohen@post.bgu.ac.il", testName = "Basic diagnosis - final", subject = chestXRays.SubjectId };
            _TestId++;
            _db.addTest(t2);
            TestQuestion testq = new TestQuestion { TestId = 2, QuestionId = 1 };
            TestQuestion testq2 = new TestQuestion { TestId = 2, QuestionId = 2 };
            GroupTest gt2 = new GroupTest{GroupName = "Basic diagnosis", AdminId = "aCohen@post.bgu.ac.il", TestId = 2};
            _db.addGroupTest(gt2);
            _db.addTestQuestion(testq);
            _db.addTestQuestion(testq2);
            Answer a1 = new Answer
            {
                AnswerId = 1,
                isCorrectAnswer = true,
                normal = true,
                normalityCertainty = 4,
                QuestionId = 1,
                questionLevel = 5,
                timeAdded = DateTime.Now,
                UserId = "defaultadmin@gmail.com"
            };
            _db.addAnswer(a1);
            GroupTestAnswer gta = new GroupTestAnswer
            {
                AdminId = "aCohen@post.bgu.ac.il",
                AnswerId = 1,
                GroupName = "Basic diagnosis",
                TestId = 2,
                UserId = "defaultadmin@gmail.com"
            };
            _db.addGroupTestAnswer(gta);
            Answer a2 = new Answer
            {
                AnswerId = 2,
                isCorrectAnswer = true,
                normal = true,
                normalityCertainty = 4,
                QuestionId = 2,
                questionLevel = 5,
                timeAdded = DateTime.Now,
                UserId = "defaultadmin@gmail.com"
            };
            _db.addAnswer(a2);
            GroupTestAnswer gta2 = new GroupTestAnswer
            {
                AdminId = "aCohen@post.bgu.ac.il",
                AnswerId = 2,
                GroupName = "Basic diagnosis",
                TestId = 2,
                UserId = "defaultadmin@gmail.com"
            };
            _db.addGroupTestAnswer(gta2);
            // user@gmail.com
            Answer a3 = new Answer
            {
                AnswerId = 3,
                isCorrectAnswer = true,
                normal = true,
                normalityCertainty = 4,
                QuestionId = 1,
                questionLevel = 5,
                timeAdded = DateTime.Now,
                UserId = "user@gmail.com"
            };
            _db.addAnswer(a3);
            GroupTestAnswer gta3 = new GroupTestAnswer
            {
                AdminId = "aCohen@post.bgu.ac.il",
                AnswerId = 3,
                GroupName = "Basic diagnosis",
                TestId = 2,
                UserId = "user@gmail.com"
            };
            _db.addGroupTestAnswer(gta3);
            Answer a4 = new Answer
            {
                AnswerId = 4,
                isCorrectAnswer = false,
                normal = false,
                normalityCertainty = 4,
                QuestionId = 2,
                questionLevel = 5,
                timeAdded = DateTime.Now,
                UserId = "user@gmail.com"
            };
            _db.addAnswer(a4);
            DiagnosisCertainty dc = new DiagnosisCertainty
            {
                userLevel = 5,
                AnswerId = 4,
                SubjectId = chestXRays.SubjectId,
                TopicId = "Cavitary Lesion",
                certainty = 5
            };
            _db.addDiagnosisCertainty(dc);
            GroupTestAnswer gta4 = new GroupTestAnswer
            {
                AdminId = "aCohen@post.bgu.ac.il",
                AnswerId = 4,
                GroupName = "Basic diagnosis",
                TestId = 2,
                UserId = "user@gmail.com"
            };
            _db.addGroupTestAnswer(gta4);
            #endregion
        }

        private void addQuestions(Subject s, List<Topic> diagnoses, List<List<string>> images)
        {
            foreach (List<string> l in images)
            {
                _qm.addQuestion(s, diagnoses, l, "");
            }
        }

        private void setSubjectsAndTopics()
        {
            List<Subject> subjects = _db.getSubjects();
            foreach (Subject s in subjects)
            {
                List<Topic> topics = _db.getTopics(s.SubjectId);
                List<string> subjectTopics = new List<string>();
                foreach (Topic t in topics)
                {
                    subjectTopics.Add(t.TopicId);
                }
                _subjectsTopics[s.SubjectId] = subjectTopics;
            }
        }

        private void removeNonActiveUsers()
        {
            while (true)
            {
                Thread.Sleep(MILLISECONDS_TO_SLEEP);
                List<User> toBeRemoved = new List<User>();
                foreach (User u in _loggedUsers.Keys)
                {
                    if (DateTime.Now.Subtract(_loggedUsers[u]).Hours >= HOURS_TO_LOGOUT)
                    {
                        toBeRemoved.Add(u);
                    }
                }
                foreach (User u in toBeRemoved)
                {
                    removeUserFromCache(u);
                }
            }
        }

        private void removeUserFromCache(User u)
        {
            User remove = null;
            foreach (User user in _loggedUsers.Keys)
            {
                if (user.UserId.Equals(u.UserId))
                {
                    remove = user;
                    break;
                }
            }
            if (remove == null)
            {
                return;
            }
            _loggedUsers.Remove(remove);
            _usersTestsAnswerEveryTime.Remove(remove);
            _usersTestsAnswersAtEndRemainingQuestions.Remove(remove);
            _usersTestsAnswersAtEndAnsweredQuestions.Remove(remove);
        }

        public void logout(int userUniqueInt)
        {
            User user = _db.getUser(userUniqueInt);
            if (user != null)
            {
                removeUserFromCache(user);
            }
        }

        public Tuple<string, int> register(string email, string password, string medicalTraining, string firstName, string lastName)
        {
            // check for illegal input values
            if (!InputTester.isValidInput(new List<string>() { medicalTraining, firstName, lastName }))
            {
                return new Tuple<string, int>(GENERAL_INPUT_ERROR, -1);
            }
            if (!InputTester.isLegalEmail(email))
            {
                return new Tuple<string, int>(INVALID_EMAIL, -1);
            }
            if (!InputTester.isLegalPassword(password))
            {
                return new Tuple<string, int>(ILLEGAL_PASSWORD, -1);
            }
            // check if medical training level is invalid
            if (!Users.medicalTrainingLevels.Contains(medicalTraining))
            {
                return new Tuple<string, int>("Error - incorrect medical training level.", -1);
            }
            return _um.register(email, password, medicalTraining, firstName, lastName);
        }

        public Tuple<string, int> login(string email, string password)
        {
            // check for illegal input
            if (!InputTester.isLegalEmail(email))
            {
                return new Tuple<string, int>(INVALID_EMAIL, -1);
            }
            if (!InputTester.isLegalPassword(password))
            {
                return new Tuple<string, int>(ILLEGAL_PASSWORD, -1);
            }
            Tuple<string, User> loginResult = _um.login(email, password);
            if (!loginResult.Item1.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, int>(loginResult.Item1, -1);
            }
            // addd user to logged users list
            _loggedUsers[loginResult.Item2] = DateTime.Now;
            return new Tuple<string, int>(loginResult.Item1, loginResult.Item2.uniqueInt);
        }

        public string restorePassword(string email)
        {
            // check for illegal input values
            if (!InputTester.isLegalEmail(email))
            {
                return INVALID_EMAIL;
            }
            return _um.restorePassword(email);
        }

        public Tuple<string, int> answerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties, bool isAutoGenerated)
        {
            if (diagnoses == null || diagnosisCertainties == null)
            {
                return new Tuple<string, int>("Error. A list cannot be null", ERROR);
            }
            if (diagnoses.Count != diagnosisCertainties.Count)
            {
                return new Tuple<string, int>("Error. Cannot have a different number of diagnoses and diagnosis certainties.", -1);
            }
            if (normalityCertainty < 1 || normalityCertainty > 10)
            {
                return new Tuple<string, int>(CERTAINTY_LEVEL_ERROR, ERROR);
            }
            foreach (int i in diagnosisCertainties)
            {
                if (i < 1 || i > 10)
                {
                    return new Tuple<string, int>(CERTAINTY_LEVEL_ERROR, ERROR);
                }
            }
            // check for illegal input values
            if (isNormal && diagnoses.Count != 0)
            {
                return new Tuple<string, int>("Error. Cannot have diagnoses for a question that was deemed normal.", ERROR);
            }
            if (isNormal)
            {
                diagnoses.Add(Topics.NORMAL);
            }
            List<string> input = new List<string>();
            foreach (string s in diagnoses)
            {
                input.Add(s);
            }
            if (!InputTester.isValidInput(input))
            {
                return new Tuple<string, int>(GENERAL_INPUT_ERROR, ERROR);
            }
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, int>(NOT_LOGGED_IN, ERROR);
            }
            if (isAutoGenerated && !hasKey(_usersTestsAnswerEveryTime, user) && !hasKey(_usersTestsAnswersAtEndRemainingQuestions, user))
            {
                return new Tuple<string, int>("Error. Cannot answer a question prior to requesting one.", ERROR);
            }
            updateUserLastActionTime(user);
            // get data from DB
            Question q = _db.getQuestion(questionID);
            if (q == null)
            {
                return new Tuple<string, int>("Wrong data accepted. Incorrect question ID was recieved.", ERROR);
            }
            if (q.isDeleted)
            {
                return new Tuple<string, int>("Error. the question is marked as \"Removed\" so an answer cannot be saved.", ERROR);
            }
            foreach (string s in diagnoses)
            {
                if (_db.getTopic(q.SubjectId, s) == null)
                {
                    return new Tuple<string, int>("Error. diagnosis " + s + " is invalid for subject " + q.SubjectId, ERROR);
                }
            }
            foreach (Diagnosis d in _db.getQuestionDiagnoses(q.QuestionId))
            {
                UserLevel userLevel = _db.getUserLevel(user.UserId, q.SubjectId, d.TopicId);
                if (userLevel == null)
                {
                    userLevel = new UserLevel { UserId = user.UserId, SubjectId = q.SubjectId, TopicId = d.TopicId, level = Levels.DEFAULT_LVL, timesAnswered = 0, timesAnsweredCorrectly = 0 };
                    _db.addUserLevel(userLevel);
                }
            }
            int answerId = _logic.answerAQuestion(user, q, isNormal, normalityCertainty, diagnoses, diagnosisCertainties).Item2;
            if (!isAutoGenerated)
            {
                return new Tuple<string, int>(Replies.SUCCESS, answerId);
            }
            // if answer at end save q in answered questions, if remaining does not contain user return "Done" else return "Next"
            if (hasKey(_usersTestsAnswersAtEndRemainingQuestions, user))
            {
                User userInDictionary = null;
                foreach (User u in _usersTestsAnswersAtEndRemainingQuestions.Keys)
                {
                    if (u.UserId.Equals(user.UserId))
                    {
                        userInDictionary = u;
                        break;
                    }
                }
                // remove first question
                List<Question> l = _usersTestsAnswersAtEndRemainingQuestions[userInDictionary];
                l.RemoveAt(0);
                if (!hasKey(_usersTestsAnswersAtEndAnsweredQuestions, userInDictionary))
                {
                    //q.diagnoses = qDiagnoses;
                    _usersTestsAnswersAtEndAnsweredQuestions[userInDictionary] = new List<Question>() { q };
                }
                else
                {
                    List<Question> answered = _usersTestsAnswersAtEndAnsweredQuestions[userInDictionary];
                    answered.Add(q);
                    _usersTestsAnswersAtEndAnsweredQuestions[userInDictionary] = answered;
                }
                if (l.Count == 0)
                {
                    removeUserFromDictionary(user, _usersTestsAnswersAtEndRemainingQuestions);
                    //_usersTestsAnswersAtEndRemainingQuestions.Remove(user);
                    return new Tuple<string, int>(Replies.SHOW_ANSWER, answerId);
                }
                return new Tuple<string, int>(Replies.NEXT, answerId);
            }
            // else return "Show Answer"
            else
            {
                User userInDictionary = null;
                foreach (User u in _usersTestsAnswerEveryTime.Keys)
                {
                    if (u.UserId.Equals(user.UserId))
                    {
                        userInDictionary = u;
                        break;
                    }
                }
                List<Question> l = _usersTestsAnswerEveryTime[userInDictionary];
                //q.diagnoses = qDiagnoses;
                _usersTestsAnswersAtEndAnsweredQuestions[userInDictionary] = new List<Question>() { q };
                l.RemoveAt(0);
                if (l.Count == 0)
                {
                    removeUserFromDictionary(userInDictionary, _usersTestsAnswerEveryTime);
                    //_usersTestsAnswerEveryTime.Remove(user);
                }
                else
                {
                    _usersTestsAnswerEveryTime[userInDictionary] = l;
                }
                return new Tuple<string, int>(Replies.SHOW_ANSWER, answerId);
            }
        }

        private bool hasKey(Dictionary<User, List<Question>> d, User u)
        {
            foreach (User user in d.Keys)
            {
                if (user.UserId.Equals(u.UserId))
                {
                    return true;
                }
            }
            return false;
        }

        public Tuple<string, List<Question>> getAnsweres(int userUniqueInt)
        {
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, List<Question>>(NOT_LOGGED_IN, null);
            }
            User userInDictionary = null;
            foreach (User u in _usersTestsAnswersAtEndAnsweredQuestions.Keys)
            {
                if (u.UserId.Equals(user.UserId))
                {
                    userInDictionary = u;
                }
            }
            if (userInDictionary == null)
            {
                return new Tuple<string, List<Question>>("Error.", null);
            }
            List<Question> l = _usersTestsAnswersAtEndAnsweredQuestions[userInDictionary];
            _usersTestsAnswersAtEndAnsweredQuestions.Remove(userInDictionary);
            return new Tuple<string, List<Question>>(Replies.SUCCESS, l);
        }

        public Tuple<string, Question> getNextQuestion(int userUniqueInt)
        {
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, Question>(NOT_LOGGED_IN, null);
            }
            Question q = getQuestionFromTest(_usersTestsAnswerEveryTime, user);
            if (q == null)
            {
                q = getQuestionFromTest(_usersTestsAnswersAtEndRemainingQuestions, user);
            }
            return new Tuple<string, Question>(Replies.SUCCESS, q);
        }

        public List<string> getQuestionImages(int questionId)
        {
            List<QuestionImage> l = _db.getQuestionImages(questionId);
            List<string> ans = new List<string>();
            foreach (QuestionImage i in l)
            {
                ans.Add(i.ImageId.Replace("..", "../com"));
            }
            return ans;
        }

        public List<string> getQuestionDiagnoses(int questionId)
        {
            List<Diagnosis> l = _db.getQuestionDiagnoses(questionId);
            List<string> ans = new List<string>();
            foreach (Diagnosis d in l)
            {
                ans.Add(d.TopicId);
            }
            return ans;
        }

        private Question getQuestionFromTest(Dictionary<User, List<Question>> d, User user)
        {
            foreach (User u in d.Keys)
            {
                if (u.UserId.Equals(user.UserId) && d[u].Count > 0)
                {
                    return d[u][0];
                }
            }
            return null;
        }

        public string getAutoGeneratedQuesstion(int userUniqueInt, string subject, List<string> topics)
        {
            return getAutoGeneratedTest(userUniqueInt, subject, topics, 1, true);
        }

        public string getAutoGeneratedTest(int userUniqueInt, string subject, List<string> topics, int numOfQuestions, bool answerEveryTime)
        {
            if (topics == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            // check for illegal input values
            List<string> input = new List<string>(topics);
            input.Add(subject);
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return NOT_LOGGED_IN;
            }
            if (numOfQuestions < 1)
            {
                return "Error. Invalid number of questions for test.";
            }
            // if subject or topic are not in DB return error
            List<Topic> listOfTopics = new List<Topic>();
            foreach (string topic in topics)
            {
                Topic t = _db.getTopic(subject, topic);
                if (t == null)
                {
                    return "Error. Topic " + topic + "does not exist under subject " + subject;
                }
                listOfTopics.Add(t);
            }
            // send the userId instead of user level, so we can add a level only if relevant
            Tuple<string, List<Question>> test = _logic.getAutoGeneratedTest(subject, listOfTopics, user, numOfQuestions);
            if (!test.Item1.Equals(Replies.SUCCESS))
            {
                return test.Item1;
            }
            if (answerEveryTime || numOfQuestions == 1)
            {
                _usersTestsAnswerEveryTime[user] = test.Item2;
            }
            else
            {
                _usersTestsAnswersAtEndRemainingQuestions[user] = test.Item2;
            }
            return test.Item1;
        }

        public List<string> getAllSubjects()
        {
            setSubjectsAndTopics();
            List<string> l = new List<string>();
            foreach (string s in _subjectsTopics.Keys)
            {
                l.Add(s);
            }
            return l;
        }

        public List<string> getSubjectTopicsGetAQuestion(string subject)
        {
            return getSubjectTopics(subject, true);
        }

        public List<string> getSubjectTopicsCreateAQuestion(string subject)
        {
            return getSubjectTopics(subject, false);
        }

        private List<string> getSubjectTopics(string subject, bool isGettingAQuestion)
        {
            if (!InputTester.isValidInput(new List<string>() { subject }))
            {
                return null;
            }
            setSubjectsAndTopics();
            List<string> l = _subjectsTopics[subject];
            if (isGettingAQuestion)
            {
                l.Remove(Topics.NORMAL);
            }
            return l;
        }

        public string addSubject(int userUniqueInt, string subject)
        {
            // check for illegal input values
            List<string> input = new List<string>() { subject };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            return _se.addSubject(subject);
        }

        public string addTopic(int userUniqueInt, string subject, string topic)
        {
            // check for illegal input values
            List<string> input = new List<string>() { subject, topic };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            return _se.addTopic(subject, topic);
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            if (!InputTester.isLegalEmail(usernameToTurnToAdmin))
            {
                return INVALID_EMAIL;
            }
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            return _um.setUserAsAdmin(usernameToTurnToAdmin);
        }

        public bool hasMoreQuestions(int userUniqueInt)
        {
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return false;
            }
            return _usersTestsAnswerEveryTime.Keys.Contains(user) || _usersTestsAnswersAtEndRemainingQuestions.Keys.Contains(user);
        }

        public User isLoggedIn(int userUniqueInt)
        {
            User user = _db.getUser(userUniqueInt);
            if (user == null || !isLoggedIn(user))
            {
                return null;
            }
            updateUserLastActionTime(user);
            return user;
        }

        private bool isLoggedIn(User u)
        {
            foreach (User user in _loggedUsers.Keys)
            {
                if (user.UserId.Equals(u.UserId))
                {
                    return true;
                }
            }
            return false;
        }

        public string getUserName(int userUniqueInt)
        {
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return "";
            }
            return user.userFirstName + " " + user.userLastName;
        }

        public string createGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input) || inviteEmails == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            StringBuilder wrongEmails = new StringBuilder();
            wrongEmails.Append("These email addresses are invalid:" + Environment.NewLine);
            if (!inviteEmails.Equals(""))
            {
                List<string> emails = getEmailsfromString(inviteEmails);
                bool wrongEmail = false;
                foreach (string email in emails)
                {
                    if (!InputTester.isLegalEmail(email))
                    {
                        wrongEmail = true;
                        wrongEmails.Append(email + Environment.NewLine);
                    }
                }
                if (wrongEmail)
                {
                    return wrongEmails.ToString();
                }
            }
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return t.Item1;
            }
            lock (_syncLockGroup)
            {
                Group group = _db.getGroup(t.Item2.AdminId, groupName);
                if (group != null)
                {
                    return "Error. You have already created a group with that name.";
                }
                Group g = new Group { AdminId = t.Item2.AdminId, name = groupName };
                _db.addGroup(g);
            }
            return inviteEmails .Equals("") ? Replies.SUCCESS : inviteToGroup(userUniqueInt, groupName + GroupsMembers.CREATED_BY + t.Item2.AdminId + ")", inviteEmails, emailContent);
        }

        public string inviteToGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input) || inviteEmails == null || inviteEmails.Equals(""))
            {
                return GENERAL_INPUT_ERROR;
            }
            StringBuilder wrongEmails = new StringBuilder();
            wrongEmails.Append("These email addresses are invalid:" + Environment.NewLine);
            List<string> emails = getEmailsfromString(inviteEmails);
            bool wrongEmail = false;
            foreach (string email in emails)
            {
                if (!InputTester.isLegalEmail(email))
                {
                    wrongEmail = true;
                    wrongEmails.Append(email + Environment.NewLine);
                }
            }
            if (wrongEmail)
            {
                return wrongEmails.ToString();
            }
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return t.Item1;
            }
            User user = _db.getUser(userUniqueInt);
            lock (_syncLockGroup)
            {
                if (_db.getGroup(t.Item2.AdminId, getGroupNameAndAdminId(groupName).Item1) == null)
                {
                    return NON_EXISTING_GROUP;
                }
                if (!InputTester.isValidInput(new List<string>() { emailContent }))
                {
                    StringBuilder content = new StringBuilder();
                    content.Append("Hello," + Environment.NewLine + Environment.NewLine);
                    content.Append("You have been invited by " + user.userFirstName + " " + user.userLastName + " to take part in his group: " + groupName + "." + Environment.NewLine);
                    content.Append("In order to accept or decline the request, please visit " + WEBSITE + Environment.NewLine);
                    content.Append("If you are not registered, please use this email address in order to register and view your invitation.");
                    emailContent = content.ToString();
                }
                foreach (string email in emails)
                {
                    EmailSender.sendMail(email, "MedTrain Group Invitation", emailContent);
                    GroupMember gm = new GroupMember { GroupName = getGroupNameAndAdminId(groupName).Item1, AdminId = t.Item2.AdminId, UserId = email, invitationAccepted = false };
                    _db.addGroupMember(gm);
                }
            }
            return Replies.SUCCESS;
        }

        private List<string> getEmailsfromString(string emails)
        {
            string[] splittedemails = emails.Split(',');
            for (int i = 0; i < splittedemails.Length; i++)
            {
                string s = removeSpacesFromStart(splittedemails[i]);
                s = removeSpacesFromEnd(s);
                splittedemails[i] = s;
            }
            return splittedemails.ToList();
        }

        private string removeSpacesFromStart(string email)
        {
            while (email.StartsWith(" "))
            {
                email = email.Substring(1);
            }
            return email;
        }

        private string removeSpacesFromEnd(string email)
        {
            while (email.EndsWith(" "))
            {
                email = email.Substring(0, email.Length - 1);
            }
            return email;
        }

        public Tuple<string, List<string>> getAllAdminsGroups(int userUniqueInt)
        {
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return new Tuple<string,List<string>>(t.Item1, null);
            }
            List<Group> groups = _db.getAdminsGroups(t.Item2.AdminId);
            List<string> adminsGroups = new List<string>();
            foreach (Group g in groups)
            {
                adminsGroups.Add(g.name + GroupsMembers.CREATED_BY + t.Item2.AdminId + ")");
            }
            return new Tuple<string, List<string>>(Replies.SUCCESS, adminsGroups);
        }

        public string removeGroup(int userUniqueInt, string groupName)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            if (!groupName.EndsWith(")"))
            {
                return "Error. Invalid group name format.";
            }
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return t.Item1;
            }
            lock (_syncLockGroup)
            {
                Group g = _db.getGroup(t.Item2.AdminId, getGroupNameAndAdminId(groupName).Item1);
                if (g == null)
                {
                    return NON_EXISTING_GROUP;
                }
                _db.removeGroupMembers(g);
                _db.removeGroup(g);
                _db.removeGroupTestAnswers(groupName, t.Item2.AdminId);
            }
            return Replies.SUCCESS;
        }

        public string createTest(int userUniqueInt, string subject, List<string> topics)
        {
            // check for illegal input values
            if (topics == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            topics.Insert(0, Topics.NORMAL);
            List<string> input = new List<string>() { subject };
            foreach (string s in topics)
            {
                input.Add(s);
            }
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return t.Item1;
            }
            // verify subject is valid
            if (_db.getSubject(subject) == null)
            {
                return "The subject " + subject + " does not exist in the system.";
            }
            // verify topics are valid
            StringBuilder error = new StringBuilder();
            error.Append("The following topics does not exist in the system under the subject " + subject + ":");
            bool faultyTopics = false;
            List<Topic> subjectTopics = _db.getTopics(subject);
            List<string> subjectTopicsNames = new List<string>();
            foreach (Topic topic in subjectTopics)
            {
                subjectTopicsNames.Add(topic.TopicId);
            }
            foreach (string s in topics)
            {
                if (!subjectTopicsNames.Contains(s))
                {
                    faultyTopics = true;
                    error.Append(Environment.NewLine + s);
                }
            }
            if (faultyTopics)
            {
                return error.ToString();
            }
            // get all relevant questions for each topic
            List<Question> questions = new List<Question>();
            foreach (string s in topics)
            {
                List<Question> qs = _db.getQuestions(subject, s);
                foreach (Question q in qs)
                {
                    if (!questions.Contains(q))
                    {
                        questions.Add(q);
                    }
                }
            }
            // select only questions that has no irrelevant topics
            List<Question> selecetedQuestions = new List<Question>();
            foreach (Question q in questions)
            {
                List<Diagnosis> qDiagnoses = _db.getQuestionDiagnoses(q.QuestionId);
                foreach (Diagnosis d in qDiagnoses)
                {
                    if (!topics.Contains(d.TopicId))
                    {
                        break;
                    }
                    selecetedQuestions.Add(q);
                }
            }
            _testQuestions[t.Item2.AdminId] = new Tuple<string,List<Question>>(subject, selecetedQuestions);
            return Replies.SUCCESS;
        }

        public List<Question> getTestQuestions(int userUniqueInt)
        {
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return null;
            }
            if (!_testQuestions.Keys.Contains(t.Item2.AdminId))
            {
                return null;
            }
            List<Question> ans = _testQuestions[t.Item2.AdminId].Item2;
            //_testQuestions.Remove(t.Item2.AdminId);
            return ans;
        }

        public string createTest(int userUniqueInt, List<int> questionsIds, string name)
        {
            // check for illegal input values
            List<string> input = new List<string>() { name };
            if (!InputTester.isValidInput(input) || questionsIds == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return t.Item1;
            }
            // verify all question ids are valid
            foreach (int i in questionsIds)
            {
                if (_db.getQuestion(i) == null)
                {
                    return "Error. An invalid question has been selected.";
                }
            }
            // randomize questions order
            Random rnd = new Random();
            int n = questionsIds.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                int value = questionsIds[k];
                questionsIds[k] = questionsIds[n];
                questionsIds[n] = value;
            }
            string testSubject = _testQuestions[t.Item2.AdminId].Item1;
            _testQuestions.Remove(t.Item2.AdminId);
            lock (_syncLockTestId)
            {
                _db.addTest(new Test { TestId = _TestId, testName = name, AdminId = t.Item2.AdminId, subject = testSubject });
                foreach (int i in questionsIds)
                {
                    _db.addTestQuestion(new TestQuestion { TestId = _TestId, QuestionId = i });
                }
                _TestId++;
            }
            return Replies.SUCCESS;
        }

        public bool isAdmin(int userUniqueInt)
        {
            return hasPermissions(userUniqueInt).Item1.Equals(Replies.SUCCESS);
        }

        public Tuple<string, List<Test>> getAllTests(int userUniqueInt, string subject)
        {
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string,List<Test>>(s, null);
            }
            return new Tuple<string, List<Test>>(Replies.SUCCESS, _db.getAllTests(subject));
        }

        public string addTestToGroup(int userUniqueInt, string groupName, int TestId)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            Tuple<string, Admin> t = hasPermissions(userUniqueInt);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return t.Item1;
            }
            // verify group exist
            Tuple<string, string> groupNameAndAdmin = getGroupNameAndAdminId(groupName);
            if (_db.getGroup(t.Item2.AdminId, groupNameAndAdmin.Item1) == null)
            {
                return "Error. The administrator " + t.Item2.AdminId + " does not have a group named " + groupNameAndAdmin.Item1;
            }
            GroupTest gt = new GroupTest { AdminId = t.Item2.AdminId, GroupName = groupNameAndAdmin.Item1, TestId = TestId };
            _db.addGroupTest(gt);
            // email group members they have a test
            List<GroupMember> gms = _db.getGroupMembers(groupNameAndAdmin.Item1, t.Item2.AdminId);
            foreach (GroupMember gm in gms)
            {
                string msg = "A new test has been added to your group " + groupNameAndAdmin.Item1 + Environment.NewLine +
                    "Login now to complete it!";
                EmailSender.sendMail(gm.UserId, "New test in group " + groupNameAndAdmin.Item1, msg);
            }
            return Replies.SUCCESS;
        }

        public Tuple<string, List<string>> getUsersGroups(int userUniqueInt)
        {
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string,List<string>>(NOT_LOGGED_IN, null);
            }
            return groupMembers(_db.getUserGroups(user.UserId));
        }

        public Tuple<string, List<String>> getUsersGroupsInvitations(int userUniqueInt)
        {
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, List<string>>(NOT_LOGGED_IN, null);
            }
            return groupMembers(_db.getUserInvitations(user.UserId));
        }

        private Tuple<string, List<string>> groupMembers(List<GroupMember> gms)
        {
            List<string> l = new List<string>();
            foreach (GroupMember gm in gms)
            {
                l.Add(gm.ToString());
            }
            return new Tuple<string, List<string>>(Replies.SUCCESS, l);
        }

        public string acceptUsersGroupsInvitations(int userUniqueInt, List<string> groups)
        {
            return editUserInvitations(userUniqueInt, groups, true);
        }

        public string createQuestion(int userUniqueInt, string subject, List<string> qDiagnoses, List<byte[]> allImgs, string freeText)
        {
            // check for illegal input values
            if (qDiagnoses == null || allImgs == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            List<string> input = new List<string>(qDiagnoses);
            input.Add(subject);
            if (!InputTester.isValidInput(input) || freeText == null || freeText == "null")
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            return _qm.createQuestion(subject, qDiagnoses, allImgs, freeText);
        }

        public string removeQuestions(int userUniqueInt, List<Tuple<int,string>> questionsIdsAndResonsList)
        {
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            return _qm.removeQuestions(questionsIdsAndResonsList);
        }

        public Tuple<string, List<Tuple<string, int>>> getUnfinishedTests(int userUniqueInt, string groupName)
        {
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
            {
                return new Tuple<string, List<Tuple<string, int>>>(GENERAL_INPUT_ERROR, null);
            }
            if (groupName.LastIndexOf(GroupsMembers.CREATED_BY) == -1)
            {
                return new Tuple<string, List<Tuple<string, int>>>(INVALID_GROUP_NAME, null);
            }
            Tuple<string, string> t = getGroupNameAndAdminId(groupName);
            return getTests(userUniqueInt, t.Item1, t.Item2, false);
        }

        public Tuple<string, List<Tuple<string, int>>> getFinishedTests(int userUniqueInt, string groupName)
        {
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
            {
                return new Tuple<string, List<Tuple<string, int>>>(GENERAL_INPUT_ERROR, null);
            }
            if (groupName.LastIndexOf(GroupsMembers.CREATED_BY) == -1)
            {
                return new Tuple<string, List<Tuple<string, int>>>(INVALID_GROUP_NAME, null);
            }
            Tuple<string, string> t = getGroupNameAndAdminId(groupName);
            return getTests(userUniqueInt, t.Item1, t.Item2, true);
        }

        private Tuple<string, List<Tuple<string, int>>> getTests(int userUniqueInt, string groupName, string adminId, bool completed)
        {
            // verify user logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, List<Tuple<string, int>>>(NOT_LOGGED_IN, null);
            }
            // get all tests for the given group
            List<GroupTest> groupTests = _db.getGroupTests(groupName, adminId);
            List<Tuple<string, int>> ans = new List<Tuple<string, int>>();
            // foreach test
            foreach (GroupTest gt in groupTests)
            {
                if (!completed && hasMoreQuestionsGroupTest(userUniqueInt, groupName + GroupsMembers.CREATED_BY + adminId + ")", gt.TestId))
                {
                    Test test = _db.getTest(gt.TestId);
                    ans.Add(new Tuple<string, int>(test.testName, test.TestId));
                }
                if (completed && !hasMoreQuestionsGroupTest(userUniqueInt, groupName + GroupsMembers.CREATED_BY + adminId + ")", gt.TestId))
                {
                    Test test = _db.getTest(gt.TestId);
                    ans.Add(new Tuple<string, int>(test.testName, test.TestId));
                }
            }

            return new Tuple<string, List<Tuple<string, int>>>(Replies.SUCCESS, ans);
        }

        public Tuple<string, Question> getNextQuestionGroupTest(int userUniqueInt, string group, int test)
        {
            Tuple<string, List<int>> t = groupTestQuestions(userUniqueInt, group, test);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return new Tuple<string,Question>(t.Item1, null);
            }
            if (t.Item2.Count == 0)
            {
                return new Tuple<string, Question>("Error. All questions have been answered.", null);
            }
            Question q = _db.getQuestion(t.Item2[0]);
            if (q == null)
            {
                return new Tuple<string, Question>(DB_FAULT, null);
            }
            return new Tuple<string, Question>(Replies.SUCCESS, q);
        }

        public string answerAQuestionGroupTest(int userUniqueInt, string group, int test, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties)
        {
            string s = groupTestInputValidation(userUniqueInt, group, test);
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            // answer the question via answerAQuestion
            Tuple<string, int> t = answerAQuestion(userUniqueInt, questionID, isNormal, normalityCertainty, diagnoses, diagnosisCertainties, false);
            // if successfull, add to GroupsTestsAnswers
            if (t.Item2 == ERROR)
            {
                return t.Item1;
            }
            Tuple<string, string> splittedGroupName = getGroupNameAndAdminId(group);
            User u = _db.getUser(userUniqueInt);
            GroupTestAnswer gta = new GroupTestAnswer
            {
                GroupName = splittedGroupName.Item1,
                AdminId = splittedGroupName.Item2,
                AnswerId = t.Item2,
                UserId = u.UserId,
                TestId = test
            };
            _db.addGroupTestAnswer(gta);
            return hasMoreQuestionsGroupTest(userUniqueInt, group, test) ? Replies.NEXT : Replies.SUCCESS;
        }
        
        public bool hasMoreQuestionsGroupTest(int userUniqueInt, string group, int test)
        {
            Tuple<string, List<int>> t = groupTestQuestions(userUniqueInt, group, test);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return false;
            }
            return t.Item2.Count != 0;
        }

        private Tuple<string, List<int>> groupTestQuestions(int userUniqueInt, string group, int test)
        {
            string s = groupTestInputValidation(userUniqueInt, group, test);
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, List<int>>(s, null);
            }
            Tuple<string, List<int>> t = groupTestRemainingQuestions(userUniqueInt, group, test);
            if (!t.Item1.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, List<int>>(t.Item1, null);
            }
            return t;
        }

        private string groupTestInputValidation(int userUniqueInt, string group, int test)
        {
            List<string> input = new List<string>() { group };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            if (group.LastIndexOf(GroupsMembers.CREATED_BY) == -1)
            {
                return INVALID_GROUP_NAME;
            }
            Tuple<string, string> t = getGroupNameAndAdminId(group);
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return NOT_LOGGED_IN;
            }
            // verify group exist
            if (_db.getGroup(t.Item2, t.Item1) == null)
            {
                return NON_EXISTING_GROUP;
            }
            // verify test exist
            if (_db.getTest(test) == null)
            {
                return NON_EXISTING_TEST;
            }
            // verify the group has this test
            if (_db.getGroupTests(t.Item2, t.Item1) == null)
            {
                return "Error. This group does not have the requested test.";
            }
            return Replies.SUCCESS;
        }

        private Tuple<string, List<int>> groupTestRemainingQuestions(int userUniqueInt, string group, int test)
        {
            // get all test questions
            List<TestQuestion> testQuestions = _db.getTestQuestions(test);
            List<Question> questions = new List<Question>();
            foreach (TestQuestion tq in testQuestions)
            {
                Question q = _db.getQuestion(tq.QuestionId);
                if (q == null)
                {
                    return new Tuple<string, List<int>>(DB_FAULT, null);
                }
                questions.Add(q);
            }
            // get all relevant group test answers
            Tuple<string, string> t = getGroupNameAndAdminId(group);
            List<GroupTestAnswer> gtas = _db.getGroupTestAnswers(t.Item1, t.Item2, test);
            List<GroupTestAnswer> relevantGTAs = new List<GroupTestAnswer>();
            User user = _db.getUser(userUniqueInt);
            foreach (GroupTestAnswer gta in gtas)
            {
                if (user.UserId.Equals(gta.UserId))
                {
                    relevantGTAs.Add(gta);
                }
            }
            // get all relevant answers
            List<Answer> answers = new List<Answer>();
            foreach (GroupTestAnswer gta in relevantGTAs)
            {
                Answer a = _db.getAnswer(gta.AnswerId);
                if (a == null)
                {
                    return new Tuple<string, List<int>>(DB_FAULT, null);
                }
                answers.Add(a);
            }
            // get a question not answered by the user
            List<int> questionsIds = new List<int>();
            foreach (Question q in questions)
            {
                questionsIds.Add(q.QuestionId);
            }
            List<int> answeredQuestionsIds = new List<int>();
            foreach (Answer a in answers)
            {
                answeredQuestionsIds.Add(a.QuestionId);
            }
            List<int> remainingQuestionsIds = questionsIds.Except(answeredQuestionsIds).ToList();
            return new Tuple<string, List<int>>(Replies.SUCCESS, remainingQuestionsIds);
        }

        public string saveSelectedSubjectTopic(int userUniqueInt, string subject, List<string> topicsList)
        {
            // check for illegal input values
            if (topicsList == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            List<string> input = new List<string>(topicsList);
            input.Add(subject);
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            User user = _db.getUser(userUniqueInt);
            return _qm.saveSelectedSubjectTopic(user, subject, topicsList);
        }

        public Tuple<string, List<Question>> getAllReleventQuestions(int userUniqueInt)
        {
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string,List<Question>>(s, null);
            }
            User user = _db.getUser(userUniqueInt);
            return _qm.getAllReleventQuestions(user);
        }

        public string saveGroupAndTest(int userUniqueInt, string groupName, int TestId)
        {
            if (!InputTester.isValidInput(new List<string>() { groupName }))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return s;
            }
            User user = _db.getUser(userUniqueInt);
            _testDisplaying[user] = new Tuple<string, int>(groupName, TestId);
            return Replies.SUCCESS;
        }

        public Tuple<string, Tuple<string, int>> getSavedGroupAndTest(int userUniqueInt)
        {
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, Tuple<string, int>>(s, null);
            }
            User user = _db.getUser(userUniqueInt);
            if (!_testDisplaying.Keys.Contains(user))
            {
                return new Tuple<string, Tuple<string, int>>("Error. You have not selected a test to display.", null);
            }
            Tuple<string, int> t = _testDisplaying[user];
            _testDisplaying.Remove(user);
            return new Tuple<string, Tuple<string, int>>(Replies.SUCCESS, t);
        }

        public Tuple<string, List<Question>> getTestQuestionsByTestId(int userUniqueInt, int TestId)
        {
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, List<Question>>(s, null);
            }
            List<TestQuestion> tqs = _db.getTestQuestions(TestId);
            List<Question> ans = new List<Question>();
            foreach (TestQuestion tq in tqs)
            {
                ans.Add(_db.getQuestion(tq.QuestionId));
            }
            return new Tuple<string, List<Question>>(Replies.SUCCESS, ans);
        }

        public Tuple<string, List<Tuple<string, int, int, int, int>>> getPastGroupGrades(int userUniqueInt, string groupName)
        {
            return getPastGroupGrades(userUniqueInt, groupName, false);
        }

        private Tuple<string, List<Tuple<string, int, int, int, int>>> getPastGroupGrades(int userUniqueInt, string groupName, bool forAdmin)
        {
            // verify input
            if (!InputTester.isValidInput(new List<string>() { groupName }))
            {
                return new Tuple<string, List<Tuple<string, int, int, int, int>>>(GENERAL_INPUT_ERROR, null);
            }
            Tuple<string, string> t = getGroupNameAndAdminId(groupName);
            User user = isLoggedIn(userUniqueInt);
            if (!forAdmin)
            {
                // verify user is logged in
                if (user == null)
                {
                    return new Tuple<string, List<Tuple<string, int, int, int, int>>>(NOT_LOGGED_IN, null);
                }
            }
            else
            {
                user = _db.getUser(userUniqueInt);
            }
            // verify group exist
            if (_db.getGroup(t.Item2, t.Item1) == null)
            {
                return new Tuple<string, List<Tuple<string, int, int, int, int>>>(NON_EXISTING_GROUP, null);
            }
            List<Tuple<string, int, int, int, int>> l = new List<Tuple<string, int, int, int, int>>();
            // get all group tests
            List<GroupTest> groupTests = _db.getGroupTests(t.Item1, t.Item2);
            // get all group members
            List<GroupMember> groupMembers = _db.getGroupMembers(t.Item1, t.Item2);
            // foreach group test
            foreach (GroupTest gt in groupTests)
            {
                // get test
                Test test = _db.getTest(gt.TestId);
                // get test questions
                List<TestQuestion> testQuestions = _db.getTestQuestions(gt.TestId);
                // get user answers
                List<GroupTestAnswer> answers = _db.getGroupTestAnswers(t.Item1, t.Item2, gt.TestId);
                List<GroupTestAnswer> relevantAnswers = new List<GroupTestAnswer>();
                List<int> numsOfCorrectAnswers = new List<int>();
                foreach (GroupMember gm in groupMembers)
                {
                    List<GroupTestAnswer> usersAnswers = answers.Where(a => a.UserId.Equals(gm.UserId)).ToList();
                    // if not completed the test do not count this user
                    if (usersAnswers.Count < testQuestions.Count)
                    {
                        continue;
                    }
                    int correctAnswers = 0;
                    foreach (GroupTestAnswer gta in usersAnswers)
                    {
                        Answer a = _db.getAnswer(gta.AnswerId);
                        if (a.isCorrectAnswer)
                        {
                            correctAnswers++;
                        }
                    }
                    numsOfCorrectAnswers.Add(correctAnswers);
                    // set relevant answers if current user is the requesting user
                    if (gm.UserId.Equals(user.UserId))
                    {
                        relevantAnswers = usersAnswers;
                    }
                }
                // if the user did not complete the test ignore this test
                if (relevantAnswers.Count < testQuestions.Count)
                {
                    continue;
                }
                // count user correct answers
                int userCorrectAnswers = 0;
                foreach (GroupTestAnswer gta in relevantAnswers)
                {
                    Answer a = _db.getAnswer(gta.AnswerId);
                    if (a.isCorrectAnswer)
                    {
                        userCorrectAnswers++;
                    }
                }
                // count how many users has a better grade
                int betterThanUser = numsOfCorrectAnswers.Where(i => i > userCorrectAnswers).Count();
                l.Add(new Tuple<string, int, int, int, int>(test.testName, testQuestions.Count, userCorrectAnswers, betterThanUser, test.TestId));
            }
            if (l.Count == 0)
            {
                return new Tuple<string, List<Tuple<string, int, int, int, int>>>("Error. No statistics to show.", null);
            }
            return new Tuple<string, List<Tuple<string, int, int, int, int>>>(Replies.SUCCESS, l);
        }

        public double getTestGrade(int userUniqueInt, string group, int TestId)
        {
            // verify input
            if (!InputTester.isValidInput(new List<string>() { group }))
            {
                return -1;
            }
            Tuple<string, string> t = getGroupNameAndAdminId(group);
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return -1;
            }
            // verify group exist
            if (_db.getGroup(t.Item2, t.Item1) == null)
            {
                return -1;
            }
            Tuple<string, List<Tuple<string, int, int, int, int>>> usersTests = getPastGroupGrades(userUniqueInt, group);
            if (!usersTests.Item1.Equals(Replies.SUCCESS))
            {
                return -1;
            }
            foreach (Tuple<string, int, int, int, int> tuple in usersTests.Item2)
            {
                if (tuple.Item5 == TestId)
                {
                    return Math.Round((double)tuple.Item3 * (100.0 / tuple.Item2), 2);
                }
            }
            return -1;
        }

        public string declineUsersGroupsInvitations(int userUniqueInt, List<string> groups)
        {
            return editUserInvitations(userUniqueInt, groups, false);
        }

        private string editUserInvitations(int userUniqueInt, List<string> groups, bool accepted)
        {
            if (groups == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return NOT_LOGGED_IN;
            }
            string s = "decline invitations from";
            if (accepted)
            {
                s = "accept invitations to";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("Cannot " + s + " the following groups:");
            bool error = false;
            List<Tuple<string, string>> l = new List<Tuple<string, string>>();
            foreach (string group in groups)
            {
                if (group.LastIndexOf(GroupsMembers.CREATED_BY) == -1)
                {
                    return INVALID_GROUP_NAME;
                }
                Tuple<string, string> t = getGroupNameAndAdminId(group);
                if (!InputTester.isLegalEmail(t.Item2) || _db.getAdmin(t.Item2) == null)
                {
                    return INVALID_GROUP_NAME;
                }
                l.Add(t);
            }
            foreach (Tuple<string, string> t in l)
            {
                if (!_db.hasInvitation(user.UserId, t.Item1, t.Item2))
                {
                    sb.Append(Environment.NewLine + t.Item1);
                    error = true;
                }
            }
            if (error)
            {
                return sb.ToString();
            }
            foreach (Tuple<string, string> t in l)
            {
                GroupMember gm = _db.getGroupMemberInvitation(user.UserId, t.Item1, t.Item2);
                if (accepted)
                {
                    gm.invitationAccepted = true;
                    _db.updateGroupMember(gm);
                }
                else
                {
                    _db.removeGroupMember(gm);
                    _db.SaveChanges();
                }
            }
            return Replies.SUCCESS;
        }

        public Tuple<string, List<Test>> getGroupTests(int userUniqueInt, string group)
        {
            // verify input
            if (!InputTester.isValidInput(new List<string>() { group }))
            {
                return new Tuple<string, List<Test>>(GENERAL_INPUT_ERROR, null);
            }
            Tuple<string, string> t = getGroupNameAndAdminId(group);
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, List<Test>>(s, null);
            }
            // verify group exist
            if (_db.getGroup(t.Item2, t.Item1) == null)
            {
                return new Tuple<string, List<Test>>(NON_EXISTING_GROUP, null);
            }
            // get group tests
            List<GroupTest> gts = _db.getGroupTests(t.Item1, t.Item2);
            List<Test> tests = new List<Test>();
            foreach (GroupTest gt in gts)
            {
                Test test = _db.getTest(gt.TestId);
                if (test == null)
                {
                    return new Tuple<string, List<Test>>("Error. Invalid data - there is a group test to group " + t.Item1 + "with id " + gt.TestId + " but no test with that id in the DB.", null);
                }
                tests.Add(test);
            }
            return new Tuple<string, List<Test>>(Replies.SUCCESS, tests);
        }

        public Tuple<string, List<Tuple<string, int>>> getGrades(int userUniqueInt, int TestId, string group)
        {
            // verify input
            if (!InputTester.isValidInput(new List<string>() { group }))
            {
                return new Tuple<string, List<Tuple<string, int>>>(GENERAL_INPUT_ERROR, null);
            }
            Tuple<string, string> t = getGroupNameAndAdminId(group);
            // verify user has permissions
            string s = hasPermissions(userUniqueInt).Item1;
            if (!s.Equals(Replies.SUCCESS))
            {
                return new Tuple<string, List<Tuple<string, int>>>(s, null);
            }
            // verify group exist
            if (_db.getGroup(t.Item2, t.Item1) == null)
            {
                return new Tuple<string, List<Tuple<string, int>>>(NON_EXISTING_GROUP, null);
            }
            // verify test exist
            if (_db.getTest(TestId) == null)
            {
                return new Tuple<string, List<Tuple<string, int>>>(NON_EXISTING_TEST, null);
            }
            // get group members
            List<GroupMember> groupMembers = _db.getGroupMembers(t.Item1, t.Item2);
            // foreach member get their grade
            List<Tuple<string, int>> ans = new List<Tuple<string, int>>();
            foreach (GroupMember gm in groupMembers)
            {
                User u = _db.getUser(gm.UserId);
                Tuple<string, List<Tuple<string, int, int, int, int>>> grades = getPastGroupGrades(u.uniqueInt, group, true);
                Tuple<string, int, int, int, int> relevantGrade = null;
                foreach (Tuple<string, int, int, int, int> grade in grades.Item2)
                {
                    if (grade.Item5 == TestId)
                    {
                        relevantGrade = grade;
                        break;
                    }
                }
                if (relevantGrade == null)
                {
                    continue;
                }
                ans.Add(new Tuple<string, int>(u.UserId, (int)relevantGrade.Item3 * 100 / relevantGrade.Item2));
            }
            return new Tuple<string, List<Tuple<string, int>>>(Replies.SUCCESS, ans);
        }

        private void updateUserLastActionTime(User u)
        {
            User update = null;
            foreach (User user in _loggedUsers.Keys)
            {
                if (user.UserId.Equals(u.UserId))
                {
                    update = user;
                    break;
                }
            }
            if (update == null)
            {
                return;
            }
            _loggedUsers[u] = DateTime.Now;
        }

        private Tuple<string, string> getGroupNameAndAdminId(string s)
        {
            int i = s.LastIndexOf(GroupsMembers.CREATED_BY);
            string groupName = s.Substring(0, i);
            string adminId = s.Substring(i + GroupsMembers.CREATED_BY.Length);
            adminId = adminId.Substring(0, adminId.Length - 1);
            return new Tuple<string, string>(groupName, adminId);
        }

        private Tuple<string, Admin> hasPermissions(int userUniqueInt)
        {
            User u = isLoggedIn(userUniqueInt);
            if (u == null)
            {
                return new Tuple<string,Admin>(NOT_LOGGED_IN, null);
            }
            Admin a = _db.getAdmin(u.UserId);
            return a != null ? new Tuple<string, Admin>(Replies.SUCCESS, a) : new Tuple<string, Admin>(NOT_AN_ADMIN, null);
        }

        private void removeUserFromDictionary(User u, Dictionary<User, List<Question>> d)
        {
            User remove = null;
            foreach (User user in _loggedUsers.Keys)
            {
                if (user.UserId.Equals(u.UserId))
                {
                    remove = user;
                    break;
                }
            }
            if (remove == null)
            {
                return;
            }
            d.Remove(remove);
        }
    }
}
