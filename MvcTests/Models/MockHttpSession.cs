using System.Collections.Generic;
using System.Web;

namespace MvcTests.Models
{
    class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string, object> _sessionDictionary = new Dictionary<string, object>();

        public override object this[string name]
        {
            get { return _sessionDictionary[name]; }

            set { _sessionDictionary[name] = value; }
        }
    }
}
