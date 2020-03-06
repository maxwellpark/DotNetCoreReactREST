﻿using DotNetCoreReactREST.DbContexts;
using DotNetCoreReactREST.Entities;
using DotNetCoreReactREST.ResourceParameters;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreReactREST.Repositories
{
    public class UserRepository : IUserRepository
    {
        private AppDbContext _context;

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void AddUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            ///for testing
            _context.Users.Add(user);
        }

        public void DeleteUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Remove(user);
        }

        public ApplicationUser GetUserById(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }
        public IEnumerable<ApplicationUser> GetUsers()
        {
            return _context.Users.OrderBy(u => u.IsAdmin == true)
                .OrderBy(u => u.UserName).ToList();
        }

        public IEnumerable<ApplicationUser> GetUsers(UsersResourceParameters usersResourceParameters)
        {
            if (usersResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(usersResourceParameters));
            }

            if (string.IsNullOrWhiteSpace(usersResourceParameters.SearchQuery))
            {
                return GetUsers();
            }

            var collection = _context.Authors as IQueryable<Author>;

            if (!string.IsNullOrWhiteSpace(usersResourceParameters.MainCategory))
            {
                var mainCategory = usersResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }

            if (!string.IsNullOrWhiteSpace(usersResourceParameters.SearchQuery))
            {

                var searchQuery = usersResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                    || a.FirstName.Contains(searchQuery)
                    || a.LastName.Contains(searchQuery));
            }

            return collection.ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateUser(ApplicationUser user)
        {
            //no code needed here using ef core. 
            //Call to this method is just in line with 
            //loose coupling and persistance agnostic
        }

        public bool UserExists(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _context.Users.Any(u => u.Id == userId);
        }
    }
}
