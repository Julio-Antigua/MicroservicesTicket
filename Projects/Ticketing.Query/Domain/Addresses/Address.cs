using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketing.Query.Domain.Addresses
{
    [ComplexType]
    public class Address
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
