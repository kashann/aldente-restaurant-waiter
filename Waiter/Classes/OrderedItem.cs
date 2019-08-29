using Newtonsoft.Json;

namespace Waiter.Classes
{
    public class OrderedItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Observation { get; set; }
        [JsonProperty(PropertyName = "served")]
        public bool Served { get; set; }
        public bool Ready { get; set; }

        public OrderedItem() { }

        public OrderedItem(string name, int price, int quantity, string observation)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            Observation = observation;
            Served = false;
        }
    }
}