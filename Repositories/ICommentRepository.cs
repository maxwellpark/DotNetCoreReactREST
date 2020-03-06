﻿using DotNetCoreReactREST.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreReactREST.Repositories
{
    public interface ICommentRepository
    {
        public IEnumerable<Comment> GetComment();
        public Comment GetCommentById(int commentId);
        public void AddComment(Comment comment);
        public void UpdateComment(Comment comment);
        public void DeleteComment(Comment comment);
        public bool CommentExists(int commentId);
        public bool Save();
    }
}
