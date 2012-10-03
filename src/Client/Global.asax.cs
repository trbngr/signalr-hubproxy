using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Hubs;
using SignalR.Client.Hubs;
using SignalR.Hubs;

namespace Client
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            HubProxies.Init();
        }
    }

    public class HubProxies
    {
        private readonly HubConnection _hubConnection;
        private readonly IDictionary<Type, HubProxy> _proxies;

        public static HubProxies Instance { get; private set; }

        internal static void Init()
        {
            Instance = new HubProxies();
        }

        private HubProxies()
        {
            var url = ConfigurationManager.AppSettings["signalr:host"];

            _hubConnection = new HubConnection(url);
            _proxies = CreateKnownProxies(_hubConnection);
            Debug.WriteLine(string.Format("Starting SignalR connection to: {0}", url));
            _hubConnection.Start()
                .ContinueWith(t => Debug.WriteLine(t.IsFaulted ? "SignalR connection failed" : "SignalR connection succeeded"))
                .Wait();
        }

        private IDictionary<Type, HubProxy> CreateKnownProxies(HubConnection connection)
        {
            var proxies = typeof(ValuesHub).Assembly.GetExportedTypes()
                .Where(t => typeof(IHub).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToDictionary(t => t, t => new HubProxy(connection, GetHubName(t)));

            return proxies.WithDefaultValue(new HubProxy(connection, "noexistence"));
        }

        public IHubProxy Get<T>() where T : IHub
        {
            return _proxies[typeof(T)];
        }

        private string GetHubName(Type type)
        {
            HubNameAttribute attribute =
                type.GetCustomAttributes(typeof(HubNameAttribute), false).Cast<HubNameAttribute>().SingleOrDefault();
            return attribute != null ? attribute.HubName : type.Name;
        }


    }

    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> WithDefaultValue<TValue, TKey>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            return new DefaultableDictionary<TKey, TValue>(dictionary, defaultValue);
        }
    }
    public class DefaultableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly TValue _defaultValue;
        private readonly IDictionary<TKey, TValue> _dictionary;

        public DefaultableDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            _dictionary = dictionary;
            _defaultValue = defaultValue;
        }

        #region  Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Remove(item);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!_dictionary.TryGetValue(key, out value))
                value = _defaultValue;

            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                try
                {
                    return _dictionary[key];
                }
                catch (KeyNotFoundException)
                {
                    return _defaultValue;
                }
            }

            set { _dictionary[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>(_dictionary.Values)
                {
                    _defaultValue
                };
                return values;
            }
        }

        #endregion
    }

}