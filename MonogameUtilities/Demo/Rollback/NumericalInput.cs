

using MonogameUtilities.Networking;
using System;

namespace MonogameUtilities.Demo.Rollback
{
    public class NumericalInput : IStringableGeneric
    {
        public readonly Numbers Number;

        public NumericalInput(Numbers number)
        {
            Number = number;
        }

        public IStringableGeneric FromString(string value)
        {
            Numbers number = (Numbers)Enum.Parse(typeof(Numbers), value);

            return new NumericalInput(number);
        }

        public static NumericalInput StaticFromString(string value)
        {
            Numbers number = (Numbers)Enum.Parse(typeof(Numbers), value);

            return new(number);
        }

        public enum Numbers
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine
        }

    }
}
