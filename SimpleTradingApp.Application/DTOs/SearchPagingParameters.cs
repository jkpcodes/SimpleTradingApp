using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTradingApp.Application.DTOs;

public class SearchPagingParameters : PagingParameters
{
    public string? LastName { get; set; } = "";
    public string? ID { get; set; } = "";
}
