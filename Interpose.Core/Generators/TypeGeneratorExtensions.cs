namespace Interpose.Core.Generators
{
    public static class TypeGeneratorExtensions
    {
        public static InterceptedTypeGenerator AsCached(this InterceptedTypeGenerator generator)
        {
            if (generator is CachedTypeGenerator)
            {
                return generator;
            }

            return new CachedTypeGenerator(generator);
        }
    }
}
