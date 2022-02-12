namespace tnORM.Querying
{
    public static class tnORMCache
    {
        private static readonly CacheInstance<int> Cache = new();


        public static bool HasKey<T>(int key, out T obj)
        {
            return Cache.HasKey(key, out obj);
        }


        public static void Expire(int key)
        {
            Cache.Expire(key);
        }


        public static void Set(int key, object value)
        {
            Cache.Set(key, value, 600);
        }


        public static void Set(int key, object value, int lifeSpan)
        {
            Cache.Set(key, value, lifeSpan);
        }
    }


    internal class CacheInstance<TKey>
    {
        private readonly Dictionary<TKey, Cache> Cache = new();

        public bool HasKey<T>(TKey key, out T result)
        {
            result = default;
            if (Cache.ContainsKey(key))
            {
                if (Cache[key].IsLive)
                {
                    result = (T)Cache[key].Object;
                    return true;
                }
                else
                {
                    Cache.Remove(key);
                }
            }
            return false;
        }


        public void Set(TKey key, object obj, int lifeSpan)
        {
            if (lifeSpan > 0)
            {
                Cache[key] = new(obj, lifeSpan);
            }
        }


        public void Expire(TKey key)
        {
            if (Cache.ContainsKey(key))
            {
                Cache.Remove(key);
            }
        }
    }


    internal struct Cache
    {
        public object Object;
        private readonly int LifeSpan { get; }
        private readonly DateTime CachedTime { get; }

        public Cache(object obj, int lifeSpan)
        {
            Object = obj;
            LifeSpan = lifeSpan;
            CachedTime = DateTime.UtcNow;
        }


        public bool IsLive
        {
            get
            {
                var timeSpan = (DateTime.UtcNow - CachedTime);
                return timeSpan.TotalSeconds < LifeSpan;
            }
        }
    }



}
