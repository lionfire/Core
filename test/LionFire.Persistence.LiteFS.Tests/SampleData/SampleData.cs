using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.LiteFs.Tests
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }

    public class SampleData
    {
        static Random r = new();

        public static Customer GetCustomer()
        {
            var customer = new Customer
            {
                Name = "John Doe",
                Phones = new string[] { $"{r.Next(999)}-{r.Next(999)}-{r.Next(9999)}", $"{r.Next(999)}-{r.Next(999)}-{r.Next(9999)}" },
                IsActive = r.NextDouble() > 0.5,
            };
            return customer;
        }
    }
}
