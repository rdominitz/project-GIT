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
        public IDbSet<QuestionImage> Images { get; set; }
        public IDbSet<Diagnosis> Diagnoses { get; set; }
        public IDbSet<Test> Tests { get; set; }
        public IDbSet<TestQuestion> TestsQuestions { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<GroupMember> GroupsMembers { get; set; }
        public IDbSet<GroupTest> GroupsTests { get; set; }
        public IDbSet<Answer> Answers { get; set; }
        public IDbSet<DiagnosisCertainty> DiagnosesCertainties { get; set; }
        public IDbSet<UserLevelAnswer> UsersLevelsWhenAnswring { get; set; }
        public IDbSet<UserLevel> UsersLevels { get; set; }
        public IDbSet<UserGroupTest> UsersGroupsTests { get; set; }
        public IDbSet<GroupTestAnswer> GroupsTestsAnswers { get; set; }

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
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        public int getMillisecondsToSleep()
        {
            return 1 * 60 * 1000;
        }

        #region user
        public void addUser(User u)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Users.Find(u.UserId) != null)
                {
                    return;
                }
                db.Users.Add(u);
                db.SaveChanges();
            }
        }

        public User getUser(string UserId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from u in db.Users
                            where u.UserId.Equals(UserId)
                            select u;
                return query.Count() == 1 ? query.ToList()[0] : null;
            }
        }

        public User getUser(int uniqueInt)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from u in db.Users
                            where (u.uniqueInt == uniqueInt)
                            select u;
                if (query.Count() == 0)
                {
                    return null;
                }
                return query.ToList().ElementAt(0);
            }
        }
        #endregion
        #region admin
        public void addAdmin(Admin a)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Users.Find(a.AdminId) == null || db.Admins.Find(a.AdminId) != null)
                {
                    return;
                }
                db.Admins.Add(new Admin { AdminId = a.AdminId });
                db.SaveChanges();
            }
        }

        public Admin getAdmin(string UserId)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Admins.Find(UserId);
            }
        }
        #endregion
        #region subject
        public void addSubject(Subject s)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Subjects.Find(s.SubjectId) != null)
                {
                    return;
                }
                db.Subjects.Add(new Subject { SubjectId = s.SubjectId, timeAdded = DateTime.Now });
                db.SaveChanges();
            }
        }

        public Subject getSubject(string subject)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Subjects.Find(subject);
            }
        }

        public List<Subject> getSubjects()
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Subjects.ToList();
            }
        }
        #endregion
        #region topic
        public void addTopic(Topic t)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Subjects.Find(t.SubjectId) == null || db.Topics.Find(t.TopicId, t.SubjectId) != null)
                {
                    return;
                }
                db.Topics.Add(t);
                db.SaveChanges();
            }
        }

        public Topic getTopic(string subject, string topic)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Topics.Find(topic, subject);
            }
        }

        public List<Topic> getTopics(string subject)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from t in db.Topics
                            where t.SubjectId.Equals(subject)
                            select t;
                return query.ToList();
            }
        }
        #endregion
        #region question
        public void addQuestion(Question q)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Questions.Find(q.QuestionId) != null)
                {
                    return;
                }
                db.Questions.Add(q);
                db.SaveChanges();
            }
        }

        public Question getQuestion(int id)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Questions.Find(id);
            }
        }

        public void updateQuestion(Question q)
        {
            using (var db = new MedTrainDBContext())
            {
                db.Entry(q).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<Question> getQuestions(string subject, string topic)
        {
            using (var db = new MedTrainDBContext())
            {
                if (getTopic(subject, topic) == null)
                {
                    return new List<Question>();
                }
                var query = from q in db.Questions
                            where q.SubjectId.Equals(subject) && !q.isDeleted
                            select q;
                List<Question> ans = new List<Question>();
                foreach (Question q in query)
                {
                    var diagnoses = from d in db.Diagnoses
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
        }

        public List<Question> getNormalQuestions(string subject)
        {
            return getQuestions(subject, Constants.Topics.NORMAL);
        }
        #endregion
        #region image
        public void addImage(QuestionImage i)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Questions.Find(i.QuestionId) == null || db.Images.Find(i.ImageId, i.QuestionId) != null)
                {
                    return;
                }
                db.Images.Add(i);
                db.SaveChanges();
            }
        }

        public List<QuestionImage> getQuestionImages(int qId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from i in db.Images
                            where i.QuestionId == qId
                            select i;
                return query.ToList();
            }
        }
        #endregion
        #region diagnosis
        public void addDiagnosis(Diagnosis d)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Subjects.Find(d.SubjectId) == null || db.Topics.Find(d.TopicId, d.SubjectId) == null ||
                    db.Questions.Find(d.QuestionId) == null || db.Diagnoses.Find(d.TopicId, d.SubjectId, d.QuestionId) != null)
                {
                    return;
                }
                db.Diagnoses.Add(d);
                db.SaveChanges();
            }
        }

        public List<Diagnosis> getQuestionDiagnoses(int qId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from d in db.Diagnoses
                            where d.QuestionId == qId
                            select d;
                return query.ToList();
            }
        }
        #endregion
        #region answer
        public void addAnswer(Answer a)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Questions.Find(a.QuestionId) == null || db.Users.Find(a.UserId) == null ||
                    db.Answers.Find(a.AnswerId) != null)
                {
                    return;
                }
                var query = from ans in db.Answers
                            where ans.QuestionId == a.QuestionId && ans.UserId.Equals(a.UserId) &&
                            ans.timeAdded.Equals(a.timeAdded)
                            select ans;
                if (query.ToList().Count != 0)
                {
                    return;
                }
                db.Answers.Add(a);
                db.SaveChanges();
            }
        }

        public Answer getAnswer(int answerId)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Answers.Find(answerId);
            }
        }
        #endregion
        #region diagnosis certainty
        public void addDiagnosisCertainty(DiagnosisCertainty dc)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Answers.Find(dc.AnswerId) == null || db.Subjects.Find(dc.SubjectId) == null ||
                    db.Topics.Find(dc.TopicId, dc.SubjectId) == null ||
                    db.DiagnosesCertainties.Find(dc.AnswerId, dc.TopicId, dc.SubjectId) != null)
                {
                    return;
                }
                db.DiagnosesCertainties.Add(dc);
                db.SaveChanges();
            }
        }
        #endregion
        #region user level answer
        public void addUserLevelAnwer(UserLevelAnswer ula)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Answers.Find(ula.AnswerId) == null || db.Subjects.Find(ula.SubjectId) == null ||
                    db.Topics.Find(ula.TopicId, ula.SubjectId) == null ||
                    db.UsersLevelsWhenAnswring.Find(ula.AnswerId, ula.TopicId, ula.SubjectId) != null)
                {
                    return;
                }
                db.UsersLevelsWhenAnswring.Add(ula);
                db.SaveChanges();
            }
        }
        #endregion
        #region user level
        public void addUserLevel(UserLevel ul)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.UsersLevels.Find(ul.UserId, ul.TopicId, ul.SubjectId) != null)
                {
                    return;
                }
                db.UsersLevels.Add(ul);
                db.SaveChanges();
            }
        }

        public UserLevel getUserLevel(string eMail, string subject, string topic)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.UsersLevels.Find(eMail, topic, subject);
            }
        }

        public void updateUserLevel(UserLevel ul)
        {
            using (var db = new MedTrainDBContext())
            {
                db.Entry(ul).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion
        #region group
        public void addGroup(Group g)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Admins.Find(g.AdminId) == null || db.Groups.Find(g.AdminId, g.name) != null)
                {
                    return;
                }
                db.Groups.Add(g);
                db.SaveChanges();
            }
        }

        public Group getGroup(string adminId, string groupName)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Groups.Find(adminId, groupName);
            }
        }

        public void removeGroup(Group g)
        {
            using (var db = new MedTrainDBContext())
            {
                Group group = db.Groups.Find(g.AdminId, g.name);
                db.Groups.Remove(group);
                db.SaveChanges();
            }
        }

        public List<Group> getAdminsGroups(string adminId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from g in db.Groups
                            where g.AdminId.Equals(adminId)
                            select g;
                return query.ToList();
            }
        }
        #endregion
        #region group member
        public void addGroupMember(GroupMember gm)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Groups.Find(gm.AdminId, gm.GroupName) == null || db.GroupsMembers.Find(gm.GroupName, gm.AdminId, gm.UserId) != null)
                {
                    return;
                }
                db.GroupsMembers.Add(gm);
                db.SaveChanges();
            }
        }

        public void removeGroupMembers(Group g)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gm in db.GroupsMembers
                            where gm.GroupName.Equals(g.name) && gm.AdminId.Equals(g.AdminId)
                            select gm;
                foreach (GroupMember gm in query)
                {
                    db.GroupsMembers.Remove(gm);
                }
            }
        }

        public List<GroupMember> getUserGroups(string userId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gm in db.GroupsMembers
                            where gm.UserId.Equals(userId) && gm.invitationAccepted
                            select gm;
                return query.ToList();
            }
        }

        public List<GroupMember> getUserInvitations(string userId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gm in db.GroupsMembers
                            where gm.UserId.Equals(userId) && !gm.invitationAccepted
                            select gm;
                return query.ToList();
            }
        }

        public bool hasInvitation(string userId, string groupName, string adminId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gm in db.GroupsMembers
                            where gm.UserId.Equals(userId) && gm.GroupName.Equals(groupName) &&
                                gm.AdminId.Equals(adminId) && !gm.invitationAccepted
                            select gm;
                return query.Count() == 1;
            }
        }

        public GroupMember getGroupMemberInvitation(string userId, string groupName, string adminId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gm in db.GroupsMembers
                            where gm.UserId.Equals(userId) && gm.GroupName.Equals(groupName) &&
                                gm.AdminId.Equals(adminId) && !gm.invitationAccepted
                            select gm;
                return query.ToList()[0];
            }
        }

        public void updateGroupMember(GroupMember gm)
        {
            using (var db = new MedTrainDBContext())
            {
                db.Entry(gm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<GroupMember> getGroupMembers(string groupName, string adminId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gm in db.GroupsMembers
                            where gm.GroupName.Equals(groupName) && gm.AdminId.Equals(adminId) && gm.invitationAccepted
                            select gm;
                return query.ToList();
            }
        }

        public void removeGroupMember(GroupMember gm)
        {
            using (var db = new MedTrainDBContext())
            {
                db.GroupsMembers.Remove(gm);
            }
        }
        #endregion
        #region test
        public void addTest(Test t)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Admins.Find(t.AdminId) == null || db.Subjects.Find(t.subject) == null || db.Tests.Find(t.TestId) != null)
                {
                    return;
                }
                db.Tests.Add(t);
                db.SaveChanges();
            }
        }

        public List<Test> getAllTests(string subject)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from t in db.Tests
                            where t.subject.Equals(subject)
                            select t;
                return query.ToList();
            }
        }

        public Test getTest(int testId)
        {
            using (var db = new MedTrainDBContext())
            {
                return db.Tests.Find(testId);
            }
        }
        #endregion
        #region group test
        public void addGroupTest(GroupTest gt)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Admins.Find(gt.AdminId) == null || db.Groups.Find(gt.AdminId, gt.GroupName) == null ||
                    db.Tests.Find(gt.TestId) == null || db.GroupsTests.Find(gt.AdminId, gt.GroupName, gt.TestId) != null)
                {
                    return;
                }
                db.GroupsTests.Add(gt);
                db.SaveChanges();
            }
        }

        public List<GroupTest> getGroupTests(string groupName, string adminId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gt in db.GroupsTests
                            where gt.GroupName.Equals(groupName) && gt.AdminId.Equals(adminId)
                            select gt;
                return query.ToList();
            }
        }
        #endregion
        #region test question
        public void addTestQuestion(TestQuestion tq)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Tests.Find(tq.TestId) == null || db.Questions.Find(tq.QuestionId) == null ||
                    db.Questions.Find(tq.QuestionId).isDeleted || db.TestsQuestions.Find(tq.TestId, tq.QuestionId) != null)
                {
                    return;
                }
                db.TestsQuestions.Add(tq);
                db.SaveChanges();
            }
        }

        public List<TestQuestion> getTestQuestions(int testId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from tq in db.TestsQuestions
                            where tq.TestId == testId
                            select tq;
                return query.ToList();
            }
        }
        #endregion
        #region group test answer
        public void addGroupTestAnswer(GroupTestAnswer gta)
        {
            using (var db = new MedTrainDBContext())
            {
                if (db.Users.Find(gta.UserId) == null || db.Admins.Find(gta.AdminId) == null ||
                    db.Groups.Find(gta.AdminId, gta.GroupName) == null || db.Tests.Find(gta.TestId) == null ||
                    db.Answers.Find(gta.AnswerId) == null ||
                    db.GroupsTestsAnswers.Find(gta.GroupName, gta.AdminId, gta.TestId, gta.AnswerId) != null)
                {
                    return;
                }
                db.GroupsTestsAnswers.Add(gta);
                db.SaveChanges();
            }
        }

        public void removeGroupTestAnswers(string groupName, string adminId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gta in db.GroupsTestsAnswers
                            where gta.GroupName.Equals(groupName) && gta.AdminId.Equals(adminId)
                            select gta;
                foreach (GroupTestAnswer gta in query)
                {
                    db.GroupsTestsAnswers.Remove(gta);
                }
                db.SaveChanges();
            }
        }

        public List<GroupTestAnswer> getGroupTestAnswers(string groupName, string adminId, int testId)
        {
            using (var db = new MedTrainDBContext())
            {
                var query = from gta in db.GroupsTestsAnswers
                            where gta.GroupName.Equals(groupName) && gta.AdminId.Equals(adminId) &&
                                gta.TestId == testId
                            select gta;
                return query.ToList();
            }
        }
        #endregion
    }
}
