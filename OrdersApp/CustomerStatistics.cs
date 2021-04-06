using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersApp
{
    class CustomerStatistics
    {
        public int customerId;
        public int ordersCount;
        public int totalCost;

        public override string ToString()
        {
            return $"Customer number {customerId} made {ordersCount} orders and spent {totalCost}"; 
        }
    }
}
