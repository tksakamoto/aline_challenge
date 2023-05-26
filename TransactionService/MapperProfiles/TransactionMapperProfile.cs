using AutoMapper;
using TransactionService.Dtos;
using TransactionService.Models;

namespace TransactionService.MapperProfiles {

    public class TransactionProfile : Profile
    {
        public TransactionProfile(){
            
            CreateMap<TransactionModel, TransactionReadDto>();
            CreateMap<TransactionModel, TransactionFraudCheckDto>();
            CreateMap<TransactionCreateDto, TransactionModel>();            
            CreateMap<TransactionStatusModel, TransactionStatusReadDto>();
            CreateMap<TransactionTypeModel, TransactionTypeReadDto>();
        }
        
    }

}