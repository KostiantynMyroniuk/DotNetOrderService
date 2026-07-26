using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public record PaginatedList<T>(
        ICollection<T> Items, 
        int PageNumber, 
        int PageSize, 
        int TotalCount);
    
}
