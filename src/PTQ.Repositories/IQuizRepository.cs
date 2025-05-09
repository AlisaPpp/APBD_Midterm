using PTQ.Models;
namespace PTQ.Repositories;

public interface IQuizRepository
{
    List<Quiz> GetAllQuizzes();
    public SpecificQuizDTO GetSpecificQuiz(int quizId);
    public bool AddQuiz(AddANewQuizDTO quiz);
}