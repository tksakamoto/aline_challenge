using TransactionService.Models;

namespace TransactionService.Data
{
    public interface ITransactionData
    {
        void CreateTransaction(TransactionModel transaction);
        void UpdateTransaction(TransactionModel transaction);
        TransactionModel? GetTransactionByExternalId(Guid id);
        TransactionTypeModel? GetTransactionTypeById(int id);

    }
}