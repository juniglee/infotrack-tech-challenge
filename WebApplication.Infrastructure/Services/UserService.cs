﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryContext _dbContext;

        public UserService(InMemoryContext dbContext)
        {
            _dbContext = dbContext;

            // this is a hack to seed data into the in memory database. Do not use this in production.
            _dbContext.Database.EnsureCreated();
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users.Where(user => user.Id == id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException("Implement a way to find users that match the provided given names OR last name.");
            return await _dbContext.Users.Where(user => user.GivenNames == givenNames || user.LastName == lastName)
                                         .Include(x => x.ContactDetail)
                                         .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement a way to get a 'page' of users.");
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException("Implement a way to add a new user, including their contact details.");
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return await _dbContext.Users.Where(u => u.Id == user.Id)
                                         .FirstAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException("Implement a way to update an existing user, including their contact details.");
            User? existingUser = await _dbContext.Users.Where(u => u.Id == user.Id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                //_dbContext.Users.Remove(user);
                _dbContext.Entry(existingUser).CurrentValues.SetValues(user);
                _dbContext.SaveChanges();
            }
            else
            {
                return await AddAsync(user, cancellationToken);
            }

            return existingUser;
        }

        /// <inheritdoc />
        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException("Implement a way to delete an existing user, including their contact details.");
            User? user = await _dbContext.Users.Where(user => user.Id == id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }

            return user;
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement a way to count the number of users in the database.");
        }
    }
}
