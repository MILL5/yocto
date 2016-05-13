using System;
using static yocto.Preconditions;

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
            CheckIsNotNull(nameof(pet), pet);

            _pet = pet;
        }
    }

    public class CatPerson : Person
    {
        public CatPerson(ICat cat) : base(cat)
        {
            
        }
    }

    public class DogPerson : Person
    {
        public DogPerson(IDog dog) : base(dog)
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
