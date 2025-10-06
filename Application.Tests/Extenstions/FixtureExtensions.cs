using AutoFixture;

namespace Application.Tests.Extenstions
{
    internal static class FixtureExtensions
    {
        public static Fixture FixCircularReference(this Fixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}
