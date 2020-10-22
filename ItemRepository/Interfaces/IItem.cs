namespace ItemRepository.Interfaces
{
    public interface IItem
    {
        public int Id { get; set; }

        public string ItemName { get; set; }

        public int Cost { get; set; }
    }
}