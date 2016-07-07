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

    public class MultipleConstructors3
    {
        static MultipleConstructors3()
        {
        }

        public MultipleConstructors3(object test)
        {
        }

        public MultipleConstructors3(object[] test)
        {
        }
    }
}
