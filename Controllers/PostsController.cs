﻿using AutoMapper;
using DotNetCoreReactREST.Dtos;
using DotNetCoreReactREST.Entities;
using DotNetCoreReactREST.Repositories;
using DotNetCoreReactREST.ResourceParameters;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreReactREST
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostsController(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }
        //POST Api/Posts
        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody]PostDto post)
        {
            Post newPost = await _postRepository.CreatePostAsync(_mapper.Map<Post>(post));
            var baseURI = Request.GetDisplayUrl();
            // Alternative way
            // var baseURI = Request.Scheme + "://" + Request.Host + Request.Path;
            return Created(baseURI + newPost.Id, _mapper.Map<PostDto>(newPost));
        }
        //GET Api/posts[category = string &| searchQuery = string]
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetPostsAsync([FromQuery]PostResourceParameter postResourceParameter = null)
        {
            if (postResourceParameter == null)
            {
                // Code never went here
                IEnumerable<Post> posts = await _postRepository.GetPostsAsync();
                if (posts == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<PostDto>>(posts));
            }
            var query = await _postRepository.GetPostsAsync(postResourceParameter);
            if (query == null) 
            {
                return NotFound();
            }
            return Ok(query);

            //IEnumerable<Post> postFromRepository = query.posts;
            //if (postFromRepository == null)
            //{
            //    return NotFound();
            //}
            //return Ok(_mapper.Map<IEnumerable<PostDto>>(postFromRepository));
        }
        //GET Api/Posts/{postId}
        [HttpGet]
        // Route will only match if postId can be casted as a int
        [Route("{postId:int}")]
        public async Task<IActionResult> GetPostByIdAsync(int postId)
        {
            Post postFromRepository = await _postRepository.GetPostByIdAsync(postId);
            if (postFromRepository == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PostDto>(postFromRepository));
        }
        //PATCH Api/Posts/{postId}
        [HttpPatch("{postId:int}", Name = "{postId:int}")]
        public async Task<IActionResult> UpdatePost([FromRoute]int postId, [FromBody]JsonPatchDocument<Post> patchDocument)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Post to update
            Post oldPost = await _postRepository.GetPostByIdAsync(postId);
            if (oldPost == null)
            {
                return NotFound();
            }
            else
            {
                patchDocument.ApplyTo(oldPost, ModelState);

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                // Update time
                oldPost.DateTime = DateTime.Now;

                // Save
                await _postRepository.Save();

                return Ok(oldPost);
            };
        }
        //DELETE Api/Posts/{PostId}
        [HttpDelete("{postId:int}")]
        public async Task<IActionResult> DeletePost([FromRoute]int postId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound("There is nothing to delete.");
            }

            bool result = await _postRepository.DeletePostAsync(postId);
            if (result)
            {
                return Ok("Post Deleted.");
            }
            return Ok("Post not deleted.");
        }
    }
}