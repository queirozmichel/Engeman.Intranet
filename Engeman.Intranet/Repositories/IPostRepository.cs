using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<PostDto> GetPostsByRestriction(int userDepartmentId, int userIdSession);
    public Post GetPostById(int id);
    public List<int> GetRestrictedDepartmentsIdByPost(int id);
    public void AddQuestion(AskQuestionDto askQuestionDto);
    public void UpdateQuestion(int id, AskQuestionDto askQuestionDto);
    public void UpdateArchivePost(int id, AskQuestionDto postInformation, List<Archive> archives);
    public void AddArchive(AskQuestionDto askQuestionDto, List<Archive> archives);
    public void AddArchive(int id, List<Archive> archives);
    public void DeletePost(int postId);
  }
}