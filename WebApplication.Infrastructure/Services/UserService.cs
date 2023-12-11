using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryContext _dbContext;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public UserService(InMemoryContext dbContext)
        {
            _dbContext = dbContext;

            // this is a hack to seed data into the in memory database. Do not use this in production.
            _dbContext.Database.EnsureCreated();
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Users.Where(user => user.Id == id)
                                             .Include(x => x.ContactDetail)
                                             .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Users.Where(user => user.GivenNames == givenNames || user.LastName == lastName)
                                             .Include(x => x.ContactDetail)
                                             .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            try
            { 
                return await _dbContext.Users.OrderBy(u => u.Id)
                                         .Skip((page - 1) * count)
                                         .Take(count)
                                         .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                return await _dbContext.Users.Where(u => u.Id == user.Id)
                                             .FirstAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <inheritdoc />
        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                User? existingUser = await _dbContext.Users.Where(u => u.Id == user.Id)
                                                           .Include(x => x.ContactDetail)
                                                           .FirstOrDefaultAsync(cancellationToken);

                if (existingUser != null)
                {
                    _dbContext.Entry(existingUser).CurrentValues.SetValues(user);
                    _dbContext.SaveChanges();
                    return existingUser;
                }
                else
                {
                    return await AddAsync(user, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <inheritdoc />
        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                User? user = await _dbContext.Users.Where(user => user.Id == id)
                                                   .Include(x => x.ContactDetail)
                                                   .FirstOrDefaultAsync(cancellationToken);

                if (!(user is default(User)))
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                }

                return user;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Users.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
    }
}
