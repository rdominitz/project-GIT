using System;
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
        private const int USERS_CACHE_LIMIT = 1000;
        private const int HOURS_TO_LOGOUT = 1;
        private const int MILLISECONDS_TO_SLEEP = HOURS_TO_LOGOUT * 60 * 60 * 1000;

        private List<User> _usersCache; // each action will move the user to the last position of the cache, removing old users from the beginning
        private Dictionary<User, DateTime> _loggedUsers;

        private Dictionary<User, List<Question>> _usersTestsAnswerEveryTime;
        private Dictionary<User, List<Question>> _usersTestsAnswersAtEndRemainingQuestions;
        private Dictionary<User, List<Question>> _usersTestsAnswersAtEndAnsweredQuestions;

        private int _userUniqueInt;
        private readonly object syncLockUserUniqueInt;

        private int _questionID;
        private readonly object syncLockQuestionId;

        private readonly object syncLockGroup;

        private int _testID;

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
            _db = db;
            _logic = new LogicImpl(db);
            _userUniqueInt = 100000;
            syncLockUserUniqueInt = new object();
            _questionID = 1;
            syncLockQuestionId = new object();
            syncLockGroup = new object();
            _testID = 1;
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
            setDB();
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
                normal = true,
                text = "",
                level = Levels.DEFAULT_LVL,
                points = Questions.QUESTION_INITAL_POINTS,
                timeAdded = DateTime.Now,
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
            lock (syncLockUserUniqueInt)
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

        public string AnswerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties)
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
            User user = getUserByInt(userUniqueInt);
            if (user == null)
            {
                return USER_NOT_REGISTERED;
            }
            if (!_usersTestsAnswerEveryTime.Keys.Contains(user) && !_usersTestsAnswersAtEndRemainingQuestions.Keys.Contains(user))
            {
                return "Error. Cannot answer a question prior to requesting one.";
            }
            // verify user is logged in
            if (!_loggedUsers.ContainsKey(user))
            {
                return NOT_LOGGED_IN;
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
            User user = getUserByInt(userUniqueInt);
            if (user == null)
            {
                return new Tuple<string, List<Question>>(USER_NOT_REGISTERED, null);
            }
            updateUserLastActionTime(user);
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
                ans.Add(i.ImageId);
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
            List<string> l = new List<string>();
            foreach (string s in _subjectsTopics.Keys)
            {
                l.Add(s);
            }
            return l;
        }

        public List<string> getSubjectTopics(string subject)
        {
            return _subjectsTopics[subject];
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

        public string addQuestion(int userUniqueInt, string subject, bool isNormal, string text, List<string> qDiagnoses)
        {
            // check for illegal input values
            List<string> input = new List<string>(qDiagnoses);
            input.Add(subject);
            if (!InputTester.isValidInput(input) || text == null || text == "null")
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
            Question q = new Question { };
            lock (syncLockQuestionId)
            {
                foreach (string diagnosis in qDiagnoses)
                {
                    diagnoses.Add(new Diagnosis { QuestionId = _questionID, SubjectId = subject, TopicId = diagnosis });
                }
                q.QuestionId = _questionID;
                q.SubjectId = subject;
                q.normal = isNormal;
                q.text = "";
                q.level = Levels.DEFAULT_LVL;
                q.points = Questions.QUESTION_INITAL_POINTS;
                q.timeAdded = DateTime.Now;
                _questionID++;
            }
            foreach (Diagnosis d in diagnoses)
            {
                _db.addDiagnosis(d);
            }
            _db.addQuestion(q);
            _db.SaveChanges();
            return Replies.SUCCESS;
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            User u = _db.getUser(usernameToTurnToAdmin);
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

        public bool isLoggedIn(int userUniqueInt)
        {
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return false;
            }
            updateUserLastActionTime(user);
            return _loggedUsers.Keys.Contains(user);
        }

        public string getUserName(int userUniqueInt)
        {
            User user = getUserByInt(userUniqueInt);
            if (user == null || !_loggedUsers.ContainsKey(user))
            {
                return "";
            }
            updateUserLastActionTime(user);
            return user.userFirstName + " " + user.userLastName;
        }

        public string createGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
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
            lock (syncLockGroup)
            {
                Group group = _db.getGroup(admin.AdminId, groupName);
                if (group != null)
                {
                    return "Error. You have already created a group with that name.";
                }
                Group g = new Group { AdminId = admin.AdminId, name = groupName };
                _db.addGroup(g);
            }
            return inviteToGroup(userUniqueInt, groupName, inviteEmails, emailContent);
        }

        public string inviteToGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent)
        {
            // check for illegal input values
            List<string> input = new List<string>() { groupName };
            if (!InputTester.isValidInput(input))
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
            lock (syncLockGroup)
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
            lock (syncLockGroup)
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

        public Tuple<string, List<Question>> createTest(int userUniqueInt, string testName, string subject, List<string> topics)
        {
            return new Tuple<string,List<Question>>("temp",new List<Question>());
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
            List<Test> tests = new List<Test>();

            return new Tuple<string, List<Test>>("temp", tests);
        }

        public string addTestToGroup(int userUniqueInt, string groupName, int testId)
        {
            return "temp";
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
    }
}
