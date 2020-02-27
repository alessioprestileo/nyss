using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common.Utils;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Common.Utils.Logging;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Data.Queries;
using RX.Nyss.Web.Features.Supervisors.Dto;
using RX.Nyss.Web.Features.Supervisors.Models;
using RX.Nyss.Web.Services;
using static RX.Nyss.Common.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.Supervisors
{
    public interface ISupervisorService
    {
        Task<Result> Create(int nationalSocietyId, CreateSupervisorRequestDto createSupervisorRequestDto);
        Task<Result<GetSupervisorResponseDto>> Get(int supervisorId);
        Task<Result> Edit(int supervisorId, EditSupervisorRequestDto editSupervisorRequestDto);
        Task<Result> Delete(int supervisorId);
    }

    public class SupervisorService : ISupervisorService
    {
        private readonly ILoggerAdapter _loggerAdapter;
        private readonly INyssContext _dataContext;
        private readonly IIdentityUserRegistrationService _identityUserRegistrationService;
        private readonly INationalSocietyUserService _nationalSocietyUserService;
        private readonly IVerificationEmailService _verificationEmailService;
        private readonly IDeleteUserService _deleteUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SupervisorService(IIdentityUserRegistrationService identityUserRegistrationService, INationalSocietyUserService nationalSocietyUserService, INyssContext dataContext,
            ILoggerAdapter loggerAdapter, IVerificationEmailService verificationEmailService, IDeleteUserService deleteUserService, IDateTimeProvider dateTimeProvider)
        {
            _identityUserRegistrationService = identityUserRegistrationService;
            _nationalSocietyUserService = nationalSocietyUserService;
            _dataContext = dataContext;
            _loggerAdapter = loggerAdapter;
            _verificationEmailService = verificationEmailService;
            _deleteUserService = deleteUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> Create(int nationalSocietyId, CreateSupervisorRequestDto createSupervisorRequestDto)
        {
            try
            {
                string securityStamp;
                SupervisorUser user;
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var identityUser = await _identityUserRegistrationService.CreateIdentityUser(createSupervisorRequestDto.Email, Role.Supervisor);
                    securityStamp = await _identityUserRegistrationService.GenerateEmailVerification(identityUser.Email);

                    user = await CreateSupervisorUser(identityUser, nationalSocietyId, createSupervisorRequestDto);

                    transactionScope.Complete();
                }

                await _verificationEmailService.SendVerificationEmail(user, securityStamp);
                return Success(ResultKey.User.Registration.Success);
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }
        }

        public async Task<Result<GetSupervisorResponseDto>> Get(int supervisorId)
        {
            var supervisor = await _dataContext.Users.FilterAvailable()
                .OfType<SupervisorUser>()
                .Where(u => u.Id == supervisorId)
                .Select(u => new GetSupervisorResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    Email = u.EmailAddress,
                    PhoneNumber = u.PhoneNumber,
                    AdditionalPhoneNumber = u.AdditionalPhoneNumber,
                    Sex = u.Sex,
                    DecadeOfBirth = u.DecadeOfBirth,
                    ProjectId = u.CurrentProject.Id,
                    Organization = u.Organization,
                    NationalSocietyId = u.UserNationalSocieties.Select(uns => uns.NationalSocietyId).Single(),
                    CurrentProject = new EditSupervisorFormDataDto.ListProjectsResponseDto
                    {
                        Id = u.CurrentProject.Id,
                        Name = u.CurrentProject.Name,
                        IsClosed = u.CurrentProject.State == ProjectState.Closed
                    },
                    EditSupervisorFormData = new EditSupervisorFormDataDto
                    {
                        AvailableProjects = _dataContext.Projects
                            .Where(p => p.NationalSociety.Id == u.UserNationalSocieties.Select(uns => uns.NationalSocietyId).Single())
                            .Where(p => p.State == ProjectState.Open)
                            .Select(p => new EditSupervisorFormDataDto.ListProjectsResponseDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                IsClosed = p.State == ProjectState.Closed
                            })
                            .ToList()
                    }
                })
                .SingleOrDefaultAsync();

            if (supervisor == null)
            {
                _loggerAdapter.Debug($"Data manager with id {supervisorId} was not found");
                return Error<GetSupervisorResponseDto>(ResultKey.User.Common.UserNotFound);
            }

            if (supervisor.CurrentProject != null && supervisor.CurrentProject.IsClosed)
            {
                supervisor.EditSupervisorFormData.AvailableProjects.Add(new EditSupervisorFormDataDto.ListProjectsResponseDto
                {
                    Id = supervisor.CurrentProject.Id,
                    Name = supervisor.CurrentProject.Name,
                    IsClosed = supervisor.CurrentProject.IsClosed
                });
            }

            return new Result<GetSupervisorResponseDto>(supervisor, true);
        }

        public async Task<Result> Edit(int supervisorId, EditSupervisorRequestDto editSupervisorRequestDto)
        {
            try
            {
                var supervisorUserData = await GetSupervisorUser(supervisorId);

                if (supervisorUserData == null)
                {
                    _loggerAdapter.Debug($"A supervisor with id {supervisorId} was not found");
                    return Error(ResultKey.User.Common.UserNotFound);
                }

                var supervisorUser = supervisorUserData.User;

                supervisorUser.Name = editSupervisorRequestDto.Name;
                supervisorUser.Sex = editSupervisorRequestDto.Sex;
                supervisorUser.DecadeOfBirth = editSupervisorRequestDto.DecadeOfBirth;
                supervisorUser.PhoneNumber = editSupervisorRequestDto.PhoneNumber;
                supervisorUser.AdditionalPhoneNumber = editSupervisorRequestDto.AdditionalPhoneNumber;
                supervisorUser.Organization = editSupervisorRequestDto.Organization;

                await UpdateSupervisorProjectReferences(supervisorUser, supervisorUserData.CurrentProjectReference, editSupervisorRequestDto.ProjectId);

                await _dataContext.SaveChangesAsync();
                return Success();
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }
        }

        public async Task<Result> Delete(int supervisorId)
        {
            try
            {
                await _deleteUserService.EnsureCanDeleteUser(supervisorId, Role.Supervisor);

                using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var supervisorUserData = await GetSupervisorUser(supervisorId);

                await EnsureSupervisorHasNoDataCollectors(supervisorUserData.User);

                RemoveExistingProjectReference(supervisorUserData.CurrentProjectReference);

                AnonymizeSupervisor(supervisorUserData.User);
                supervisorUserData.User.DeletedAt = _dateTimeProvider.UtcNow;

                await _identityUserRegistrationService.DeleteIdentityUser(supervisorUserData.User.IdentityUserId);
                supervisorUserData.User.IdentityUserId = null;

                await _dataContext.SaveChangesAsync();
                transactionScope.Complete();

                return Success();
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }

            async Task EnsureSupervisorHasNoDataCollectors(SupervisorUser supervisorUser)
            {
                var dataCollectorInfo = await _dataContext.DataCollectors
                    .Where(dc => dc.Supervisor == supervisorUser)
                    .Select(dc => new
                    {
                        dc,
                        IsDeleted = dc.DeletedAt != null
                    })
                    .GroupBy(dc => dc.IsDeleted)
                    .Select(g => new
                    {
                        IsDeleted = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var notDeletedDataCollectorCount = dataCollectorInfo.SingleOrDefault(dc => !dc.IsDeleted)?.Count;
                if (notDeletedDataCollectorCount > 0)
                {
                    throw new ResultException(ResultKey.User.Deletion.CannotDeleteSupervisorWithDataCollectors);
                }
            }
        }

        private async Task<SupervisorUser> CreateSupervisorUser(IdentityUser identityUser, int nationalSocietyId, CreateSupervisorRequestDto createSupervisorRequestDto)
        {
            var nationalSociety = await _dataContext.NationalSocieties.Include(ns => ns.ContentLanguage)
                .SingleOrDefaultAsync(ns => ns.Id == nationalSocietyId);

            if (nationalSociety == null)
            {
                throw new ResultException(ResultKey.User.Registration.NationalSocietyDoesNotExist);
            }

            if (nationalSociety.IsArchived)
            {
                throw new ResultException(ResultKey.User.Registration.CannotCreateUsersInArchivedNationalSociety);
            }

            var defaultUserApplicationLanguage = await _dataContext.ApplicationLanguages
                .SingleOrDefaultAsync(al => al.LanguageCode == nationalSociety.ContentLanguage.LanguageCode);

            var user = new SupervisorUser
            {
                IdentityUserId = identityUser.Id,
                EmailAddress = identityUser.Email,
                Name = createSupervisorRequestDto.Name,
                PhoneNumber = createSupervisorRequestDto.PhoneNumber,
                AdditionalPhoneNumber = createSupervisorRequestDto.AdditionalPhoneNumber,
                ApplicationLanguage = defaultUserApplicationLanguage,
                DecadeOfBirth = createSupervisorRequestDto.DecadeOfBirth,
                Sex = createSupervisorRequestDto.Sex,
                Organization = createSupervisorRequestDto.Organization
            };

            await AddNewSupervisorToProject(user, createSupervisorRequestDto.ProjectId, nationalSocietyId);

            var userNationalSociety = CreateUserNationalSocietyReference(nationalSociety, user);

            await _dataContext.AddAsync(userNationalSociety);
            await _dataContext.SaveChangesAsync();
            return user;
        }

        private async Task AddNewSupervisorToProject(SupervisorUser user, int? projectId, int nationalSocietyId)
        {
            if (projectId.HasValue)
            {
                var project = await _dataContext.Projects
                    .Where(p => p.State == ProjectState.Open)
                    .Where(p => p.NationalSociety.Id == nationalSocietyId)
                    .SingleOrDefaultAsync(p => p.Id == projectId.Value);

                if (project == null)
                {
                    throw new ResultException(ResultKey.User.Supervisor.ProjectDoesNotExistOrNoAccess);
                }

                await AttachSupervisorToProject(user, project);
            }
        }

        private async Task AttachSupervisorToProject(SupervisorUser user, Project project)
        {
            var newSupervisorUserProject = CreateSupervisorUserProjectReference(project, user);
            user.CurrentProject = project;
            await _dataContext.AddAsync(newSupervisorUserProject);
        }

        private UserNationalSociety CreateUserNationalSocietyReference(NationalSociety nationalSociety, User user) =>
            new UserNationalSociety
            {
                NationalSociety = nationalSociety,
                User = user
            };

        private SupervisorUserProject CreateSupervisorUserProjectReference(Project project, SupervisorUser supervisorUser) =>
            new SupervisorUserProject
            {
                Project = project,
                SupervisorUser = supervisorUser
            };

        private async Task UpdateSupervisorProjectReferences(SupervisorUser user, SupervisorUserProject currentProjectReference, int? selectedProjectId)
        {
            var projectHasNotChanged = selectedProjectId.HasValue && selectedProjectId.Value == currentProjectReference?.ProjectId;
            if (projectHasNotChanged)
            {
                return;
            }

            if (selectedProjectId.HasValue)
            {
                var supervisorHasNotDeletedDataCollectors = await _dataContext.DataCollectors.Where(dc => dc.Supervisor == user)
                    .AnyAsync(dc => dc.Project == dc.Supervisor.CurrentProject && !dc.DeletedAt.HasValue);

                if (supervisorHasNotDeletedDataCollectors)
                {
                    throw new ResultException(ResultKey.User.Supervisor.CannotChangeProjectSupervisorHasDataCollectors);
                }

                var project = await _dataContext.Projects
                    .Where(p => p.State == ProjectState.Open)
                    .Where(p => user.UserNationalSocieties.Select(uns => uns.NationalSocietyId).Contains(p.NationalSociety.Id))
                    .SingleOrDefaultAsync(p => p.Id == selectedProjectId.Value);

                if (project == null)
                {
                    throw new ResultException(ResultKey.User.Supervisor.ProjectDoesNotExistOrNoAccess);
                }

                await AttachSupervisorToProject(user, project);
            }

            RemoveExistingProjectReference(currentProjectReference);
        }

        private void RemoveExistingProjectReference(SupervisorUserProject existingProjectReference)
        {
            if (existingProjectReference != null)
            {
                _dataContext.SupervisorUserProjects.Remove(existingProjectReference);
            }
        }

        private void AnonymizeSupervisor(SupervisorUser supervisorUser)
        {
            supervisorUser.Name = Anonymization.Text;
            supervisorUser.EmailAddress = Anonymization.Text;
            supervisorUser.PhoneNumber = Anonymization.Text;
            supervisorUser.AdditionalPhoneNumber = Anonymization.Text;
        }

        public async Task<SupervisorUserData> GetSupervisorUser(int supervisorUserId)
        {
            var supervisorUserData = await _dataContext.Users.FilterAvailable()
                .OfType<SupervisorUser>()
                .Include(u => u.UserNationalSocieties)
                .Where(u => u.Id == supervisorUserId)
                .Select(u => new SupervisorUserData
                {
                    User = u,
                    CurrentProjectReference = u.SupervisorUserProjects
                        .SingleOrDefault(sup => sup.Project.State == ProjectState.Open)
                })
                .SingleOrDefaultAsync();

            if (supervisorUserData == null)
            {
                _loggerAdapter.Debug($"Supervisor user id {supervisorUserId} was not found");
                throw new ResultException(ResultKey.User.Common.UserNotFound);
            }

            return supervisorUserData;
        }
    }
}
