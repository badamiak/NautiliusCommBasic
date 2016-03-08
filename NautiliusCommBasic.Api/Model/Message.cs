using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NautiliusCommBasic.Api.Model
{
    [DataContract]
    [Serializable]
    public class Message
    {
        [DataMember]
        public DateTime Created { get; set; } = DateTime.Now;
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Text { get; set; }
    }
}
