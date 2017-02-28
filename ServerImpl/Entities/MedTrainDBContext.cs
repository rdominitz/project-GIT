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
        public IDbSet<Image> Images { get; set; }
        public IDbSet<Diagnosis> Diagnoses { get; set; }
        public IDbSet<Test> Tests { get; set; }
        public IDbSet<TestQuestion> TestsQuestions { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<GroupMember> GroupsMembers { get; set; }
        public IDbSet<GroupTest> GroupsTests { get; set; }
        public IDbSet<Answer> Answers { get; set; }
        public IDbSet<DiagnosisCertainty> DiagnosesCertainties {get; set;}
        public IDbSet<UserLevelAnswer> UsersLevelsWhenAnswring { get; set; }
        public IDbSet<UserLevel> UsersLevels { get; set; }
        public IDbSet<UserGroupTest> UsersGroupsTests { get; set; }
        public IDbSet<GroupTestAnswer> GroupsTestsQuestionsAnswers { get; set; }

        public MedTrainDBContext()
            : base("MedTrainDB")
        {
            //Database.SetInitializer<MedTrainDBContext>(new CreateDatabaseIfNotExists<MedTrainDBContext>());
            //Database.SetInitializer<MedTrainDBContext>(new DropCreateDatabaseIfModelChanges<MedTrainDBContext>());
            Database.SetInitializer<MedTrainDBContext>(new DropCreateDatabaseAlways<MedTrainDBContext>());
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

        public User getUser(string UserId)
        {
            return Users.Find(UserId);
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
        public void addAdmin(Admin a)
        {
            User u = Users.Find(a.AdminId);
            if (u == null)
            {
                return;
            }
            if (Admins.Find(a.AdminId) != null)
            {
                return;
            }
            Admins.Add(new Admin { AdminId = a.AdminId });
            SaveChanges();
        }

        public Admin getAdmin(string UserId)
        {
            return Admins.Find(UserId);
        }
        #endregion
        #region subject
        public void addSubject(Subject s)
        {
            if (Subjects.Find(s.SubjectId) != null)
            {
                return;
            }
            Subjects.Add(new Subject { SubjectId = s.SubjectId, timeAdded = DateTime.Now });
            SaveChanges();
        }

        public Subject getSubject(string subject)
        {
            return Subjects.Find(subject);
        }

        public List<Subject> getSubjects()
        {
            return Subjects.ToList();
        }
        #endregion
        #region topic
        public void addTopic(Topic t)
        {
            if (Subjects.Find(t.SubjectId) == null || Topics.Find(t.TopicId, t.SubjectId) != null)
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
            return Questions.Find(id);
        }

        public void updateQuestion(Question q)
        {
            Entry(q).State = System.Data.Entity.EntityState.Modified;
            SaveChanges();
        }

        public List<Question> getQuestions(string subject, string topic)
        {
            Topic t = getTopic(subject, topic);
            if (t == null)
            {
                return new List<Question>();
            }
            var query = from q in Questions
                        where q.SubjectId.Equals(subject)
                        select q;
            List<Question> ans = new List<Question>();
            foreach (Question q in query)
            {
                var diagnoses = from d in Diagnoses
                                where d.QuestionId == q.QuestionId
                                select d;
                foreach (Diagnosis d in diagnoses)
                {
                    if (d.TopicId.Equals(topic))
                    {
                        ans.Add(q);
                        break;
                    }
                }
            }
            return ans;
        }

        public List<Question> getNormalQuestions(string subject)
        {
            return getQuestions(subject, Constants.Topics.NORMAL);
        }
        #endregion
        #region image
        public void addImage(Image i)
        {
            if (Questions.Find(i.QuestionId) == null || Images.Find(i.ImageId, i.QuestionId) != null)
            {
                return;
            }
            Images.Add(i);
            SaveChanges();
        }

        public List<Image> getQuestionImages(int qId)
        {
            var query = from i in Images
                        where i.QuestionId == qId
                        select i;
            return query.ToList();
        }
        #endregion
        #region diagnosis
        public void addDiagnosis(Diagnosis d)
        {
            if (Subjects.Find(d.SubjectId) == null || Topics.Find(d.TopicId, d.SubjectId) == null ||
                Questions.Find(d.QuestionId) == null || Diagnoses.Find(d.TopicId, d.SubjectId, d.QuestionId) != null)
            {
                return;
            }
            Diagnoses.Add(d);
            SaveChanges();
        }

        public List<Diagnosis> getQuestionDiagnoses(int qId)
        {
            var query = from d in Diagnoses
                        where d.QuestionId == qId
                        select d;
            return query.ToList();
        }
        #endregion
        #region answer
        public void addAnswer(Answer a)
        {
            if (Questions.Find(a.QuestionId) == null || Users.Find(a.UserId) == null || Answers.Find(a.AnswerId) != null)
            {
                return;
            }
            var query = from ans in Answers
                        where ans.QuestionId == a.QuestionId && ans.UserId.Equals(a.UserId) && ans.timeAdded.Equals(a.timeAdded)
                        select ans;
            if (query.ToList().Count != 0)
            {
                return;
            }
            Answers.Add(a);
            SaveChanges();
        }
        #endregion
        #region diagnosis certainty
        public void addDiagnosisCertainty(DiagnosisCertainty dc)
        {
            if (Answers.Find(dc.AnswerId) == null || Subjects.Find(dc.SubjectId) == null ||
                Topics.Find(dc.TopicId, dc.SubjectId) == null ||
                DiagnosesCertainties.Find(dc.AnswerId, dc.TopicId, dc.SubjectId) != null)
            {
                return;
            }
            DiagnosesCertainties.Add(dc);
            SaveChanges();
        }
        #endregion
        #region user level answer
        public void addUserLevelAnwer(UserLevelAnswer ula)
        {
            if (Answers.Find(ula.AnswerId) == null || Subjects.Find(ula.SubjectId) == null ||
                Topics.Find(ula.TopicId, ula.SubjectId) == null ||
                UsersLevelsWhenAnswring.Find(ula.AnswerId, ula.TopicId, ula.SubjectId) != null)
            {
                return;
            }
            UsersLevelsWhenAnswring.Add(ula);
            SaveChanges();
        }
        #endregion
        #region user level
        public void addUserLevel(UserLevel ul)
        {
            if (UsersLevels.Find(ul.UserId, ul.TopicId, ul.SubjectId) != null)
            {
                return;
            }
            UsersLevels.Add(ul);
            SaveChanges();
        }
        
        public UserLevel getUserLevel(string eMail, string subject, string topic)
        {
            return UsersLevels.Find(eMail, topic, subject);
        }

        public void updateUserLevel(UserLevel ul)
        {
            Entry(ul).State = System.Data.Entity.EntityState.Modified;
            SaveChanges();
        }
        #endregion
    }
}
