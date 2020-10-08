using DZen.DynamicTypeBuilder.Test;
using System;

namespace DZen.DynamicTypeBuilder.AssemblySaver
{
    class Program
    {
        static void Main(string[] args)
        {
            IInterfaceBuilder builder = new InterfaceBuilder();
            Type t = builder.Implement<IPOCO>();
            var ipoco = Activator.CreateInstance(t) as IPOCO;
            InterfaceBuilder.assemblyBuilder.Save("doesnt matter");

            ipoco.Id = 47;
            Console.WriteLine(ipoco.Id);
        }
    }
}
