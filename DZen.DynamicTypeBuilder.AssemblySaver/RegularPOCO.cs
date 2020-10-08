using DZen.DynamicTypeBuilder.Test;

namespace DZen.DynamicTypeBuilder.AssemblySaver
{
    public class RegularPOCO : IPOCO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public (int width, int height) Dimensions { get; set; }
    }
}
