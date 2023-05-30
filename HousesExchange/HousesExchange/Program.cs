namespace HousesExchange
{
    public class House
    {
        public string District { get; set; }
        public double Area { get; set; }
        public string Layout { get; set; }
        public int Floor { get; set; }
        public int NumberOfRooms { get; set; }
        public List<string> Requirements { get; set; }

        public House(string district, double area, string layout, List<string> requirements, int roomCount, int floor)
    }
}