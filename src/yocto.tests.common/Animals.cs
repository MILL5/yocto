using System;

namespace yocto.tests
{
    public interface IAnimal
    {
    }

    public interface ICat: IAnimal
    {
    }

    public interface IDog : IAnimal
    {
    }

    public class Dog : IDog
    {
    }

    public class Cat : ICat
    {
    }

    public interface ILizard : IAnimal
    {
    }

    public class Lizard : ILizard
    {
    }

    public interface IMonkey : IAnimal
    {
    }

    public class Monkey : IMonkey
    {
    }
}
