using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<PostDto> GetAllPosts();
    public void InsertQuestion(AskQuestionDto askQuestionDto);
  }
}