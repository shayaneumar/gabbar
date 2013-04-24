using System.Runtime.Serialization;

namespace BuildingSecurity.Web.Api.Models
{
    [DataContract]
    public class SimulatorScript
    {
        [DataMember(Name = "script")]
        public string Script { get; set; }
    }
}
