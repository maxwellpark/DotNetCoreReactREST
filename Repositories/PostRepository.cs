﻿using DotNetCoreReactREST.DbContexts;
using DotNetCoreReactREST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreReactREST.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _appDbContext;
        public PostRepository(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public IEnumerable<Post> GetPosts
        {
          get
            {
                IEnumerable<Post> Posts = _appDbContext.Posts;
                return Posts;
            }
        }

        public void CreatePost(Post post)
        {
            post.DateTime = DateTime.Now;
            _appDbContext.Posts.Add(post);
            Save();
        }

        public void DeletePost(int postId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Post[]> GetPostByCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Post GetPostById(int postId)
        {
            Post post = _appDbContext.Posts.Where(p => p.Id == postId).FirstOrDefault();
            return post;
        }

        public Task<Post> GetPostByIdAndCategory(int postId, string category)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _appDbContext.SaveChangesAsync();
        }

        public Task<Post> UpdatePost(Post post)
        {
            throw new NotImplementedException();
        }
    }
}