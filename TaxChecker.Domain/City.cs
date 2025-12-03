namespace TaxChecker.Infrastructure
{
    public class City
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public City(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("City name cannot be empty.");

            Name = name.Trim();
        }
    }
}
