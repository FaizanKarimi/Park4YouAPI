using System;
using System.Threading.Tasks;
using BusinessOperations.Interfaces;
using Components.Identity;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.Vendors;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        #region Private Members
        private readonly IBOUser _bOUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBOUserProfile _bOUserProfile;
        #endregion

        #region Constructor
        public VendorController(IBOUser bOUser, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IBOUserProfile bOUserProfile)
        {
            _bOUser = bOUser;
            _userManager = userManager;
            _roleManager = roleManager;
            _bOUserProfile = bOUserProfile;
        }
        #endregion

        #region API'S
        [HttpGet]
        [Route("get-vendors")]
        public IActionResult GetVendors()
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUser.GetVendors();
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("add-vendor")]
        public async Task<IActionResult> AddVendorAsync(AddVendorRequest addVendorRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            var user = await this._userManager.FindByEmailAsync(addVendorRequest.Email);
            if (user != null)
            {
                bool emailAlreadyExist = !user.EmailConfirmed;
                if (emailAlreadyExist)
                    throw new Park4YouException(ErrorMessages.USER_EMAIL_ALREADY_EXIST);
            }
            else
            {
                bool IsAdded = await _AddVendorAsync(addVendorRequest);
                if (IsAdded)
                {
                    ApplicationUser newUser = await this._userManager.FindByEmailAsync(addVendorRequest.Email);
                    UserProfiles userProfile = new UserProfiles()
                    {
                        UserId = newUser.Id,
                        FirstName = addVendorRequest.FirstName,
                        LastName = addVendorRequest.LastName,
                        MobileNumber = addVendorRequest.MobileNumber,
                        EmailAddress = addVendorRequest.Email,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        UpdatedBy = newUser.Id
                    };
                    basicResponse.Data = _bOUserProfile.AddUserProfile(userProfile);
                }
                else
                {
                    throw new Park4YouException(ErrorMessages.USER_NOT_CREATED);
                }
            }
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("edit-vendor")]
        public async Task<IActionResult> EditVendorAsync(EditVendorRequest editVendorRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await this._userManager.FindByIdAsync(editVendorRequest.Id);

            user.Email = editVendorRequest.Email;
            user.MobileNumber = editVendorRequest.MobileNumber;
            user.UserName = editVendorRequest.Email;
            IdentityResult result = await this._userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                basicResponse.Data = result.Succeeded;
                if (!string.IsNullOrEmpty(editVendorRequest.Password))
                {
                    await this._userManager.RemovePasswordAsync(user);
                    await this._userManager.AddPasswordAsync(user, editVendorRequest.Password);
                }
                UserProfiles userProfile = new UserProfiles()
                {
                    FirstName = editVendorRequest.FirstName,
                    LastName = editVendorRequest.LastName,
                    MobileNumber = editVendorRequest.MobileNumber,
                    UserId = editVendorRequest.Id,
                    IsDeleted = false
                };
                basicResponse.Data = _bOUserProfile.UpdateByUserId(userProfile);
            }
            return Ok(basicResponse);
        }

        [HttpGet]
        [Route("get-vendor/{id}")]
        public async Task<IActionResult> GetVendorAsync(string id)
        {
            BasicResponse basicResponse = new BasicResponse();
            var user = await this._userManager.FindByIdAsync(id);
            if (user != null)
            {
                UserProfiles userProfile = _bOUserProfile.Get(user.Id);
                if (userProfile == null)
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

                var obj = new
                {
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    MobileNumber = userProfile.MobileNumber,
                    Email = user.Email
                };
                basicResponse.Data = obj;
            }
            else
            {
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);
            }
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("delete-vendor")]
        public async Task<IActionResult> DeleteVendorAsync(DeleteVendorRequest deleteVendorRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser applicationUser = await this._userManager.FindByIdAsync(userId: deleteVendorRequest.UserId);
            if (applicationUser != null)
            {
                if (deleteVendorRequest.Status)
                {
                    IdentityResult result = await this._userManager.SetLockoutEnabledAsync(applicationUser, true);
                    if (result.Succeeded)
                    {
                        result = await this._userManager.SetLockoutEndDateAsync(applicationUser, DateTimeOffset.MaxValue);
                        basicResponse.Data = true;
                    }
                }
                else
                {
                    IdentityResult result = await this._userManager.SetLockoutEnabledAsync(applicationUser, false);
                    if (result.Succeeded)
                    {
                        result = await this._userManager.ResetAccessFailedCountAsync(applicationUser);
                        basicResponse.Data = true;
                    }
                }
            }
            else
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);

            return Ok(basicResponse);
        }
        #endregion

        #region Private Methods
        private async Task<bool> _AddVendorAsync(AddVendorRequest addVendorRequest)
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = addVendorRequest.Email,
                Email = addVendorRequest.Email,
                EmailConfirmed = true,
                MobileNumber = addVendorRequest.MobileNumber,
                DateRegistration = DateTime.UtcNow
            };

            IdentityResult result = await this._userManager.CreateAsync(applicationUser, addVendorRequest.Password);
            if (result.Succeeded)
            {
                result = await this._userManager.AddToRoleAsync(applicationUser, UserRoles.Vendor.ToString());
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}