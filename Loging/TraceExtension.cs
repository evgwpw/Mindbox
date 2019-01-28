using System;
using System.IO;
using System.Text;
using System.Web.Services.Protocols;
using Core.SRD;

namespace Core.Diagnostics
{
    public class TraceExtension : SoapExtension
    {
        Stream _oldStream;
        Stream _newStream;
        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {

        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    break;
                case SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
                default:
                    throw new Exception("invalid stage");
            }
        }

        public override Stream ChainStream(Stream stream)
        {
            _oldStream = stream;
            _newStream = new MemoryStream();
            return _newStream;
        }

        private void WriteOutput(SoapMessage message)
        {
            _newStream.Position = 0;
            var ms = new MemoryStream();
            var w = new StreamWriter(ms);
            var soapString = (message is SoapServerMessage) ? "SoapResponse" : "SoapRequest";
            w.WriteLine("-----" + soapString + " at " + DateTime.Now);
            w.Flush();
            Copy(_newStream, ms);
            //  w.Close();
            _newStream.Position = 0;
            Copy(_newStream, _oldStream);
            WriteToDb(ms, soapString);
        }

        void WriteInput(SoapMessage message)
        {
            Copy(_oldStream, _newStream);
            var ms = new MemoryStream();
            var w = new StreamWriter(ms);

            var soapString = (message is SoapServerMessage) ? "SoapRequest" : "SoapResponse";
            w.WriteLine("-----" + soapString + " at " + DateTime.Now);
            w.Flush();
            _newStream.Position = 0;
            Copy(_newStream, ms);
            //  w.Close();
            _newStream.Position = 0;
            WriteToDb(ms, soapString);
        }

        private static void WriteToDb(Stream stream, string soapString)
        {
            stream.Position = 0;
            var sb = new StringBuilder();
            var sr = new StreamReader(stream);
            var sw = new StringWriter(sb);
            sw.WriteLine(sr.ReadToEnd());
            DiagnosticsManager.Trace("Inec.StateMachine.WSReferences.Service", "GetApplications", soapString, sb.ToString(), null, SRDSession.GetCurrentUserId());
            stream.Close();
        }

        static void Copy(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TraceExtensionAttribute : SoapExtensionAttribute
    {
        public override Type ExtensionType
        {
            get
            {
                return typeof(Core.Diagnostics.TraceExtension);
            }
        }

        public override int Priority { get; set; }
    }
}
