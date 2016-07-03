using System;

namespace yocto.tests
{
    public interface IPerson
    {
        
    }

    public class Person : IPerson
    {
        private readonly IAnimal _pet;

        public Person(IAnimal pet)
        {
            _pet = pet;
        }
    }

    public class CatPerson : Person
    {
        public CatPerson(ICat cat) : base(cat)
        {
            
        }
    }

    public class LizardPerson : Person
    {
        public LizardPerson(ILizard lizard) : base(lizard)
        {
        }
    }

    public class MonkeyPerson : Person
    {
        public MonkeyPerson(IMonkey monkey) : base(monkey)
        {
        }
    }
}
