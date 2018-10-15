using Google.Protobuf;

namespace ETModel
{
    public interface IMessage : Google.Protobuf.IMessage
    { }

    public interface IRequest : IMessage
    {
        int RpcId { get; set; }
    }

    public interface IResponse : IMessage
    {
        int Error { get; set; }
        string Message { get; set; }
        int RpcId { get; set; }
    }

    public class ResponseMessage : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }

        public int CalculateSize()
        {
            return 0;
        }

        public void MergeFrom(CodedInputStream input)
        {
        }

        public void WriteTo(CodedOutputStream output)
        {
        }
    }
}