using System;
using Microsoft.Extensions.Options;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class OptionsWrapper<T> : IOptions<T> where T: class
    {
        public T Value { get; }

        public OptionsWrapper(T value)
        {
            Value = value;
        }
    }

}

