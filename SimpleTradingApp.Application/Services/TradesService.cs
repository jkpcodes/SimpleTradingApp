using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.IRepositories;
using SimpleTradingApp.Application.Mappers;
using SimpleTradingApp.Application.ServiceContracts;

namespace SimpleTradingApp.Application.Services;

public class TradesService : ITradesService
{
    private readonly ITradesRepository _tradesRepository;
    private readonly IAccountsRepository _accountsRepository;

    public TradesService(ITradesRepository tradesRepository, IAccountsRepository accountsRepository)
    {
        _tradesRepository = tradesRepository;
        _accountsRepository = accountsRepository;
    }

    public async Task<TradeResponse?> AddTrade(AddTradeDto addTradeDto)
    {
        // Validate that the account exists
        var account = await _accountsRepository.GetAccountById(addTradeDto.AccountId);

        if (account == null)
        {
            return null;
        }

        var result = await _tradesRepository.AddTrade(addTradeDto.ToTrade());

        if (result == null)
        {
            return null;
        }

        return result.ToTradeResponse();
    }

    public async Task<TradeResponse?> UpdateTradeStatus(UpdateTradeStatusDto updateTradeStatusDto)
    {
        var existingTrade = await _tradesRepository.GetTradeById(updateTradeStatusDto.ID);
        if (existingTrade == null)
        {
            return null;
        }

        existingTrade.Status = updateTradeStatusDto.Status;

        var updatedTrade = await _tradesRepository.UpdateTrade(existingTrade);
        if (updatedTrade == null)
        {
            return null;
        }

        return updatedTrade.ToTradeResponse();
    }
}
