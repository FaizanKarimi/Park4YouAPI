using System.Collections.Generic;
using System.Linq;
using Dapper;
using DapperExtensions;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class ChargeSheetPriceRepository : IChargeSheetPriceRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public bool Add(List<ChargeSheetPrices> chargeSheetPrices)
        {
            const string sqlQuery = @"INSERT INTO ChargeSheetPrices (ChargeSheetId, ChargeSheetRuleId, AttributeKey, AttributeValue, UpdatedOn, CreatedOn)
                                      VALUES (@ChargeSheetId, @ChargeSheetRuleId, @AttributeKey, @AttributeValue, @UpdatedOn, @CreatedOn)";
            int result = UnitOfWork.Connection.Execute(sqlQuery, chargeSheetPrices, UnitOfWork.Transaction);
            return result > 0;
        }

        public List<ChargeSheetPrices> Get(int chargeSheetId)
        {
            const string sqlQuery = @"SELECT * FROM ChargeSheetPrices WHERE ChargeSheetId = @ChargeSheetId";
            return UnitOfWork.Connection.Query<ChargeSheetPrices>(sqlQuery, new { ChargeSheetId = chargeSheetId }).ToList();
        }

        public bool Update(List<ChargeSheetPrices> chargeSheetPrices)
        {
            const string sqlQuery = @"UPDATE ChargeSheetPrices SET AttributeValue = @AttributeValue, UpdatedOn = @UpdatedOn WHERE Id = @Id AND AttributeKey = @AttributeKey";
            int result = UnitOfWork.Connection.Execute(sqlQuery, chargeSheetPrices, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}