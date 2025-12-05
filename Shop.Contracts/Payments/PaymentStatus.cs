using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Payments
{
    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Failed
    }
}
