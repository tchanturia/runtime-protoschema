extern alias pbnr;
using nFileDescriptorSet = pbnr::Google.Protobuf.Reflection.FileDescriptorSet;
using Google.Cloud.BigQuery.Storage.V1;
using Google.Protobuf;
using System.IO;
using System.Linq;
using Google.Protobuf.Reflection;
using ProtoBuf;
using static Google.Cloud.BigQuery.Storage.V1.AppendRowsRequest.Types;
namespace ConsoleApp20
{
    class Program
    {
        static void Main(string[] args)
        {
            //protobuff-net stuff

            // generate .proto into string
            var protoDefinition = Serializer.GetProto<Yolo>();
            
            // compile .proto with protobuf-net instead of protoc and put into memory stream
            var nFileDescriptorSet = new nFileDescriptorSet();
            nFileDescriptorSet.Add("yolo.proto", source: new StringReader(protoDefinition));
            nFileDescriptorSet.Process();
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, nFileDescriptorSet);
            ms.Seek(0, SeekOrigin.Begin);

            // actual google stuff

            // read compiled .proto and put into descriptor
            var fileDescriptionSet = FileDescriptorSet.Parser.ParseFrom(ms);
            var byteStrings = fileDescriptionSet.File.Select(f => f.ToByteString()).ToArray();
            var fileDescriptor = FileDescriptor.BuildFromByteStrings(byteStrings).First();

            // yolo
            var protoData = new ProtoData
            {
                WriterSchema = new ProtoSchema
                {
                    ProtoDescriptor = fileDescriptor.MessageTypes[0].ToProto()
                }
            };

        }
    }

    [ProtoContract]
    public class Yolo
    {
        [ProtoMember(1)]
        public int Hey;

    }
}
