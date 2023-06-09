using System;
using System.Collections.Generic;
using UnityEngine;

namespace instance.id.EATK
{
    [Serializable]
    public class SerializedDictionary<KeyType, TValue> : Dictionary<KeyType, TValue>, ISerializationCallbackReceiver
    {
        public bool isCustomEditor = false;
        public const string KeyProperty = nameof(_keys);
        public const string ValueProperty = nameof(_values);

        [SerializeField] internal List<SerializedKeyValuePair<KeyType, TValue>> serializedData 
            = new List<SerializedKeyValuePair<KeyType, TValue>>();
        [SerializeField] internal KeyType editorAddKey;
        [SerializeField] internal TValue editorAddValue;

        // These are protected so they can be found by the editor.
        [SerializeField] protected List<KeyType> _keys = new List<KeyType>();
        [SerializeField] protected List<TValue> _values = new List<TValue>(); // @formatter:off

        public SerializedDictionary() { } 
        public SerializedDictionary(IDictionary<KeyType, TValue> dictionary) // @formatter:on
        {
            if (dictionary != null)
                foreach (KeyValuePair<KeyType, TValue> keyValuePair in (IEnumerable<KeyValuePair<KeyType, TValue>>) dictionary)
                    this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public SerializedDictionary(IDictionary<KeyType, TValue> dictionary, IEqualityComparer<KeyType> comparer)
        {
            if (dictionary != null)
                foreach (KeyValuePair<KeyType, TValue> keyValuePair in (IEnumerable<KeyValuePair<KeyType, TValue>>) dictionary)
                    this.Add(keyValuePair.Key, keyValuePair.Value);
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize() => ConvertToLists();
        void ISerializationCallbackReceiver.OnAfterDeserialize() => ConvertFromLists();

        private void ConvertToLists()
        {
            _keys.Clear();
            _values.Clear();
            serializedData.Clear();

            foreach (var entry in this)
            {
                _keys.Add(entry.Key);
                _values.Add(entry.Value);
                serializedData.Add(new SerializedKeyValuePair<KeyType, TValue>(entry.Key, entry.Value));
            }
        }

        private void ConvertFromLists() // @formatter:off
        {
            Clear();

            var count = Math.Min(_keys.Count, _values.Count);
            var failed = 0;
            try { for (var i = 0; i < count; i++) { failed = i; Add(_keys[i], _values[i]); } }
            catch (Exception e) { Debug.Log(e.ToString()); throw; }
        } // @formatter:on
    }

    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializedKeyValuePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static implicit operator KeyValuePair<TKey, TValue>(SerializedKeyValuePair<TKey, TValue> data) => new KeyValuePair<TKey, TValue>(data.Key, data.Value);
        public static implicit operator SerializedKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> data) => new SerializedKeyValuePair<TKey, TValue>(data.Key, data.Value);

        public override string ToString() => $"key: {Key}, value: {Value}";
    }
}
