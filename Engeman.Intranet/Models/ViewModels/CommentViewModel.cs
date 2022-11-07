using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
    public class CommentViewModel
    {
        public CommentViewModel()
        {
            Files = new List<CommentFile>();
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public string UserDepartmentName { get; set; }
        public byte[] UserPhoto { get; set; }
        public bool Revised { get; set; }
        public List<CommentFile> Files { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
