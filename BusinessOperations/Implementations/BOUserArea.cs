using System.Collections.Generic;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOUserArea : IBOUserArea
    {
        #region Private Members
        private readonly IUserAreaService _userAreaService;
        #endregion

        #region Constructor
        public BOUserArea(IUserAreaService userAreaService)
        {
            _userAreaService = userAreaService;
        }
        #endregion

        #region Public Methods
        public UserAreas Add(UserAreas userArea)
        {
            return _userAreaService.AddUserArea(userArea);
        }

        public bool Delete(int id)
        {
            return _userAreaService.DeleteArea(id);
        }

        public List<UserAreas> GetUserAreas(string mobileNumber)
        {
            return _userAreaService.GetUserAreas(mobileNumber);
        } 
        #endregion
    }
}