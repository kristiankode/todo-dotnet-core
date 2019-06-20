using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToDoApi.Tests.IntegrationTests
{
    public class JsonHelper
    {
        public static StringContent ConvertObjectToStringContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }

        public static T ToObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}