using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTradingApp.Application.DTOs;
public class PagingParameters
{
    public static readonly int MinPageNumber = 1;
    public static readonly int MinPageSize = 1;
    public static readonly int MaxPageSize = 100;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
