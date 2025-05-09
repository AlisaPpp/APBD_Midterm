using PTQ.Models;
namespace PTQ.Application;

public interface IQuizService
{
    public List<GetAllQuizzesDTO> GetAllQuizzes();
    public SpecificQuizDTO GetSpecificQuiz(int id);
    public bool AddQuiz(AddANewQuizDTO quiz);
}