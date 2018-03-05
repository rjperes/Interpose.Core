namespace Interpose.Core.Generators
{
    public static class TypeGeneratorExtensions
    {
        public static InterceptedTypeGenerator AsCached(this InterceptedTypeGenerator generator)
        {
            return new CachedTypeGenerator(generator);
        }
    }
}
