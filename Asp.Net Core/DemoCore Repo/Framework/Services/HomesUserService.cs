using Common.Abstractions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Framework.Services
{
    public class HomesUserService : IHomesUserService
    {
        private bool _usingPervasive = true;
        private IHomesUserRepository _homesUserRepository;
        private IHomesUserSqlRepository _homesUserSqlRepository;
        public HomesUserService(IHomesUserRepository homesUserRepository, IHomesUserSqlRepository homesUserSqlRepository)
        {
            _homesUserRepository = homesUserRepository;
            _homesUserSqlRepository = homesUserSqlRepository;
            _usingPervasive = homesUserRepository.IsPervasiveTrue();
        }
        public Task<HomesUser> AddAsync(HomesUser homesUser)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<HomesUser>> AddUsers(ICollection<HomesUser> homesUsers)
        {
            foreach (HomesUser user in homesUsers)
            {
                string originalPassword = user.Password;
                string encryptedpassword = string.Empty;
                int spins = 0;

                user.Password = encryptedpassword;
                user.Spins = spins;
            }

            try
            {
                if (_usingPervasive)
                {
                    foreach (HomesUser user in homesUsers)
                    {
                        await _homesUserRepository.AddAsync(user);
                    }
                }

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (HomesUser user in homesUsers)
                    {
                        await _homesUserSqlRepository.AddAsync(user);
                    }

                    transaction.Complete();
                }

                return homesUsers;
            }
            catch (Exception ex)
            {
                if (_usingPervasive)
                {
                    foreach (var user in homesUsers)
                    {
                        if (user.UserId > 0)
                            await _homesUserRepository.DeleteFromUser(user.UserId);
                        else
                            break;
                    }
                }
                throw ex;
            }
        }

        public Task<bool> DeleteAsync(HomesUser hmsTransaction)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<HomesUser>> ReadAllAsync()
        {
            return await _homesUserSqlRepository.GetAllAsync();
        }

        public Task<ICollection<HomesUser>> ReadAsync(HomesUser hmsDomainEntity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HomesUser>> ReadByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<HomesUser>> ReadByIdAsync(long Id)
        {
            return await _homesUserSqlRepository.GetByIdAsync(Id);
        }

        public Task<ICollection<HomesUser>> ReadFilteredListAsync(Func<HomesUser, bool> condition)
        {
            throw new NotImplementedException();
        }

        public async Task<HomesUser> UpdateAsync(HomesUser homesUser)
        {
            if (!string.IsNullOrEmpty(homesUser.Password))
            {
                string originalPassword = homesUser.Password;
                string encryptedpassword = string.Empty;
                int spins = 0;

                homesUser.Password = encryptedpassword;
                homesUser.Spins = spins;
            }

            return await _homesUserRepository.UpdateAsync(homesUser);
        }

    }
}
