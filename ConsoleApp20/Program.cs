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
            // protobuf-net stuff
            var protoDefinition = Serializer.GetProto<Yolo>();
            var nFileDescriptorSet = new nFileDescriptorSet();
            nFileDescriptorSet.Add("yolo.proto", source: new StringReader(protoDefinition));
            nFileDescriptorSet.Process();
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, nFileDescriptorSet);
            ms.Seek(0, SeekOrigin.Begin);

            // actual google stuff
            var fileDescriptionSet = FileDescriptorSet.Parser.ParseFrom(ms);
            var byteStrings = fileDescriptionSet.File.Select(f => f.ToByteString()).ToArray();
            var fileDescriptor = FileDescriptor.BuildFromByteStrings(byteStrings).First();

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
