using Newtonsoft.Json;

namespace Navitus
{
    // Just for responding JSON message back to user
    public class ResponseMessage
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ResponseMessage(bool status, string message)
        {
            this.Status = status;
            this.Message = message;
            Json();
        }

        public string Json()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
