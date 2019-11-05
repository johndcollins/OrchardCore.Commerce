using System.Globalization;
using OrchardCore.Commerce.Money;
using OrchardCore.Commerce.Services;
using OrchardCore.Commerce.Settings;
using OrchardCore.Commerce.Tests.Fakes;
using Xunit;
using static OrchardCore.Commerce.Money.Currency;
using static OrchardCore.Commerce.Tests.Fakes.AnkhMorporkCurrencyProvider;

namespace OrchardCore.Commerce.Tests
{
    public partial class MoneyServiceTests
    {
        [Fact]
        public void DefaultCurrencyWithoutSettingsOrProvidersIsDollar()
        {
            Assert.Equal("USD", new MoneyService(null, null).DefaultCurrency.IsoCode);
        }

        [Fact]
        public void DefaultCurrencyWithNullSettingsButNoProvidersIsDollar()
        {
            Assert.Equal(
                "USD",
                new MoneyService(
                    null,
                    new TestOptions<CommerceSettings>(new CommerceSettings { })
                    ).DefaultCurrency.IsoCode);
        }

        [Fact]
        public void DefaultCurrencyWithSettingsSpecifyingDefaultCurrencyIsObserved()
        {
            Assert.Equal("EUR", new TestMoneyService().DefaultCurrency.IsoCode);
        }

        [Fact]
        public void NotFoundDefaultCurrencyFallsBackToDollar()
        {
            Assert.Equal(
                "USD",
                new MoneyService(null, new TestOptions<CommerceSettings>(
                    new CommerceSettings {
                        DefaultCurrency = "WTF"
                    })).DefaultCurrency.IsoCode);
        }

        [Fact]
        public void CanGetCurrenciesFromMultipleProviders()
        {
            Assert.Equal("EUR", new TestMoneyService().GetCurrency("EUR").IsoCode);
            Assert.Equal("AMD", new TestMoneyService().GetCurrency("AMD").IsoCode);
        }

        [Fact]
        public void UnknownCurrencyCodeGivesNullResult()
        {
            Assert.Null(new TestMoneyService().GetCurrency("WTF"));
        }

        [Fact]
        public void CreateMakesAmountWithCurrency()
        {
            var service = new TestMoneyService();
            var amount = service.Create(42, "AMD");

            Assert.Equal(42, amount.Value);
            Assert.Equal(service.GetCurrency("AMD"), amount.Currency);
        }

        [Fact]
        public void EnsureCurrencyAddsRealCurrencyForCodeThatExists()
        {
            var service = new TestMoneyService();
            Amount amount = service.EnsureCurrency(new Amount(42, new Currency(null, null, null, "AMD")));
            Assert.Equal(42, amount.Value);
            Assert.Equal(service.GetCurrency("AMD"), amount.Currency);
        }
    }
}
