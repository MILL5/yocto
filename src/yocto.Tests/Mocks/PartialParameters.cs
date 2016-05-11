using System;

namespace yocto.tests
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
