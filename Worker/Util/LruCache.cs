using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker.Util
{
    public class LruCache<K, V>
    {
        private readonly int Capacity;
        private Dictionary<K, LinkedListNode<LruCacheItem<K, V>>> CacheMap = new Dictionary<K, LinkedListNode<LruCacheItem<K, V>>>();
        private LinkedList<LruCacheItem<K, V>> LruList = new LinkedList<LruCacheItem<K, V>>();

        public LruCache(int capacity)
        {
            Capacity = capacity;
        }

        public V Get(K key)
        {
            LinkedListNode<LruCacheItem<K, V>> node;
            if (CacheMap.TryGetValue(key, out node))
            {
                V value = node.Value.Value;
                LruList.Remove(node);
                LruList.AddLast(node);
                return value;
            }
            return default(V);
        }

        public void Add(K key, V val)
        {
            if (CacheMap.Count >= Capacity)
            {
                RemoveFirst();
            }

            LruCacheItem<K, V> cacheItem = new LruCacheItem<K, V>(key, val);
            LinkedListNode<LruCacheItem<K, V>> node = new LinkedListNode<LruCacheItem<K, V>>(cacheItem);
            CacheMap.Add(key, node);
            LruList.AddLast(node);
        }

        private void RemoveFirst()
        {
            LinkedListNode<LruCacheItem<K, V>> node = LruList.First;
            LruList.RemoveFirst();
            CacheMap.Remove(node.Value.Key);
        }
    }

    class LruCacheItem<K, V>
    {
        public LruCacheItem(K k, V v)
        {
            Key = k;
            Value = v;
        }
        public K Key;
        public V Value;
    }
}
