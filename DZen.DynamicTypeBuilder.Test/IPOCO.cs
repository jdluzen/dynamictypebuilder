namespace DZen.DynamicTypeBuilder.Test
{
    public interface IPOCO
    {
        int Id { get; set; }
        string Name { get; set; }
        (int width, int height) Dimensions { get; set; }
    }
}
