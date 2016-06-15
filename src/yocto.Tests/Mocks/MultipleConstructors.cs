using System;

namespace yocto.tests
{
    public class MultipleConstructors
    {
        public MultipleConstructors()
        {
            
        }

        public MultipleConstructors(IAnimal animal)
        {
            
        }
    }


    public class MultipleConstructors2
    {
        static MultipleConstructors2()
        {   
        }

        public MultipleConstructors2()
        {
        }
    }
}
