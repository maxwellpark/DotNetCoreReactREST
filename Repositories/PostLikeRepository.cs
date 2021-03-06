﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreReactREST.DbContexts;
using DotNetCoreReactREST.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DotNetCoreReactREST.Repositories
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly AppDbContext _context;

        public PostLikeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PostLike>> GetLikesForPost(int postId)
        {
            if (string.IsNullOrWhiteSpace(postId.ToString()))
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await _context.PostLikes.Where(pl => pl.PostId == postId).OrderByDescending(pl => pl.Id).ToListAsync();
        }

        public async Task<PostLike> GetPostLikeById(int postLikeId)
        {
            if (string.IsNullOrWhiteSpace(postLikeId.ToString()))
            {
                throw new ArgumentNullException(nameof(postLikeId));
            }

            return await _context.PostLikes.FirstOrDefaultAsync(l => l.Id == postLikeId);
        }

        public async Task<PostLike> LikePostAsync(PostLike postLike)
        {
            if (postLike == null)
            {
                throw new ArgumentNullException(nameof(postLike));
            }

            await _context.PostLikes.AddAsync(postLike);
            bool isSaved = await SaveAsync();
            if (!isSaved)
            {
                return null;
            }

            return await GetPostLikeById(postLike.PostId);
        }

        public async Task<PostLike> PostLikeExists(int postId, string userId)
        {
            Log.Information("PostId: {@PostId}, UserId: {@UserId}", postId, userId);
            PostLike postLike = await _context.PostLikes
                .FirstOrDefaultAsync(l =>
                l.ApplicationUserId == userId
                && l.PostId == postId);
            Log.Information("PostLikeExists: {@PostLikeExists}", postLike);
            if (postLike == null)
            {
                return null;
            }

            return postLike;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}