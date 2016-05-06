using System;
using System.Runtime.Remoting.Contexts;
using static mill5.yocto.Preconditions;

namespace mill5.yocto.Tests
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

        public override string ToString()
        {
            return $"{GetType().Name} has a {_pet.GetType().Name}";
        }
    }

    public class CatPerson : Person
    {
        public CatPerson(Cat cat) : base(cat)
        {
            
        }
    }
}
