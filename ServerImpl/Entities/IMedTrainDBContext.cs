using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
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

        void addImage(Image i);
        List<Image> getQuestionImages(int qId);

        void addDiagnosis(Diagnosis d);
        List<Diagnosis> getQuestionDiagnoses(int qId);

        void addAnswer(Answer a);

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
    }
}
