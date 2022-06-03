using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitmqSignalRService.Models
{
    public class Stock
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
