using Infrastructure.Services;

namespace Schiza.Tests.InfrastructureTests
{
    public class TokenCounterTests
    {
        [Fact]
        public void CalcTokenCount_ReturnNotZero()
        {
            var counter = new TokenCounter();

            var result = counter.CalcTokenCount("Какой-нибудь текст!");

            Assert.True(result > 0);
        }

        [Fact]
        public void CalcTokenCount_ReturnZero()
        {
            var counter = new TokenCounter();

            var result = counter.CalcTokenCount("");

            Assert.True(result == 0);
        }
    }
}
