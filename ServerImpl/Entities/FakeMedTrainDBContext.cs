using Constants;
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
        private List<Subject> _subjects;
        private List<Topic> _topics;
        private List<Question> _questions;
        private List<Image> _images;
        private List<Diagnosis> _diagnoses;
        private List<Answer> _answers;
        private List<DiagnosisCertainty> _diagnosesCertainties;
        private List<UserLevelAnswer> _usersLevelsWhenAnswering;
        private List<UserLevel> _usersLevels;
        private List<Group> _groups;
        private List<GroupMember> _groupsMembers;
        private List<Test> _tests;
        private List<GroupTest> _groupsTests;

        public FakeMedTrainDBContext()
        {
            _users = new List<User>();
            _admins = new List<Admin>();
            _subjects = new List<Subject>();
            _topics = new List<Topic>();
            _questions = new List<Question>();
            _images = new List<Image>();
            _diagnoses = new List<Diagnosis>();
            _answers = new List<Answer>();
            _diagnosesCertainties = new List<DiagnosisCertainty>();
            _usersLevelsWhenAnswering = new List<UserLevelAnswer>();
            _usersLevels = new List<UserLevel>();
            _groups = new List<Group>();
            _groupsMembers = new List<GroupMember>();
            _tests = new List<Test>();
            _groupsTests = new List<GroupTest>();
        }

        //public IDbSet<Test> Tests { get; set; }
        //public IDbSet<Group> Groups { get; set; }
        //public IDbSet<UserGroupTest> UsersGroupsTests { get; set; }
        //public IDbSet<GroupTestAnswer> GroupsTestsQuestionsAnswers { get; set; }

        public int getMillisecondsToSleep()
        {
            return 0;
        }

        public override int SaveChanges()
        {
            return 0;
        }

        #region user
        public void addUser(User u)
        {
            if (_users.Where(user => user.UserId.Equals(u.UserId)).Count() == 0)
            {
                _users.Add(u);
            }
        }

        public User getUser(string UserId)
        {
            List<User> matches = _users.Where(u => u.UserId.Equals(UserId)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public User getUser(int uniqueInt)
        {
            List<User> matches = _users.Where(u => u.uniqueInt == uniqueInt).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }
        #endregion
        #region admin
        public void addAdmin(Admin a)
        {
            if (getUser(a.AdminId) != null && _admins.Where(admin => admin.AdminId.Equals(a.AdminId)).Count() == 0)
            {
                _admins.Add(a);
            }
        }

        public Admin getAdmin(string UserId)
        {
            List<Admin> matches = _admins.Where(a => a.AdminId.Equals(UserId)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }
        #endregion
        #region subject
        public void addSubject(Subject s)
        {
            if (_subjects.Where(subject => subject.SubjectId.Equals(s.SubjectId)).Count() == 0)
            {
                _subjects.Add(s);
            }
        }

        public Subject getSubject(string subject)
        {
            List<Subject> matches = _subjects.Where(s => s.SubjectId.Equals(subject)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public List<Subject> getSubjects()
        {
            return _subjects;
        }
        #endregion
        #region topic
        public void addTopic(Topic t)
        {
            if (getSubject(t.SubjectId) != null && _topics.Where(topic => topic.TopicId.Equals(t.TopicId) && topic.SubjectId.Equals(t.SubjectId)).Count() == 0)
            {
                _topics.Add(t);
            }
        }

        public Topic getTopic(string subject, string topic)
        {
            List<Topic> matches = _topics.Where(t => t.SubjectId.Equals(subject) && t.TopicId.Equals(topic)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public List<Topic> getTopics(string subject)
        {
            return _topics.Where(t => t.SubjectId.Equals(subject)).ToList();
        }
        #endregion
        #region question
        public void addQuestion(Question q)
        {
            if (_questions.Where(question => question.QuestionId == q.QuestionId).Count() == 0)
            {
                _questions.Add(q);
            }
        }

        public Question getQuestion(int id)
        {
            List<Question> matches = _questions.Where(q => q.QuestionId == id).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public void updateQuestion(Question q)
        {
            // nothing for now, probabely would find the relevant question and copy the fields
        }

        public List<Question> getQuestions(string subject, string topic)
        {
            List<Question> matches = _questions.Where(q => q.SubjectId.Equals(subject)).ToList();
            List<Question> ans = new List<Question>();
            foreach (Question q in matches)
            {
                if (getQuestionDiagnoses(q.QuestionId).Where(d => d.TopicId.Equals(topic)).Count() == 1)
                {
                    ans.Add(q);
                }
            }
            return ans;
        }

        public List<Question> getNormalQuestions(string subject)
        {
            return getQuestions(subject, Topics.NORMAL);
        }
        #endregion
        #region image
        public void addImage(Image i)
        {
            if (_questions.Where(q => q.QuestionId == i.QuestionId).Count() == 0 ||
                _images.Where(image => image.ImageId.Equals(i.ImageId) && image.QuestionId == i.QuestionId).Count() != 0)
            {
                return;
            }
            _images.Add(i);
        }

        public List<Image> getQuestionImages(int qId)
        {
            return _images.Where(i => i.QuestionId == qId).ToList();
        }
        #endregion
        #region diagnosis
        public void addDiagnosis(Diagnosis d)
        {
            if (_diagnoses.Where(diagnosis => diagnosis.TopicId.Equals(d.TopicId) && diagnosis.QuestionId == d.QuestionId && diagnosis.SubjectId.Equals(d.SubjectId)).Count() == 0)
            {
                _diagnoses.Add(d);
            }
        }

        public List<Diagnosis> getQuestionDiagnoses(int qId)
        {
            return _diagnoses.Where(d => d.QuestionId == qId).ToList();
        }
        #endregion
        #region answer
        public void addAnswer(Answer a)
        {
            if (_answers.Where(answer => answer.AnswerId == a.AnswerId).Count() == 0)
            {
                _answers.Add(a);
            }
        }
        #endregion
        #region diagnosis certainty
        public void addDiagnosisCertainty(DiagnosisCertainty dc)
        {
            if (_diagnosesCertainties.Where(diagnosisCertainty => diagnosisCertainty.AnswerId == dc.AnswerId && diagnosisCertainty.SubjectId.Equals(dc.SubjectId) && diagnosisCertainty.TopicId.Equals(dc.TopicId)).Count() == 0)
            {
                _diagnosesCertainties.Add(dc);
            }
        }
        #endregion
        #region user level answer
        public void addUserLevelAnwer(UserLevelAnswer ula)
        {
            if (_usersLevelsWhenAnswering.Where(userLevelAnswer => userLevelAnswer.AnswerId == ula.AnswerId && userLevelAnswer.SubjectId.Equals(ula.SubjectId) && userLevelAnswer.TopicId.Equals(ula.TopicId)).Count() == 0)
            {
                _usersLevelsWhenAnswering.Add(ula);
            }
        }
        #endregion
        #region user level
        public void addUserLevel(UserLevel ul)
        {
            if (_usersLevels.Where(userLevel => userLevel.UserId.Equals(ul.UserId) && userLevel.SubjectId.Equals(ul.SubjectId) && userLevel.TopicId.Equals(ul.TopicId)).Count() == 0)
            {
                _usersLevels.Add(ul);
            }
        }

        public UserLevel getUserLevel(string UserId, string subject, string topic)
        {
            List<UserLevel> matches = _usersLevels.Where(ul => ul.UserId.Equals(UserId) && ul.SubjectId.Equals(subject) && ul.TopicId.Equals(topic)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public void updateUserLevel(UserLevel ul)
        {
            // nothing for now, probabely the same as update question
        }
        #endregion
        #region group
        public void addGroup(Group g)
        {
            if (_groups.Where(group => group.AdminId.Equals(g.AdminId) && group.name.Equals(g.name)).Count() == 0)
            {
                _groups.Add(g);
            }
        }

        public Group getGroup(string adminId, string groupName)
        {
            List<Group> matches = _groups.Where(g => g.AdminId.Equals(adminId) && g.name.Equals(groupName)).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        public void removeGroup(Group g)
        {
            _groups.Remove(g);
        }

        public List<Group> getAdminsGroups(string adminId)
        {
            return _groups.Where(g => g.AdminId.Equals(adminId)).ToList();
        }
        #endregion
        #region group member
        public void addGroupMember(GroupMember gm)
        {
            if (_groups.Where(g => g.AdminId.Equals(gm.AdminId) && g.name.Equals(gm.GroupName)).Count() == 0 ||
                _groupsMembers.Where(groupMember => groupMember.AdminId.Equals(gm.AdminId) && groupMember.GroupName.Equals(gm.GroupName) && groupMember.UserId.Equals(gm.UserId)).Count() != 0)
            {
                return;
            }
            _groupsMembers.Add(gm);
            SaveChanges();
        }

        public void removeGroupMembers(Group g)
        {
            _groupsMembers.RemoveAll(gm => gm.GroupName.Equals(g.name) && gm.AdminId.Equals(g.AdminId));
        }
        #endregion
        #region test
        public List<Test> getAllTests()
        {
            return new List<Test>(_tests);
        }
        #endregion
        #region group test
        public void addGroupTest(GroupTest gt)
        {
            if (_admins.Where(a => a.AdminId.Equals(gt.adminId)).Count() == 0 || 
                _groups.Where(g => g.AdminId.Equals(gt.adminId) && g.name.Equals(gt.GroupName)).Count() == 0 ||
                _tests.Where(t => t.TestId == gt.TestId).Count() == 0 ||
                _groupsTests.Where(gtest => gtest.adminId.Equals(gt.adminId) && gtest.GroupName.Equals(gt.GroupName) && gtest.TestId == gt.TestId).Count() != 0)
            {
                return;
            }
            _groupsTests.Add(gt);
        }
        #endregion
    }
}
