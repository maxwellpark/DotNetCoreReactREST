﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreReactREST.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        public string ImageUrl { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(1000)]
        public string Content { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostLike> PostLike { get; set; } = new List<PostLike>();

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}