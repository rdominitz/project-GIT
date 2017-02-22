using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FakeMedTrainDBContext : DbContext, IMedTrainDBContext
    {
        private List<User> _users;
        private List<Admin> _admins;
        private List<Question> _questions;
        private List<Subject> _subjects;
        private List<Topic> _topics;
        private List<UserLevel> _userLevels;
        private List<Answer> _answers;

        public FakeMedTrainDBContext()
        {
            _users = new List<User>();
            _admins = new List<Admin>();
            _questions = new List<Question>();
            _subjects = new List<Subject>();
            _topics = new List<Topic>();
            _userLevels = new List<UserLevel>();
            _answers = new List<Answer>();
        }

        public override int SaveChanges()
        {
            return 0;
        }
        #region user
        public void addUser(User user)
        {
            List<User> matches = _users.Where(u => u.UserId.Equals(user.UserId)).ToList();
            if (matches.Count == 0)
            {
                _users.Add(user);
            }
        }

        public User getUser(string eMail)
        {
            List<User> matches = _users.Where(u => u.UserId == eMail).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public User getUser(int uniqueInt)
        {
            List<User> matches = _users.Where(u => u.uniqueInt == uniqueInt).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }
        #endregion
        #region admin
        public void addAdmin(string eMail)
        {
            if (getUser(eMail) == null)
            {
                return;
            }
            List<Admin> matches = _admins.Where(u => u.AdminId == eMail).ToList();
            if (matches.Count == 0)
            {
                _admins.Add(new Admin { AdminId = eMail });
            }
        }

        public Admin getAdmin(string eMail)
        {
            List<Admin> matches = _admins.Where(u => u.AdminId == eMail).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }
        #endregion
        #region question
        public void addQuestion(Question q)
        {
            List<Question> matches = _questions.Where(u => u.QuestionId.Equals(q.QuestionId)).ToList();
            if (matches.Count == 0)
            {
                _questions.Add(q);
            }
        }

        public Question getQuestion(int id)
        {
            List<Question> matches = _questions.Where(u => u.QuestionId == id).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public void updateQuestion(Question q) { }

        public List<Question> getQuestions(string subject, string topic)
        {
            List<Topic> matches = _topics.Where(t => t.TopicId.Equals(topic) && t.SubjectId.Equals(subject)).ToList();
            if (matches.Count != 1)
            {
                return null;
            }
            return _questions.Where(u => u.subjectName.Equals(subject) && u.diagnoses.Contains(matches[0])).ToList();
        }

        public List<Question> getNormalQuestions(string subject)
        {
            return _questions.Where(u => u.subjectName.Equals(subject) && u.normal).ToList();
        }
        #endregion
        #region user level
        public void addUserLevel(UserLevel userLevel)
        {
            List<UserLevel> matches = _userLevels.Where(u => u.userId.Equals(userLevel.userId) && u.SubjectId.Equals(userLevel.SubjectId) && u.TopicId.Equals(userLevel.TopicId)).ToList();
            if (matches.Count == 0)
            {
                _userLevels.Add(userLevel);
            }
        }
        public UserLevel getUserLevel(string eMail, string subject, string topic)
        {
            List<UserLevel> matches = _userLevels.Where(u => u.userId.Equals(eMail) && u.SubjectId.Equals(subject) && u.TopicId.Equals(topic)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public void updateUserLevel(UserLevel ul) { }
        #endregion
        public void addAnswer(Answer a)
        {
            if (getQuestion(a.questionId) == null || getUser(a.userId) == null)
            {
                return;
            }
            List<Answer> matches = _answers.Where(u => u.userId.Equals(a.userId) && u.questionId.Equals(a.questionId) && u.timeAdded.Equals(a.timeAdded)).ToList();
            if (matches.Count == 0)
            {
                _answers.Add(a);
            }
        }
        #region topic
        public void addTopic(string subject, string topic)
        {
            List<Subject> subjects = _subjects.Where(u => u.SubjectId.Equals(subject)).ToList();
            if (subjects.Count != 1)
            {
                return;
            }
            List<Topic> matches = _topics.Where(u => u.SubjectId.Equals(subject) && u.TopicId.Equals(topic)).ToList();
            if (matches.Count == 0)
            {
                Topic t = new Topic { SubjectId = subject, TopicId = topic };
                _topics.Add(t);
                subjects[0].topics.Add(t);
            }
        }

        public void addTopic(Topic t)
        {
            List<Topic> matches = _topics.Where(u => u.SubjectId.Equals(t.subject) && u.TopicId.Equals(t.TopicId)).ToList();
            if (matches.Count == 0)
            {
                _topics.Add(t);
            }
        }

        public Topic getTopic(string subject, string topic)
        {
            List<Topic> matches = _topics.Where(t => t.TopicId.Equals(topic) && t.SubjectId.Equals(subject)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public List<Topic> getTopics(string subject)
        {
            return _topics.Where(t => t.SubjectId.Equals(subject)).ToList(); ;
        }
        #endregion
        #region subject
        public void addSubject(string subject)
        {
            List<Subject> matches = _subjects.Where(t => t.SubjectId.Equals(subject)).ToList();
            if (matches.Count == 0)
            {
                _subjects.Add(new Subject { SubjectId = subject });
            }
        }

        public void addSubject(Subject s)
        {
            List<Subject> matches = _subjects.Where(u => u.SubjectId.Equals(s.SubjectId)).ToList();
            if (matches.Count == 0)
            {
                _subjects.Add(s);
            }
        }

        public Subject getSubject(string subject)
        {
            List<Subject> matches = _subjects.Where(t => t.SubjectId.Equals(subject)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public List<Subject> getSubjects()
        {
            return _subjects;
        }
        #endregion
    }
}
