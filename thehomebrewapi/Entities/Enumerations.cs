using System.ComponentModel;

namespace thehomebrewapi.Entities
{
    public class Enumerations
    {
        public enum ETypeOfResourceUri : short
        {
            PreviousPage,
            NextPage,
            Current
        }

        public enum ETypeOfBeer : short
        {
            [Description("Not specified")]
            NotSpecified,
            [Description("Light lager")]
            LightLager,
            [Description("Pilsner")]
            Pilsner,
            [Description("European amber lager")]
            EurAmberLager,
            [Description("Dark lager")]
            DarkLager,
            [Description("Bock")]
            Bock,
            [Description("Light hybrid beer")]
            LightHybridBeer,
            [Description("Amber hybrid beer")]
            AmberHybridBeer,
            [Description("English pale ale")]
            EngPaleAle,
            [Description("Scottish and Irish ale")]
            ScotIrishAle,
            [Description("American ale")]
            AmericanAle,
            [Description("English brown ale")]
            EngBrownAle,
            [Description("Porter")]
            Porter,
            [Description("Stout")]
            Stout,
            [Description("India pale ale (IPA)")]
            IPA,
            [Description("German wheat and rye beer")]
            GerWheatRyeBeer,
            [Description("Belgian and French ale")]
            BelFrAle,
            [Description("Sour ale")]
            Sour,
            [Description("Belgian strong ale")]
            BelStrongAle,
            [Description("Strong ale")]
            StrongAle,
            [Description("Fruit beer")]
            FruitBeer,
            [Description(@"Spice/herb/vegetable beer")]
            SpiceHerbVegBeer,
            [Description("Smoke flavoured and wood-aged beer")]
            SmokeAgedBeer,
            [Description("Speciality beer")]
            Speciality,
            [Description("Kolsch and altbier")]
            KolschAlt
        }

        public enum ETypeOfIngredient : short
        {
            [Description("Grains")]
            Grains,
            [Description("Hops")]
            Hops,
            [Description("Adjuncts")]
            Adjuncts
        }

        public enum EUnitOfMeasure
        {
            [Description("kg")]
            kilo = 0,
            [Description("g")]
            gram,
            [Description("l")]
            litre = 100,
            [Description("ml")]
            millilitre
        }

        public enum ETypeOfDuration
        {
            [Description("None")]
            none = 0,
            [Description("Independent")]
            independent,
            [Description("Before flameout")]
            beforeFlameout,
            [Description("After flameout")]
            afterFlameout
        }

        public enum EBrewedState
        {
            notBrewed = 0,
            brewing,
            brewed
        }
    }
}
