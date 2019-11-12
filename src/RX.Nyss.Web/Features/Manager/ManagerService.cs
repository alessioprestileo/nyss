﻿using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Features.Manager.Dto;
using RX.Nyss.Web.Services;
using RX.Nyss.Web.Utils.DataContract;
using RX.Nyss.Web.Utils.Logging;

namespace RX.Nyss.Web.Features.Manager
{
    public class ManagerService : IManagerService
    {
        private readonly ILoggerAdapter _loggerAdapter;
        private readonly INyssContext _dataContext;
        private readonly IIdentityUserRegistrationService _identityUserRegistrationService;
        private readonly INationalSocietyUserService _nationalSocietyUserService;
        private readonly IVerificationEmailService _verificationEmailService;

        public ManagerService(IIdentityUserRegistrationService identityUserRegistrationService, INationalSocietyUserService nationalSocietyUserService, INyssContext dataContext, ILoggerAdapter loggerAdapter, IVerificationEmailService verificationEmailService)
        {
            _identityUserRegistrationService = identityUserRegistrationService;
            _nationalSocietyUserService = nationalSocietyUserService;
            _dataContext = dataContext;
            _loggerAdapter = loggerAdapter;
            _verificationEmailService = verificationEmailService;
        }

        public async Task<Result> CreateManager(int nationalSocietyId, CreateManagerRequestDto createManagerRequestDto)
        {
            try
            {
                string securityStamp;
                ManagerUser user;
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var identityUser = await _identityUserRegistrationService.CreateIdentityUser(createManagerRequestDto.Email, Role.Manager);
                    securityStamp = await _identityUserRegistrationService.GenerateEmailVerification(identityUser.Email);

                    user = await CreateManagerUser(identityUser, nationalSocietyId, createManagerRequestDto);
                    
                    transactionScope.Complete();
                }
                await _verificationEmailService.SendVerificationEmail(user, securityStamp);
                return Result.Success(ResultKey.User.Registration.Success);
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }
        }

        private async Task<ManagerUser> CreateManagerUser(IdentityUser identityUser, int nationalSocietyId, CreateManagerRequestDto createManagerRequestDto)
        {
            var nationalSociety = await _dataContext.NationalSocieties.Include(ns => ns.ContentLanguage)
                .SingleOrDefaultAsync(ns => ns.Id == nationalSocietyId);

            if (nationalSociety == null)
            {
                throw new ResultException(ResultKey.User.Registration.NationalSocietyDoesNotExist);
            }

            var defaultUserApplicationLanguage = await _dataContext.ApplicationLanguages
                .SingleOrDefaultAsync(al => al.LanguageCode == nationalSociety.ContentLanguage.LanguageCode);

            var user = new ManagerUser
            {
                IdentityUserId = identityUser.Id,
                EmailAddress = identityUser.Email,
                Name = createManagerRequestDto.Name,
                PhoneNumber = createManagerRequestDto.PhoneNumber,
                AdditionalPhoneNumber = createManagerRequestDto.AdditionalPhoneNumber,
                Organization = createManagerRequestDto.Organization,
                ApplicationLanguage = defaultUserApplicationLanguage,
            };

            var userNationalSociety = CreateUserNationalSocietyReference(nationalSociety, user);

            await _dataContext.AddAsync(userNationalSociety);
            await _dataContext.SaveChangesAsync();
            return user;
        }

        private UserNationalSociety CreateUserNationalSocietyReference(Nyss.Data.Models.NationalSociety nationalSociety, Nyss.Data.Models.User user) =>
            new UserNationalSociety
            {
                NationalSociety = nationalSociety,
                User = user
            };

        public async Task<Result<GetManagerResponseDto>> GetManager(int nationalSocietyUserId)
        {
            var manager = await _dataContext.Users
                .OfType<ManagerUser>()
                .Where(u => u.Id == nationalSocietyUserId)
                .Select(u => new GetManagerResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    Email = u.EmailAddress,
                    PhoneNumber = u.PhoneNumber,
                    AdditionalPhoneNumber = u.AdditionalPhoneNumber,
                    Organization = u.Organization,
                })
                .SingleOrDefaultAsync();

            if (manager == null)
            {
                _loggerAdapter.Debug($"Data manager with id {nationalSocietyUserId} was not found");
                return Result.Error<GetManagerResponseDto>(ResultKey.User.Common.UserNotFound);
            }

            return new Result<GetManagerResponseDto>(manager, true);
        }

        public async Task<Result> UpdateManager(int managerId, EditManagerRequestDto editManagerRequestDto)
        {
            try
            {
                var user = await _nationalSocietyUserService.GetNationalSocietyUser<ManagerUser>(managerId);

                user.Name = editManagerRequestDto.Name;
                user.PhoneNumber = editManagerRequestDto.PhoneNumber;
                user.Organization = editManagerRequestDto.Organization;

                await _dataContext.SaveChangesAsync();
                return Result.Success();
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }
        }

        public Task<Result> DeleteManager(int managerId) =>
            _nationalSocietyUserService.DeleteUser<ManagerUser>(managerId);
    }
}