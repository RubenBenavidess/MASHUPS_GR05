using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;

namespace TicketPremium.Console.Servicios
{
    public class SessionTokenInspector : IClientMessageInspector, IEndpointBehavior
    {
        private readonly Func<string?> _getToken;

        public SessionTokenInspector(Func<string?> getToken)
        {
            _getToken = getToken;
        }

        public object? BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var token = _getToken();
            if (string.IsNullOrEmpty(token)) return null;

            if (request.IsEmpty) return null;

            using var buffer = request.CreateBufferedCopy(int.MaxValue);
            var oldMsg = buffer.CreateMessage();
            
            using var reader = oldMsg.GetReaderAtBodyContents();
            var xmlString = reader.ReadOuterXml();
            var bodyElement = XElement.Parse(xmlString);

            if (bodyElement.Element(bodyElement.Name.Namespace + "sessionToken") == null)
            {
                bodyElement.AddFirst(new XElement(bodyElement.Name.Namespace + "sessionToken", token));
            }

            var newMsg = Message.CreateMessage(MessageVersion.Soap11, request.Headers.Action, new XElementBodyWriter(bodyElement));
            for (int i = 0; i < request.Headers.Count; i++)
            {
                if (request.Headers[i].Name != "Action")
                {
                    newMsg.Headers.CopyHeaderFrom(request, i);
                }
            }
            newMsg.Properties.CopyProperties(request.Properties);
            request = newMsg;

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
        public void Validate(ServiceEndpoint endpoint) { }
    }

    public class XElementBodyWriter : BodyWriter
    {
        private readonly XElement _element;
        public XElementBodyWriter(XElement element) : base(isBuffered: true)
        {
            _element = element;
        }
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            _element.WriteTo(writer);
        }
    }
}
