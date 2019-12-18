using System;
namespace CarStoreWeb.Data
{
    public class Proxy
    {
        protected string _endpoint;
        protected string _key;

        protected Proxy(string endpoint, string key)
        {
            if(endpoint == null || key == null)
            {
                throw new ArgumentNullException();
            }

            if(string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException();
            }

            _endpoint = endpoint;
            _key = key;

            if(!_endpoint.EndsWith("/", StringComparison.InvariantCulture))
            {
                _endpoint += "/";
            }
        }
    }
}
