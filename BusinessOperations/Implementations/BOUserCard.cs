using System.Collections.Generic;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Components.Services.QuickPay;
using Infrastructure.DataModels;
using Infrastructure.Helpers;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOUserCard : IBOUserCard
    {
        #region Private Members
        private readonly IUserCardService _userCardService;
        private readonly ILogging _logger;
        private readonly IQuickPayService _quickPayService;
        #endregion

        #region Constructor
        public BOUserCard(IUserCardService userCardService, ILogging logging, IQuickPayService quickPayService)
        {
            _userCardService = userCardService;
            _logger = logging;
            _quickPayService = quickPayService;
        }
        #endregion

        #region Public Methods
        public UserCards Add(UserCards userCards)
        {
            _logger.Debug("Add user card process started.");
            UserCards userCard = null;
            string expirationMonth = CommonHelpers.GetCardExpirationMonth(userCards.CardExpiry);
            string expirationYear = CommonHelpers.GetCardExpirationYear(userCards.CardExpiry);
            _logger.Debug(string.Format("Processing the card for verification."));
            _quickPayService.ValidateCard(userCards.MobileNumber, expirationMonth, expirationYear, userCards.CardVerficationValue, userCards.CardNumber);
            _logger.Debug(string.Format("Card is successfully verified."));
            userCard = _userCardService.Add(userCards);
            _logger.Debug("Add user card process ended.");
            return userCard;
        }

        public bool Delete(string mobileNumber, int cardId)
        {
            _logger.Debug("Delete user card process started.");
            bool IsDeleted = false;
            UserCards userCards = new UserCards()
            {
                Id = cardId,
                MobileNumber = mobileNumber
            };
            IsDeleted = _userCardService.Delete(userCards);
            _logger.Debug("Delete user card process ended.");
            return IsDeleted;
        }

        public List<UserCards> GetALL(string mobileNumber)
        {
            _logger.Debug("Getting all user cards process started.");
            List<UserCards> userCards = null;
            userCards = _userCardService.GetALL(mobileNumber);
            _logger.Debug("Getting all user cards process ended.");
            return userCards;
        }

        public bool MarkedUserCardDefault(int cardId, string mobileNumber)
        {
            return _userCardService.SetDefaultUserCard(cardId, mobileNumber);
        }
        #endregion
    }
}