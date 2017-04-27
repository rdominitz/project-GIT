﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Constants;
using System.Threading;
using QALogic;

namespace Server
{
    public class ServerImpl : IServer
    {
        private const string WEBSITE = "our website address";
        private const string GENERAL_INPUT_ERROR = "There is a null, the string \"null\" or an empty string as an input.";
        private const string INVALID_EMAIL = "Invalid eMail address.";
        private const string ILLEGAL_PASSWORD = "Illegal password. Password must be 5 to 15 characters long and consist of only letters and numbers.";
        private const string INVALID_TEMPORAL_PASSWORD = "Bad cookie. Could not identify user.";
        private const string NOT_LOGGED_IN = "User is not logged in.";
        private const string EMAIL_IN_USE = "This eMail address is already in use.";
        private const string USER_NOT_REGISTERED = "User is not registered.";
        private const string NOT_AN_ADMIN = "Error. only an admin has the required permissions to perform this action.";
        private const string CERTAINTY_LEVEL_ERROR = "Error. Certainty levels must be between 1 to 10.";
        private const string NON_EXISTING_GROUP = "Error. You have not created a group with that name.";
        private const string INVALID_GROUP_NAME = "Error. Invalid group name.";
        private const string DB_FAULT = "Error. DB fault.";
        private const string NOT_A_SUBJECT = "Error. Subject does not exist in the system.";
        private const string NON_EXISTING_TEST = "Error. There is no test matching the given value.";
        private const int USERS_CACHE_LIMIT = 1000;
        private const int HOURS_TO_LOGOUT = 1;
        private const int MILLISECONDS_TO_SLEEP = HOURS_TO_LOGOUT * 60 * 60 * 1000;

        private List<User> _usersCache; // each action will move the user to the last position of the cache, removing old users from the beginning
        private Dictionary<User, DateTime> _loggedUsers;

        private Dictionary<User, List<Question>> _usersTestsAnswerEveryTime;
        private Dictionary<User, List<Question>> _usersTestsAnswersAtEndRemainingQuestions;
        private Dictionary<User, List<Question>> _usersTestsAnswersAtEndAnsweredQuestions;
        private Dictionary<User, List<Question>> _questionsForGroupTest;
        private Dictionary<string, string> _selectedGroups;
        private Dictionary<string, List<Question>> _testQuestions;

        private int _userUniqueInt;
        private readonly object _syncLockUserUniqueInt;

        private int _questionID;
        private readonly object _syncLockQuestionId;

        private readonly object _syncLockGroup;

        private int _testID;
        private readonly object _syncLockTestId;

        private Dictionary<string, List<string>> _subjectsTopics;

        private ILogic _logic;
        private IMedTrainDBContext _db;

        public ServerImpl(IMedTrainDBContext db)
        {
            _usersCache = new List<User>();
            _loggedUsers = new Dictionary<User, DateTime>();
            _usersTestsAnswerEveryTime = new Dictionary<User, List<Question>>();
            _usersTestsAnswersAtEndRemainingQuestions = new Dictionary<User, List<Question>>();
            _usersTestsAnswersAtEndAnsweredQuestions = new Dictionary<User, List<Question>>();
            _questionsForGroupTest = new Dictionary<User, List<Question>>();
            _selectedGroups = new Dictionary<string, string>();
            _testQuestions = new Dictionary<string, List<Question>>();
            _db = db;
            _logic = new LogicImpl(db);
            _userUniqueInt = 100000;
            _syncLockUserUniqueInt = new object();
            _questionID = 1;
            _syncLockQuestionId = new object();
            _syncLockGroup = new object();
            _testID = 1;
            _syncLockTestId = new object();
            Thread.Sleep(_db.getMillisecondsToSleep());
            _subjectsTopics = new Dictionary<string, List<string>>();
            setSubjectsAndTopics();
            Thread removeUsersThread = new Thread(new ThreadStart(removeNonActiveUsers));
            removeUsersThread.Start();
            User u = new User
            {
                uniqueInt = _userUniqueInt,
                UserId = "defaultadmin@gmail.com",
                userFirstName = "default",
                userLastName = "admin",
                userMedicalTraining = Users.medicalTrainingLevels[0],
                userPassword = "password"
            };
            _userUniqueInt++;
            _db.addUser(u);
            Admin a = new Admin { AdminId = "defaultadmin@gmail.com" };
            _db.addAdmin(a);
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
            addQuestions(chestXRays, new List<Topic>() { cxrLeftPleuralEffusion }, new List<List<string>>()
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
            addQuestions(chestXRays, new List<Topic>() { cxrLeftPleuralEffusion }, new List<List<string>>()
            {
                new List<string>() { "../Images/q27_4_PA.png" },
                new List<string>() { "../Images/q28_13_PA.png", "../Images/q28_13_Lat.png" },
                new List<string>() { "../Images/q29_44_PA.png" },
                new List<string>() { "../Images/q30_51_PA.png", "../Images/q30_51_Lat.png" }
            });
            #endregion
            // add more questions
            #region add user
            User u = new User
            {
                UserId = "user@gmail.com",
                userPassword = "password",
                userMedicalTraining = Users.medicalTrainingLevels[0],
                userFirstName = "first name",
                userLastName = "last name",
                uniqueInt = _userUniqueInt
            };
            _userUniqueInt++;
            _db.addUser(u);
            #endregion
            _subjectsTopics["Chest x-Rays"] = new List<string>()
            {
                "Cavitary Lesion", "Interstitial opacities", "Left Pleural Effusion",
                "Median Sternotomy", "Right Middle Lobe Collapse"
            };
            #region add group
            Group g = new Group
            {
                AdminId = "defaultadmin@gmail.com",
                name = "Test Group 1"
            };
            _db.addGroup(g);
            #endregion
            /*********** Roie added code here*************/
            #region roie
            Group g2 = new Group
            {
                AdminId = "defaultadmin@gmail.com",
                name = "Test Group 2"
            };
            _db.addGroup(g2);
            Group g3 = new Group
            {
                AdminId = "defaultadmin@gmail.com",
                name = "Test Group 3"
            };
            _db.addGroup(g3);
            Group g4 = new Group
            {
                AdminId = "defaultadmin@gmail.com",
                name = "Test Group 4"
            };
            _db.addGroup(g4);
            Group g5 = new Group
            {
                AdminId = "defaultadmin@gmail.com",
                name = "Test Group 5"
            };
            _db.addGroup(g5);
            Group g6 = new Group
            {
                AdminId = "defaultadmin@gmail.com",
                name = "Test Group 6"
            };
            _db.addGroup(g6);

            GroupMember gm1 = new GroupMember
            {
                GroupName = "Test Group 1",
                AdminId = "defaultadmin@gmail.com",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm1);
            GroupMember gm2 = new GroupMember
            {
                GroupName = "Test Group 2",
                AdminId = "defaultadmin@gmail.com",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm2);
            GroupMember gm3 = new GroupMember
            {
                GroupName = "Test Group 3",
                AdminId = "defaultadmin@gmail.com",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm3);
            GroupMember gm4 = new GroupMember
            {
                GroupName = "Test Group 4",
                AdminId = "defaultadmin@gmail.com",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm4);
            GroupMember gm5 = new GroupMember
            {
                GroupName = "Test Group 5",
                AdminId = "defaultadmin@gmail.com",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm5);
            GroupMember gm6 = new GroupMember
            {
                GroupName = "Test Group 6",
                AdminId = "defaultadmin@gmail.com",
                UserId = "user@gmail.com",
                invitationAccepted = false
            };
            _db.addGroupMember(gm6);
            #endregion
            /*********** Roie stopped adding code here*************/
            #region add test
            Test t = new Test { TestId = _testID, AdminId = "defaultAdmin@gmail.com", testName = "example test" };
            _testID++;
            _db.addTest(t);
            for (int i = 5; i <= 13; i++)
            {
                TestQuestion tq = new TestQuestion { TestId = 1, QuestionId = i };
                _db.addTestQuestion(tq);
            }
            GroupTest gt = new GroupTest{GroupName = "Test Group 1", AdminId = "defaultAdmin@gmail.com", TestId = 1};
            _db.addGroupTest(gt);
            #endregion
        }

        private void addQuestions(Subject s, List<Topic> diagnoses, List<List<string>> images)
        {
            foreach (List<string> l in images)
            {
                addQuestion(s, diagnoses, l);
            }
        }

        private void addQuestion(Subject s, List<Topic> diagnoses, List<string> images)
        {
            Question q = new Question
            {
                QuestionId = _questionID,
                SubjectId = s.SubjectId,
                text = "",
                level = Levels.DEFAULT_LVL,
                points = Questions.QUESTION_INITAL_POINTS,
                timeAdded = DateTime.Now,
                isDeleted = false
            };
            _db.addQuestion(q);
            foreach (Topic t in diagnoses)
            {
                Diagnosis d = new Diagnosis
                {
                    QuestionId = _questionID,
                    TopicId = t.TopicId,
                    SubjectId = s.SubjectId
                };
                _db.addDiagnosis(d);
            }
            foreach (string path in images)
            {
                Image i = new Image
                {
                    ImageId = path,
                    QuestionId = _questionID,
                };
                _db.addImage(i);
            }
            _questionID++;
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
            _loggedUsers.Remove(u);
            _usersTestsAnswerEveryTime.Remove(u);
            _usersTestsAnswersAtEndRemainingQuestions.Remove(u);
            _usersTestsAnswersAtEndAnsweredQuestions.Remove(u);
        }

        public void logout(int userUniqueInt)
        {
            User user = getUserByInt(userUniqueInt);
            if (user != null)
            {
                removeUserFromCache(user);
            }
        }

        public Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName)
        {
            // check for illegal input values
            if (!InputTester.isValidInput(new List<string>() { medicalTraining, firstName, lastName }))
            {
                return new Tuple<string, int>(GENERAL_INPUT_ERROR, -1);
            }
            if (!InputTester.isLegalEmail(eMail))
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
            // search user in cache
            List<User> matches = _usersCache.Where(u => u.UserId == eMail).ToList();
            if (matches.Count != 0)
            {
                return new Tuple<string, int>(EMAIL_IN_USE, -1);
            }
            // search DB
            if (_db.getUser(eMail) != null)
            {
                return new Tuple<string, int>(EMAIL_IN_USE, -1);
            }
            // if DB contains user with that eMail return error message
            int userUniqueInt = 0;
            User user = null;
            lock (_syncLockUserUniqueInt)
            {
                userUniqueInt = _userUniqueInt;
                user = new User { UserId = eMail, userPassword = password, userMedicalTraining = medicalTraining, userFirstName = firstName, userLastName = lastName, uniqueInt = _userUniqueInt };
                _userUniqueInt++;
            }
            // add to DB
            _db.addUser(user);
            if (_usersCache.Count == USERS_CACHE_LIMIT)
            {
                _usersCache.RemoveAt(0);
            }
            // add to chache
            _usersCache.Add(user);
            return new Tuple<string, int>(Replies.SUCCESS, userUniqueInt);
        }

        public Tuple<string, int> login(string eMail, string password)
        {
            // check for illegal input
            if (!InputTester.isLegalEmail(eMail))
            {
                return new Tuple<string, int>(INVALID_EMAIL, -1);
            }
            if (!InputTester.isLegalPassword(password))
            {
                return new Tuple<string, int>(ILLEGAL_PASSWORD, -1);
            }
            // search user in cache
            List<User> matches = _usersCache.Where(u => u.UserId == eMail).ToList();
            if (matches.Count == 1)
            {
                return verifyLogin(matches.ElementAt(0), password);
            }
            // search DB
            User user = _db.getUser(eMail);
            if (user == null)
            {
                return new Tuple<string, int>("Wrong eMail or password.", -1);
            }
            // if found add to cache and return relevant message as shown above
            return verifyLogin(user, password);
        }

        private Tuple<string, int> verifyLogin(User u, string password)
        {
            if (!u.userPassword.Equals(password))
            {
                return new Tuple<string, int>("Wrong password", -1);
            }
            // place user at the end of the cache (to be the last one to be removed)
            _usersCache.Remove(u);
            _usersCache.Add(u);
            // addd user to logged users list
            updateUserLastActionTime(u);
            return new Tuple<string, int>(Replies.SUCCESS, u.uniqueInt);
        }

        public string restorePassword(string eMail)
        {
            // check for illegal input values
            if (!InputTester.isLegalEmail(eMail))
            {
                return INVALID_EMAIL;
            }
            // search user in cache
            List<User> matches = _usersCache.Where(u => u.UserId == eMail).ToList();
            User user = null;
            if (matches.Count == 1)
            {
                // place user at the end of the cache (to be the last one to be removed)
                _usersCache.Remove(matches.ElementAt(0));
                _usersCache.Add(matches.ElementAt(0));
                user = matches.ElementAt(0);
            }
            // search user in DB
            if (user == null)
            {
                user = _db.getUser(eMail);
            }
            // if doesn't exist return error message
            if (user == null)
            {
                return "eMail address does not exist in the system.";
            }
            // send eMail
            StringBuilder sb = new StringBuilder();
            sb.Append("Hello " + user.userFirstName + " " + user.userLastName + "," + Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Your password for our system is: " + user.userPassword + Environment.NewLine);
            EmailSender.sendMail(eMail, "Medical Training System Password Restoration", sb.ToString());
            return Replies.SUCCESS;
        }

        public string answerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties)
        {
            if (diagnoses == null || diagnosisCertainties == null)
            {
                return "Error - a list cannot be null";
            }
            if (diagnoses.Count != diagnosisCertainties.Count)
            {
                return "Error - cannot have a different number of diagnoses and diagnosis certainties.";
            }
            if (normalityCertainty < 1 || normalityCertainty > 10)
            {
                return CERTAINTY_LEVEL_ERROR;
            }
            foreach (int i in diagnosisCertainties)
            {
                if (i < 1 || i > 10)
                {
                    return CERTAINTY_LEVEL_ERROR;
                }
            }
            // check for illegal input values
            if (isNormal && diagnoses.Count != 0)
            {
                return "Error - cannot have diagnoses for a question that was deemed normal.";
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
                return GENERAL_INPUT_ERROR;
            }
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return NOT_LOGGED_IN;
            }
            if (!_usersTestsAnswerEveryTime.Keys.Contains(user) && !_usersTestsAnswersAtEndRemainingQuestions.Keys.Contains(user))
            {
                return "Error. Cannot answer a question prior to requesting one.";
            }
            updateUserLastActionTime(user);
            // get data from DB
            Question q = _db.getQuestion(questionID);
            if (q == null)
            {
                return "Wrong data accepted. Incorrect question ID was recieved.";
            }
            foreach (string s in diagnoses)
            {
                if (_db.getTopic(q.SubjectId, s) == null)
                {
                    return "Error. diagnosis " + s + " is invalid for subject " + q.SubjectId;
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
            _logic.answerAQuestion(user, q, isNormal, normalityCertainty, diagnoses, diagnosisCertainties);
            // if answer at end save q in answered questions, if remaining does not contain user return "Done" else return "Next"
            if (_usersTestsAnswersAtEndRemainingQuestions.ContainsKey(user))
            {
                // remove first question
                List<Question> l = _usersTestsAnswersAtEndRemainingQuestions[user];
                l.RemoveAt(0);
                if (!_usersTestsAnswersAtEndAnsweredQuestions.ContainsKey(user))
                {
                    //q.diagnoses = qDiagnoses;
                    _usersTestsAnswersAtEndAnsweredQuestions[user] = new List<Question>() { q };
                }
                else
                {
                    List<Question> answered = _usersTestsAnswersAtEndAnsweredQuestions[user];
                    answered.Add(q);
                    _usersTestsAnswersAtEndAnsweredQuestions[user] = answered;
                }
                if (l.Count == 0)
                {
                    _usersTestsAnswersAtEndRemainingQuestions.Remove(user);
                    return Replies.SHOW_ANSWER;
                }
                return Replies.NEXT;
            }
            // else return "Show Answer"
            else
            {
                List<Question> l = _usersTestsAnswerEveryTime[user];
                //q.diagnoses = qDiagnoses;
                _usersTestsAnswersAtEndAnsweredQuestions[user] = new List<Question>() { q };
                l.RemoveAt(0);
                if (l.Count == 0)
                {
                    _usersTestsAnswerEveryTime.Remove(user);
                }
                return Replies.SHOW_ANSWER;
            }
        }

        public Tuple<string, List<Question>> getAnsweres(int userUniqueInt)
        {
            User user = isLoggedIn(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, List<Question>>(NOT_LOGGED_IN, null);
            }
            List<Question> l = _usersTestsAnswersAtEndAnsweredQuestions[user];
            _usersTestsAnswersAtEndAnsweredQuestions.Remove(user);
            return new Tuple<string, List<Question>>(Replies.SUCCESS, l);
        }

        public Tuple<string, Question> getNextQuestion(int userUniqueInt)
        {
            User user = getUserByInt(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, Question>(USER_NOT_REGISTERED, null);
            }
            // verify user is logged in
            if (!_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, Question>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            Question q = getQuestionFromTest(_usersTestsAnswerEveryTime, user);
            if (q == null)
            {
                q = getQuestionFromTest(_usersTestsAnswersAtEndRemainingQuestions, user);
            }
            return new Tuple<string, Question>(Replies.SUCCESS, q);
        }

        public List<string> getQuestionImages(int questionId)
        {
            List<Image> l = _db.getQuestionImages(questionId);
            List<string> ans = new List<string>();
            foreach (Image i in l)
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
            return d.ContainsKey(user) ? d[user][0] : null;
        }

        public string getAutoGeneratedQuesstion(int userUniqueInt, string subject, string topic)
        {
            return getAutoGeneratedTest(userUniqueInt, subject, topic, 1, true);
        }

        public string getAutoGeneratedTest(int userUniqueInt, string subject, string topic, int numOfQuestions, bool answerEveryTime)
        {
            // check for illegal input values
            List<string> input = new List<string>() { subject, topic };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            if (numOfQuestions < 1)
            {
                return "Error - invalid number of questions for test.";
            }
            // if subject or topic are not in DB return error
            Topic t = _db.getTopic(subject, topic);
            if (t == null)
            {
                return "Error - provided subject and topic combination is invalid.";
            }
            // send the userId instead of user level, so we can add a level only if relevant
            Tuple<string, List<Question>> test = _logic.getAutoGeneratedTest(subject, t, user, numOfQuestions);
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

        public List<string> getSubjectTopics(string subject)
        {
            if (!InputTester.isValidInput(new List<string>() { subject }))
            {
                return null;
            }
            setSubjectsAndTopics();
            List<string> l = _subjectsTopics[subject];
            l.Remove(Topics.NORMAL);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify subject does not exist
            Subject sub = _db.getSubject(subject);
            if (sub != null)
            {
                return "Error. Subject already exists in the system.";
            }
            // add subject
            sub = new Subject { SubjectId = subject, timeAdded = DateTime.Now };
            _db.addSubject(sub);
            Topic t = new Topic { TopicId = Topics.NORMAL, SubjectId = subject, timeAdded = DateTime.Now };
            _db.addTopic(t);
            _db.SaveChanges();
            return Replies.SUCCESS;
        }

        public string addTopic(int userUniqueInt, string subject, string topic)
        {
            // check for illegal input values
            List<string> input = new List<string>() { subject, topic };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify subject exist
            Subject sub = _db.getSubject(subject);
            if (sub == null)
            {
                return "Error. Subject does not exist in the system.";
            }
            // verify topic does not exist in the system
            Topic t = _db.getTopic(subject, topic);
            if (t != null)
            {
                return "Error. Topic already exists in the system.";
            }
            Topic top = new Topic { SubjectId = subject, timeAdded = DateTime.Now, TopicId = topic };
            _db.addTopic(top);
            _db.SaveChanges();
            return Replies.SUCCESS;
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            if (!InputTester.isLegalEmail(usernameToTurnToAdmin))
            {
                return INVALID_EMAIL;
            }
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            User u = _db.getUser(usernameToTurnToAdmin);
            // verify user exist
            if (u == null)
            {
                return "Error. Cannot make " + usernameToTurnToAdmin + " an admin since " + usernameToTurnToAdmin + " is not registered to the system.";
            }
            _db.addAdmin(new Admin { AdminId = u.UserId });
            _db.SaveChanges();
            return Replies.SUCCESS;
        }

        public bool hasMoreQuestions(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return false;
            }
            updateUserLastActionTime(user);
            return _usersTestsAnswerEveryTime.Keys.Contains(user) || _usersTestsAnswersAtEndRemainingQuestions.Keys.Contains(user);
        }

        public User isLoggedIn(int userUniqueInt)
        {
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return null;
            }
            updateUserLastActionTime(user);
            return _loggedUsers.Keys.Contains(user) ? user : null;
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            lock (_syncLockGroup)
            {
                Group group = _db.getGroup(admin.AdminId, groupName);
                if (group != null)
                {
                    return "Error. You have already created a group with that name.";
                }
                Group g = new Group { AdminId = admin.AdminId, name = groupName };
                _db.addGroup(g);
            }
            return inviteEmails .Equals("") ? Replies.SUCCESS : inviteToGroup(userUniqueInt, groupName, inviteEmails, emailContent);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            lock (_syncLockGroup)
            {
                if (_db.getGroup(admin.AdminId, groupName) == null)
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
                    GroupMember gm = new GroupMember { GroupName = groupName, AdminId = admin.AdminId, UserId = email, invitationAccepted = false };
                    _db.addGroupMember(gm);
                }
            }
            return Replies.SUCCESS;
        }

        private List<string> getEmailsfromString(string emails)
        {
            string[] eMails = emails.Split(',');
            for (int i = 0; i < eMails.Length; i++)
            {
                string s = removeSpacesFromStart(eMails[i]);
                s = removeSpacesFromEnd(s);
                eMails[i] = s;
            }
            return eMails.ToList();
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, List<string>>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return new Tuple<string, List<string>>(NOT_AN_ADMIN, null);
            }
            List<Group> groups = _db.getAdminsGroups(admin.AdminId);
            List<string> adminsGroups = new List<string>();
            foreach (Group g in groups)
            {
                adminsGroups.Add(g.name);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            lock (_syncLockGroup)
            {
                Group g = _db.getGroup(admin.AdminId, groupName);
                if (g == null)
                {
                    return NON_EXISTING_GROUP;
                }
                _db.removeGroupMembers(g);
                _db.removeGroup(g);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
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
            foreach (Topic t in subjectTopics)
            {
                subjectTopicsNames.Add(t.TopicId);
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
            _testQuestions[admin.AdminId] = selecetedQuestions;
            return Replies.SUCCESS;
        }

        public List<Question> getTestQuestions(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return null;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return null;
            }
            if (!_testQuestions.Keys.Contains(admin.AdminId))
            {
                return null;
            }
            List<Question> ans = _testQuestions[admin.AdminId];
            _testQuestions.Remove(admin.AdminId);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify all question ids are valid
            foreach (int i in questionsIds)
            {
                if (_db.getQuestion(i) == null)
                {
                    return "Error. An invalid question has been selected.";
                }
            }
            lock (_syncLockTestId)
            {
                _db.addTest(new Test { TestId = _testID, testName = name, AdminId = admin.AdminId });
                foreach (int i in questionsIds)
                {
                    _db.addTestQuestion(new TestQuestion { TestId = _testID, QuestionId = i });
                }
                _testID++;
            }
            return Replies.SUCCESS;
        }

        public bool isAdmin(int userUniqueInt)
        {
            User u = getUserByInt(userUniqueInt);
            if (u == null)
            {
                return false;
            }
            return _db.getAdmin(u.UserId) != null;
        }

        public Tuple<string, List<Test>> getAllTests(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, List<Test>>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return new Tuple<string, List<Test>>(NOT_AN_ADMIN, null);
            }
            return new Tuple<string, List<Test>>(Replies.SUCCESS, _db.getAllTests());
        }

        public string addTestToGroup(int userUniqueInt, string groupName, int testId)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify group exist
            if (_db.getGroup(admin.AdminId, groupName) == null)
            {
                return "Error. The administrator " + admin.AdminId + " does not have a group named " + groupName;
            }
            GroupTest gt = new GroupTest { AdminId = admin.AdminId, GroupName = groupName, TestId = testId };
            _db.addGroupTest(gt);
            return Replies.SUCCESS;
        }

        public Tuple<string, List<string>> getUsersGroups(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string,List<string>>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            return groupMembers(_db.getUserGroups(user.UserId));
        }

        public Tuple<string, List<String>> getUsersGroupsInvitations(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, List<string>>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
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
            if (groups == null)
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            StringBuilder sb = new StringBuilder();
            sb.Append("Cannot accept invitations to the following groups:");
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
                gm.invitationAccepted = true;
                _db.updateGroupMember(gm);
            }
            return Replies.SUCCESS;
        }

        public string saveSelectedGroup(int userUniqueInt, string groupName)
        {
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
            {
                return GENERAL_INPUT_ERROR;
            }
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            _selectedGroups[admin.AdminId] = groupName;
            return Replies.SUCCESS;
        }

        public Tuple<string, string> getSavedGroup(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, string>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return new Tuple<string, string>(NOT_AN_ADMIN, null);
            }
            if (!_selectedGroups.Keys.Contains(admin.AdminId))
            {
                return new Tuple<string, string>("Error. You have not selected a group.", null);
            }
            string groupName = _selectedGroups[admin.AdminId];
            _selectedGroups.Remove(admin.AdminId);
            return new Tuple<string, string>(Replies.SUCCESS, groupName);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify subject exist
            Subject sub = _db.getSubject(subject);
            if (sub == null)
            {
                return NOT_A_SUBJECT;
            }
            // verify all diagnoses are topics of the specified subject
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            List<Topic> subjectTopics = _db.getTopics(subject);
            foreach (string diagnosys in qDiagnoses)
            {
                if (subjectTopics.Where(t => t.TopicId.Equals(diagnosys)).ToList().Count == 0)
                {
                    return "Error. " + diagnosys + " is not a topic of " + subject;
                }
            }
            if (qDiagnoses.Count == 0)
            {
                qDiagnoses.Add(Topics.NORMAL);
            }
            lock (_syncLockQuestionId)
            {
                // save images
                List<string> imagesPathes = new List<string>();
                addQuestion(sub, subjectTopics.Where(st => qDiagnoses.Contains(st.TopicId)).ToList(), imagesPathes);
            }
            return Replies.SUCCESS;
        }

        public string removeQuestions(int userUniqueInt, List<int> questionsIdsList)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify all questions exists
            lock (_syncLockQuestionId)
            {
                foreach (int i in questionsIdsList)
                {
                    if (i >= _questionID)
                    {
                        return "Error. Some questions cannot be removed.";
                    }
                }
            }
            foreach (int i in questionsIdsList)
            {
                Question q = _db.getQuestion(i);
                q.isDeleted = true;
                _db.updateQuestion(q);
            }
            return Replies.SUCCESS;
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
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, List<Tuple<string, int>>>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            // get all tests for the given group
            List<GroupTest> groupTests = _db.getGroupTests(groupName, adminId);
            List<Tuple<string, int>> ans = new List<Tuple<string, int>>();
            // foreach test
            foreach (GroupTest gt in groupTests)
            {
                // verify how many questions a test has
                Test test = _db.getTest(gt.TestId);
                if (test == null)
                {
                    return new Tuple<string, List<Tuple<string, int>>>(DB_FAULT, null);
                }
                List<TestQuestion> tqs = _db.getTestQuestions(test.TestId);
                // how many questions have been answered
                List<GroupTestAnswer> gtas = _db.getGroupTestAnswers(groupName, adminId, test.TestId);
                int counter = 0;
                foreach (GroupTestAnswer gta in gtas)
                {
                    Answer a = _db.getAnswer(gta.AnswerId);
                    if (a == null)
                    {
                        return new Tuple<string, List<Tuple<string, int>>>(DB_FAULT, null);
                    }
                    if (a.UserId.Equals(user.UserId))
                    {
                        counter++;
                    }
                }
                if (!completed && tqs.Count > counter)
                {
                    ans.Add(new Tuple<string, int>(test.testName, test.TestId));
                }
                else if (completed && tqs.Count == gtas.Count)
                {
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
                return new Tuple<string, Question>(t.Item1, null);
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
            // save regular answer
            // save GTA
            throw new NotImplementedException();
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
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
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
            User user = getUserByInt(userUniqueInt);
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
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return NOT_AN_ADMIN;
            }
            // verify subject exist
            Subject sub = _db.getSubject(subject);
            if (sub == null)
            {
                return NOT_A_SUBJECT;
            }
            // verify all diagnoses are topics of the specified subject
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            List<Topic> subjectTopics = _db.getTopics(subject);
            foreach (string diagnosys in topicsList)
            {
                if (subjectTopics.Where(t => t.TopicId.Equals(diagnosys)).ToList().Count == 0)
                {
                    return "Error. " + diagnosys + " is not a topic of " + subject;
                }
            }
            List<Question> questions = new List<Question>();
            foreach (string topic in topicsList)
            {
                questions.AddRange(_db.getQuestions(subject, topic));
            }
            _questionsForGroupTest[user] = questions;
            return Replies.SUCCESS;
        }

        //all relevnt questions on the subject and topic the saved in the last function (saveSelectedSubjectTopic)
        public Tuple<string, List<Question>> getAllReleventQuestions(int userUniqueInt)
        {
            // verify user is logged in
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return new Tuple<string, List<Question>>(NOT_LOGGED_IN, null);
            }
            updateUserLastActionTime(user);
            // verify user is an admin
            Admin admin = _db.getAdmin(user.UserId);
            if (admin == null)
            {
                return new Tuple<string, List<Question>>(NOT_AN_ADMIN, null);
            }
            if (!_questionsForGroupTest.Keys.Contains(user))
            {
                return new Tuple<string, List<Question>>("Error. No questions available.", null);
            }
            List<Question> l = _questionsForGroupTest[user];
            _questionsForGroupTest.Remove(user);
            return new Tuple<string, List<Question>>(Replies.SUCCESS, l);
        }

        private User getUserByInt(int userUniqueInt)
        {
            List<User> matches = _usersCache.Where(u => u.uniqueInt.Equals(userUniqueInt)).ToList();
            if (matches.Count == 1)
            {
                return matches.ElementAt(0);
            }
            else
            {
                return _db.getUser(userUniqueInt);
            }
        }

        private void updateUserLastActionTime(User u)
        {
            _loggedUsers[u] = DateTime.Now;
            _usersCache.Remove(u);
            _usersCache.Add(u);
            if (_usersCache.Count == USERS_CACHE_LIMIT)
            {
                _usersCache.RemoveAt(0);
            }
        }

        private Tuple<string, string> getGroupNameAndAdminId(string s)
        {
            int i = s.LastIndexOf(GroupsMembers.CREATED_BY);
            string groupName = s.Substring(0, i);
            string adminId = s.Substring(i + GroupsMembers.CREATED_BY.Length);
            adminId = adminId.Substring(0, adminId.Length - 1);
            return new Tuple<string, string>(groupName, adminId);
        }
    }
}
