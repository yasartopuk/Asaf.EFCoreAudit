namespace EFCore.Interceptor.ConsoleApp
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public override string ToString() => $"{{ Name={Name}, Price:{Price} }}";
    }
}