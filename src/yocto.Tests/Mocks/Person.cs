using System;
using System.Runtime.Remoting.Contexts;
using static yocto.Preconditions;

namespace yocto.Tests
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
        public CatPerson(Cat cat) : base(cat)
        {
            
        }
    }
}
