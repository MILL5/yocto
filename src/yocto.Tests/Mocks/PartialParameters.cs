using System;

namespace yocto.Tests
{
    public interface IUnknownParameter
    {

    }

    public class UnknownParameter : IUnknownParameter
    {
        
    }

    public class PartialParameters
    {
        public PartialParameters(IAnimal animal, IUnknownParameter up)
        {
            
        }
    }
}
