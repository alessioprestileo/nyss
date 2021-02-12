﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Features.Common;
using RX.Nyss.Web.Features.Users.Dto;
using RX.Nyss.Web.Utils;

namespace RX.Nyss.Web.Features.Users
{
    [Route("api/user")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lists all users on the national society level.
        /// </summary>
        /// <param name="nationalSocietyId">The id of the national society to list the users from</param>
        /// <returns></returns>
        [HttpGet("list")]
        [NeedsRole(Role.Administrator, Role.GlobalCoordinator, Role.Manager, Role.TechnicalAdvisor, Role.Coordinator), NeedsPolicy(Policy.NationalSocietyAccess)]
        public async Task<Result> List(int nationalSocietyId) =>
            await _userService.List(nationalSocietyId);

        /// <summary>
        /// Gets basic data about the user
        /// </summary>
        /// <param name="nationalSocietyId">User Id</param>
        /// <returns></returns>
        [HttpGet("createFormData")]
        [NeedsRole(Role.Administrator, Role.GlobalCoordinator, Role.Manager, Role.TechnicalAdvisor, Role.Coordinator)]
        public async Task<Result> GetCreateFormData(int nationalSocietyId) =>
            await _userService.GetCreateFormData(nationalSocietyId);

        /// <summary>
        /// Gets basic data about the user
        /// </summary>
        /// <param name="nationalSocietyUserId">User Id</param>
        /// <param name="nationalSocietyId">National society Id</param>
        /// <returns></returns>
        [HttpGet("editFormData")]
        [NeedsRole(Role.Administrator, Role.GlobalCoordinator, Role.Manager, Role.TechnicalAdvisor, Role.Coordinator)]
        public async Task<Result> GetEditFormData(int nationalSocietyUserId, int nationalSocietyId) =>
            await _userService.GetEditFormData(nationalSocietyUserId, nationalSocietyId);

        /// <summary>
        /// Gets modem data for adding existing user
        /// </summary>
        /// <param name="nationalSocietyId">National society Id</param>
        /// <returns></returns>
        [HttpGet("addExistingFormData")]
        [NeedsRole(Role.Administrator, Role.GlobalCoordinator, Role.Manager, Role.TechnicalAdvisor, Role.Coordinator)]
        public async Task<Result> AddExistingFormData(int nationalSocietyId) =>
            await _userService.GetAddExistingFormData(nationalSocietyId);

        /// <summary>
        /// Adds an existing technical advisor or a data consumer user to a national society.
        /// </summary>
        /// <param name="nationalSocietyId">The id of the national society</param>
        /// <param name="existingUser">The data of the existing user to be added</param>
        /// <returns></returns>
        [HttpPost("addExisting")]
        [NeedsRole(Role.Administrator, Role.GlobalCoordinator, Role.Manager, Role.TechnicalAdvisor), NeedsPolicy(Policy.NationalSocietyAccess)]
        public async Task<Result> AddExisting(int nationalSocietyId, AddExistingUserToNationalSocietyRequestDto existingUser) =>
            await _userService.AddExisting(nationalSocietyId, existingUser);
    }
}
