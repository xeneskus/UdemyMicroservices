using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Messages
{
    public class CreateOrderMessageCommand
    {

        public string BuyerId { get; set; }
        //public List<OrderItem> OrderItems { get; set; }
        public string Province { get; private set; }
        public string District { get; private set; }
        public string Street { get; private set; }
        public string ZipCode { get; private set; }
        public string Line { get; private set; }
    }
    public class OrdetItem
    {
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUrl { get; private set; }
        public Decimal Price { get; private set; }
    }

}
