using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MedTrainDBContext : DbContext, IMedTrainDBContext
    {
        public IDbSet<User> Users { get; set; }
        public IDbSet<Admin> Admins { get; set; }
        public IDbSet<Subject> Subjects { get; set; }
        public IDbSet<Topic> Topics { get; set; }
        public IDbSet<Question> Questions { get; set; }
        public IDbSet<Test> Tests { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<Answer> Answers { get; set; }
        public IDbSet<UserLevel> UsersLevels { get; set; }
        public IDbSet<UserGroupTest> UsersGroupsTests { get; set; }
        public IDbSet<GroupTestAnswer> GroupsTestsQuestionsAnswers { get; set; }

        public MedTrainDBContext()
            : base("MedTrainDB")
        {
            //Database.SetInitializer<MedTrainDBContext>(new CreateDatabaseIfNotExists<MedTrainDBContext>());
            Database.SetInitializer<MedTrainDBContext>(new DropCreateDatabaseIfModelChanges<MedTrainDBContext>());
            //Database.SetInitializer<MedTrainDBContext>(new DropCreateDatabaseAlways<MedTrainDBContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        #region user
        public void addUser(User u)
        {
            if (Users.Find(u.UserId) != null)
            {
                return;
            }
            Users.Add(u);
            SaveChanges();
        }

        public User getUser(string eMail)
        {
            return Users.Find(eMail);
        }

        public User getUser(int uniqueInt)
        {
            var query = from u in Users
                        where (u.uniqueInt == uniqueInt)
                        select u;
            if (query.Count() == 0)
            {
                return null;
            }
            return query.ToList().ElementAt(0);
        }
        #endregion
        #region admin
        public void addAdmin(string eMail)
        {
            User u = Users.Find(eMail);
            if (u == null)
            {
                return;
            }
            if (Admins.Find(eMail) != null)
            {
                return;
            }
            Admins.Add(new Admin { AdminId = eMail, user = u });
            SaveChanges();
        }

        public Admin getAdmin(string eMail)
        {
            return Admins.Find(new object[1] { eMail });
        }
        #endregion
        #region question
        public void addQuestion(Question q)
        {
            if (Questions.Find(q.QuestionId) != null)
            {
                return;
            }
            Questions.Add(q);
            SaveChanges();
        }

        public Question getQuestion(int id)
        {
            return Questions.Find(new object[1] { id });
        }

        public void updateQuestion(Question q)
        {
            Entry(q).State = System.Data.Entity.EntityState.Modified;
        }

        public List<Question> getQuestions(string subject, string topic)
        {
            Topic t = getTopic(subject, topic);
            if (t == null)
            {
                return new List<Question>();
            }
            var query = from q in Questions
                        where q.subjectName.Equals(subject)
                        select q;
            var l = query.ToList();
            List<Question> res = new List<Question>();
            foreach (Question q in l)
            {
                foreach (Topic top in q.diagnoses)
                {
                    if (top.TopicId.Equals(topic))
                    {
                        res.Add(q);
                        break;
                    }
                }
            }
            return res;
        }

        public List<Question> getNormalQuestions(string subject)
        {
            var query = from q in Questions
                        where q.subjectName.Equals(subject) && q.normal
                        select q;
            return query.ToList();
        }
        #endregion
        #region user level
        public void addUserLevel(UserLevel userLevel)
        {
            if (UsersLevels.Find(userLevel.userId, userLevel.SubjectId, userLevel.TopicId) != null)
            {
                return;
            }
            UsersLevels.Add(userLevel);
        }
        public UserLevel getUserLevel(string eMail, string subject, string topic)
        {
            return UsersLevels.Find(eMail, subject, topic);
        }

        public void updateUserLevel(UserLevel ul)
        {
            Entry(ul).State = System.Data.Entity.EntityState.Modified;
        }
        #endregion
        public void addAnswer(Answer a)
        {
            if (Answers.Find(a.questionId, a.userId, a.timeAdded) != null)
            {
                return;
            }
            Answers.Add(a);
            SaveChanges();
        }
        #region topic
        public void addTopic(string subject, string topic)
        {
            if (Topics.Find(topic, subject) != null)
            {
                return;
            }
            Subject s = Subjects.Find(subject);
            if (s == null)
            {
                return;
            }
            Topic t = new Topic { SubjectId = subject, TopicId = topic, timeAdded = DateTime.Now, subject = s };
            Topics.Add(t);
            s.topics.Add(t);
            Entry(s).State = System.Data.Entity.EntityState.Modified;
            SaveChanges();
        }

        public void addTopic(Topic t)
        {
            if (Topics.Find(t.TopicId, t.SubjectId) != null)
            {
                return;
            }
            Topics.Add(t);
            SaveChanges();
        }

        public Topic getTopic(string subject, string topic)
        {
            return Topics.Find(topic, subject);
        }

        public List<Topic> getTopics(string subject)
        {
            var query = from t in Topics
                        where t.SubjectId.Equals(subject)
                        select t;
            return query.ToList();
        }
        #endregion
        #region subject
        public void addSubject(string subject)
        {
            if (Subjects.Find(subject) != null)
            {
                return;
            }
            Subjects.Add(new Subject { SubjectId = subject, timeAdded = DateTime.Now, topics = new List<Topic>() });
            SaveChanges();
        }

        public void addSubject(Subject s)
        {
            if (Subjects.Find(s.SubjectId) != null)
            {
                return;
            }
            Subjects.Add(s);
            SaveChanges();
        }

        public Subject getSubject(string subject)
        {
            return Subjects.Find(subject);
        }

        public List<Subject> getSubjects()
        {
            var query = from s in Subjects
                        select s;
            return query.ToList();
        }
        #endregion
    }
}
