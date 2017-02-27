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
        private const string GENERAL_INPUT_ERROR = "There is a null, the string \"null\" or an empty string as an input.";
        private const string INVALID_EMAIL = "Invalid eMail address.";
        private const string ILLEGAL_PASSWORD = "Illegal password. Password must be 5 to 15 characters long and consist of only letters and numbers.";
        private const string INVALID_TEMPORAL_PASSWORD = "Bad cookie. Could not identify user.";
        private const string NOT_LOGGED_IN = "User is not logged in.";
        private const string EMAIL_IN_USE = "This eMail address is already in use.";
        private const string USER_NOT_REGISTERED = "User is not registered.";
        private const string NOT_AN_ADMIN = "Error. only an admin has the required permissions to perform this action.";
        private const string CERTAINTY_LEVEL_ERROR = "Error. Certainty levels must be between 1 to 10";
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

        private int _groupID;

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
            _groupID = 1;
            _testID = 1;
            _subjectsTopics = new Dictionary<string, List<string>>();
            Thread setSubjectsTopicsThread = new Thread(new ThreadStart(setSubjectsAndTopics));
            setSubjectsTopicsThread.Start();
            Thread removeUsersThread = new Thread(new ThreadStart(removeNonActiveUsers));
            removeUsersThread.Start();
            Thread.Sleep(40000);
            User a = new User
            {
                uniqueInt = _userUniqueInt,
                UserId = "defaultadmin@gmail.com",
                userFirstName = "default",
                userLastName = "admin",
                userMedicalTraining = Users.medicalTrainingLevels[0],
                userPassword = "password"
            };
            _userUniqueInt++;
            _db.addUser(a);
            _db.addAdmin("defaultadmin@gmail.com");
            //setDB();
            //autoGeneratedQuestionScenario();
            //autoGeneratedTestAnswerEveryTime();
            //autoGeneratedTestAnswerAtEnd();
        }

        public void setDB()
        {
            // set user
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
            // set subject
            Subject s = new Subject { SubjectId = "Chest x-Rays", timeAdded = DateTime.Now, topics = new List<Topic>() };
            _db.addSubject(s);
            // set topic
            Topic t1 = new Topic { TopicId = "Normal", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(t1);
            Topic t2 = new Topic { TopicId = "Cavitary Lesion", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(t2);
            Topic t3 = new Topic { TopicId = "Interstitial opacities", SubjectId = "Chest x-Rays", timeAdded = DateTime.Now };
            _db.addTopic(t3);
            _subjectsTopics["Chest x-Rays"] = new List<string>() { "Cavitary Lesion", "Interstitial opacities" };
            Question q1 = new Question
            {
                QuestionId = _questionID,
                subjectName = "Chest x-Rays",
                normal = true,
                text = "",
                level = Levels.DEFAULT_LVL,
                diagnoses = new List<Topic>() { t1 },
                timesAnswered = 0,
                timesAnsweredCorrectly = 0,
                timeAdded = DateTime.Now,
                images = new List<Image>()
            };
            Image i11 = new Image
            {
                ImageId = "../Images/q1_2_lat.jpg",
                QuestionId = _questionID,
            };
            Image i12 = new Image
            {
                ImageId = "../Images/q1_2_pa.png",
                QuestionId = _questionID,
            };
            _questionID++;
            q1.images.Add(i11);
            q1.images.Add(i12);
            _db.addQuestion(q1);
            Question q2 = new Question
            {
                QuestionId = _questionID,
                subjectName = "Chest x-Rays",
                normal = true,
                text = "",
                level = Levels.DEFAULT_LVL,
                diagnoses = new List<Topic>() { t1 },
                timesAnswered = 0,
                timesAnsweredCorrectly = 0,
                timeAdded = DateTime.Now,
                images = new List<Image>()
            };
            Image i21 = new Image
            {
                ImageId = "../Images/q2_17_Lat.png",
                QuestionId = _questionID,
            };
            Image i22 = new Image
            {
                ImageId = "../Images/q2_17_PA.png",
                QuestionId = _questionID,
            };
            _questionID++;
            q2.images.Add(i21);
            q2.images.Add(i22);
            _db.addQuestion(q2);
        }

        public void autoGeneratedQuestionScenario()
        {
            Console.WriteLine("AGQ Start");
            login("user@gmail.com", "password");
            getAutoGeneratedQuesstion(1, "Chest x-Rays", "Normal");
            Question q = getNextQuestion(1).Item2;
            while (q != null)
            {
                string str = AnswerAQuestion(1, q.QuestionId, true, 7, new List<string>(), new List<int>());
                if (str.Equals(Replies.SHOW_ANSWER))
                {
                    Console.WriteLine("Displaying answer for question " + q.QuestionId + " and a continue botton to get the next question");
                }
                q = getNextQuestion(1).Item2;
            }
            Console.WriteLine("End of session, continue botton will redirect to user home screen.");
            Console.WriteLine("AGQ End");
        }

        public void autoGeneratedTestAnswerEveryTime()
        {
            Console.WriteLine("AGT True Start");
            login("user@gmail.com", "password");
            getAutoGeneratedTest(1, "Chest x-Rays", "Normal", 2, true);
            Question q = getNextQuestion(1).Item2;
            while (q != null)
            {
                string str = AnswerAQuestion(1, q.QuestionId, true, 7, new List<string>(), new List<int>());
                if (str.Equals(Replies.SHOW_ANSWER))
                {
                    Console.WriteLine("Displaying answer for question " + q.QuestionId + " and a continue botton to get the next question");
                }
                q = getNextQuestion(1).Item2;
            }
            Console.WriteLine("End of session, continue botton will redirect to user home screen.");
            Console.WriteLine("AGT True End");
        }

        public void autoGeneratedTestAnswerAtEnd()
        {
            Console.WriteLine("AGT False Start");
            login("user@gmail.com", "password");
            getAutoGeneratedTest(1, "Chest x-Rays", "Normal", 2, false);
            Question q = getNextQuestion(1).Item2;
            while (q != null)
            {
                string str = AnswerAQuestion(1, q.QuestionId, true, 7, new List<string>(), new List<int>());
                if (str.Equals(Replies.NEXT))
                {
                    Console.WriteLine("Displaying a continue botton to get the next question");
                }
                q = getNextQuestion(1).Item2;
            }
            Console.WriteLine("End of session, continue botton will redirect to test answers.");
            List<Question> l = getAnsweres(1).Item2;
            foreach (Question question in l)
            {
                Console.WriteLine("Displaying answer for question " + question.QuestionId);
            }
            Console.WriteLine("AGT False End");
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
                return new Tuple<string,int>(GENERAL_INPUT_ERROR, -1);
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
                return new Tuple<string,int>(INVALID_EMAIL, -1);
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
                return new Tuple<string,int>("Wrong password", -1);
            }
            // place user at the end of the cache (to be the last one to be removed)
            _usersCache.Remove(u);
            _usersCache.Add(u);
            // addd user to logged users list
            updateUserLastActionTime(u);
            return new Tuple<string,int>(Replies.SUCCESS, u.uniqueInt);
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
                if (_db.getTopic(q.subjectName, s) == null)
                {
                    return "Error. diagnosis " + s + " is invalid for subject " + q.subjectName;
                }
            }
            //List<Topic> qDiagnoses = q.diagnoses.ToList();
            foreach (Topic s in q.diagnoses)
            {
                UserLevel userLevel = _db.getUserLevel(user.UserId, q.subjectName, s.TopicId);
                if (userLevel == null)
                {
                    userLevel = new UserLevel { userId = user.UserId, SubjectId = q.subjectName, TopicId = s.TopicId, level = Levels.DEFAULT_LVL, timesAnswered = 0, timesAnsweredCorrectly = 0 };
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
            _db.addSubject(subject);
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
            List<Topic> topics = new List<Topic>();
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
            foreach (string diagnosys in qDiagnoses)
            {
                topics.Add(_db.getTopic(subject, diagnosys));
            }
            Question q = new Question
            {
                QuestionId = _questionID,
                subjectName = subject,
                subject = sub,
                normal = isNormal,
                text = "",
                level = Levels.DEFAULT_LVL,
                diagnoses = topics,
                timesAnswered = 0,
                timesAnsweredCorrectly = 0,
                timeAdded = DateTime.Now,
            };
            _questionID++;
            _db.addQuestion(q);
            _db.SaveChanges();
            return Replies.SUCCESS;
        }

        public string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin)
        {
            _db.addAdmin(usernameToTurnToAdmin);
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
    }
}
