using System.Collections.Generic;
using Newtonsoft.Json;

namespace Waiter.Classes
{
    public class Table
    {
        [JsonProperty(PropertyName = "table_number")]
        public int TableNumber { get; set; }
        [JsonProperty(PropertyName = "status")]
        public EStatus Status { get; set; }
        [JsonProperty(PropertyName = "payment")]
        public string Payment { get; set; }
        [JsonProperty(PropertyName = "total")]
        public decimal? Total { get; set; }
        [JsonProperty(PropertyName = "tip")]
        public decimal? Tip { get; set; }
        [JsonProperty(PropertyName = "waiter")]
        public int WaiterId { get; set; }
        [JsonProperty(PropertyName = "orders")]
        public List<OrderedItem> Order { get; set; }

        public Table()
        {
            Order = new List<OrderedItem>();
        }

        public Table(int table, EStatus status, int id)
        {
            TableNumber = table;
            Status = status;
            WaiterId = id;
            Order = new List<OrderedItem>();
        }

        public Table(int table, EStatus status, int id, List<OrderedItem> comanda)
        {
            TableNumber = table;
            Status = status;
            WaiterId = id;
            Order = comanda;
        }
    }
}