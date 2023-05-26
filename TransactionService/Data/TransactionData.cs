using Microsoft.EntityFrameworkCore;
using TransactionService.Models;

namespace TransactionService.Data
{
    public class TransactionData : ITransactionData
    {
        private AppDbContext _context;

        public TransactionData(AppDbContext context)
        {
            _context = context;
        }


        public void CreateTransaction(TransactionModel transaction)
        {
            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _context.Add(transaction);
            _context.SaveChanges();
        }


        public void UpdateTransaction(TransactionModel transaction)
        {
            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _context.Update(transaction);
            _context.SaveChanges();
        }


        public TransactionModel? GetTransactionByExternalId(Guid id)
        {
            return _context.Transactions
                .Include(t => t.TransactionStatus)
                .Include(t => t.TransactionType)
                .FirstOrDefault(t => t.TransactionExternalId == id);
        }

        public TransactionTypeModel? GetTransactionTypeById(int id)
        {
            return _context.TransactionTypes.FirstOrDefault(type => type.TransactionTypeId == id);
        }

    }
}