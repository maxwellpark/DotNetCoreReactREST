﻿using System;

namespace DotNetCoreReactREST.Models
{
    public class PostDto
    {
        // Public Facing Properties
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
