﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCoreReactREST.Dtos;
using DotNetCoreReactREST.Entities;
using DotNetCoreReactREST.Repositories;
using DotNetCoreReactREST.ResourceParameters;
using DotNetCoreReactREST.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DotNetCoreReactREST
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;

        public PostsController(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        // POST: Api/Posts
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody]PostDto post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            // Replace with Imgur URL of the image
            post.ImageUrl = await new ImgurService().Upload(post.ImageUrl);

            var postToAdd = _mapper.Map<Post>(post);
            await _postRepository.AddPostAsync(postToAdd);

            var isSaved = await _postRepository.SaveAsync();
            if (!isSaved)
            {
                return Problem("Problem saving newly created post.");
            }

            Log.Information("Finding Post DateTime: {@DateTime}", postToAdd.DateTime.ToString());

            // Alternative way
            // var baseURI = Request.Scheme + "://" + Request.Host + Request.Path;
            var baseURI = Request.GetDisplayUrl();

            return Created(baseURI + postToAdd.Id, _mapper.Map<PostDto>(postToAdd));
        }

        // DELETE: Api/Posts/{PostId}
        [HttpDelete("{postId:int}")]
        public async Task<IActionResult> DeletePost([FromRoute]int postId)
        {
            var postToDelete = await _postRepository.GetPostByIdAsync(postId);
            if (postToDelete == null)
            {
                return NotFound("There is nothing to delete.");
            }

            _postRepository.DeletePost(postToDelete);

            var isSaved = await _postRepository.SaveAsync();
            if (isSaved)
            {
                return Ok("Post Deleted.");
            }

            return Ok("Post not deleted.");
        }

        // PATCH: Api/Posts/{PostId}
        [HttpPatch("{postId:int}", Name = "{postId:int}")]
        public async Task<IActionResult> EditPost([FromRoute]int postId, [FromBody]JsonPatchDocument<Post> patchDocument)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Post to update
            Post oldPost = await _postRepository.GetPostByIdAsync(postId);

            // Previous/old Image Url
            string prePatchImageUrl = oldPost.ImageUrl;

            if (oldPost == null)
            {
                return NotFound();
            }
            else
            {
                // Apply new data
                patchDocument.ApplyTo(oldPost, ModelState);

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                // Update time
                oldPost.DateTime = DateTime.Now;

                // Updated/new Image Url
                string postPatchImageUrl = oldPost.ImageUrl;

                // Replace with Imgur URL of the updated image
                if (prePatchImageUrl != postPatchImageUrl)
                {
                    oldPost.ImageUrl = await new ImgurService().Upload(oldPost.ImageUrl);
                }

                // Save
                await _postRepository.SaveAsync();

                return Ok(oldPost);
            }
        }

        // GET: Api/Posts/{PostId}
        // Route will only match if postId can be casted as a int
        [HttpGet]
        [Route("{postId:int}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            Post postFromRepo = await _postRepository.GetPostByIdAsync(postId);
            Log.Information("Post from Repository when getting post by id: {@0}", postFromRepo);
            if (postFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostDto>(postFromRepo));
        }

        // GET: Api/Posts[category = string &| searchQuery = string]
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetPosts([FromQuery]PaginationResourceParameter<Post> paginationResourceParameter)
        {
            var result = await _postRepository.GetPostsAsync(paginationResourceParameter);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}