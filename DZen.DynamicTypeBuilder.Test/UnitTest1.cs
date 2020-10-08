using FluentAssertions;
using System;
using Xunit;

namespace DZen.DynamicTypeBuilder.Test
{
    public class UnitTest1
    {
        [Fact]
        public void BuildsType()
        {
            IInterfaceBuilder builder = new InterfaceBuilder();

            Type type = builder.Implement<IPOCO>();

            IPOCO poco = Activator.CreateInstance(type) as IPOCO;

            poco.Id = 47;
            poco.Dimensions = (1, 2);
            poco.Name = "Joe";

            poco.Id.Should().Be(47);
            poco.Dimensions.Should().Be((1, 2));
            poco.Name.Should().Be("Joe");
        }

        [Fact]
        public void MustBeInterface()
        {
            IInterfaceBuilder builder = new InterfaceBuilder();

            Action act = () =>
            {
                Type type = builder.Implement<TestPOCO>();
            };
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CustomTypeName()
        {
            IInterfaceBuilder builder = new InterfaceBuilder();

            Type type = builder.Implement<IPOCO>("Custom");
            type.Name.Should().Be("Custom");
        }
    }
}
