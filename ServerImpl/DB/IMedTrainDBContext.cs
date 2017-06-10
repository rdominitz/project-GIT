using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public interface IMedTrainDBContext
    {
        int getMillisecondsToSleep();

        int SaveChanges();

        void addUser(User u);
        User getUser(string UserId);
        User getUser(int uniqueInt);

        void addAdmin(Admin a);
        Admin getAdmin(string UserId);

        void addSubject(Subject s);
        Subject getSubject(string subject);
        List<Subject> getSubjects();

        void addTopic(Topic t);
        Topic getTopic(string subject, string topic);
        List<Topic> getTopics(string subject);

        void addQuestion(Question q);
        Question getQuestion(int id);
        void updateQuestion(Question q);
        List<Question> getQuestions(string subject, string topic);
        List<Question> getNormalQuestions(string subject);

        void addImage(QuestionImage i);
        List<QuestionImage> getQuestionImages(int qId);

        void addDiagnosis(Diagnosis d);
        List<Diagnosis> getQuestionDiagnoses(int qId);

        void addAnswer(Answer a);
        Answer getAnswer(int answerId);

        void addDiagnosisCertainty(DiagnosisCertainty dc);

        void addUserLevelAnwer(UserLevelAnswer ula);

        void addUserLevel(UserLevel ul);
        UserLevel getUserLevel(string UserId, string subject, string topic);
        void updateUserLevel(UserLevel ul);

        void addGroup(Group g);
        Group getGroup(string adminId, string groupName);
        void removeGroup(Group g);
        List<Group> getAdminsGroups(string adminId);

        void addGroupMember(GroupMember gm);
        void removeGroupMembers(Group g);
        List<GroupMember> getUserGroups(string userId);
        List<GroupMember> getUserInvitations(string userId);
        bool hasInvitation(string userId, string groupName, string adminId);
        GroupMember getGroupMemberInvitation(string userId, string groupName, string adminId);
        void updateGroupMember(GroupMember gm);
        List<GroupMember> getGroupMembers(string groupName, string adminId);
        void removeGroupMember(GroupMember gm);

        void addTest(Test t);
        List<Test> getAllTests(string subject);
        Test getTest(int testId);

        void addGroupTest(GroupTest gt);
        List<GroupTest> getGroupTests(string groupName, string adminId);

        void addTestQuestion(TestQuestion tq);
        List<TestQuestion> getTestQuestions(int testId);

        void addGroupTestAnswer(GroupTestAnswer gta);
        void removeGroupTestAnswers(string groupName, string adminId);
        List<GroupTestAnswer> getGroupTestAnswers(string groupName, string adminId, int testId);
    }
}
