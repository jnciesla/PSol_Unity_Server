
namespace Data.Models
{
    public class LootItem
    {
        public string ID { get; set; }
        public string ItemID { get; set; }
        public int MinQTY { get; set; }
        public int MaxQTY { get; set; }
        public int Chance { get; set; }
    }
}
