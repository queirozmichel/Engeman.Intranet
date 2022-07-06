using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<PostDto> GetAllPosts();
    public Post GetPostById(int id);
    public void AddQuestion(AskQuestionDto askQuestionDto);
    public void UpdateQuestion(int id, AskQuestionDto askQuestionDto);
    public void UpdateArchivePost(int id, AskQuestionDto askQuestionDto, Archive archive);
    public void AddArchive(AskQuestionDto askQuestionDto, Archive archive);
    public void DeletePost(int postId);
  }
}