using NautiliusCommBasic.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NautiliusCommBasic.Api.Service
{
    [ServiceContract]
    public interface INCommService
    {
        [OperationContract]
        void SendMessage(Message message);

        [OperationContract]
        void TestConnection();
    }
}
