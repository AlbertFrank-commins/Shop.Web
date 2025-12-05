using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Orders
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Cancelled
    }
}
