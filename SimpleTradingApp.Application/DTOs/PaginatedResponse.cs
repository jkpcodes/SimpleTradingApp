namespace SimpleTradingApp.Application.DTOs;

public record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages
);