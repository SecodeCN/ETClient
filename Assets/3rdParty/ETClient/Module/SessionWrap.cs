using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
    public class SessionWrap : Entity
    {
        public readonly Session session;

        private static int RpcId { get; set; }
        private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();

        public SessionWrap(Session session)
        {
            this.session = session;
            SessionCallbackComponent sessionComponent = this.session.AddComponent<SessionCallbackComponent>();
            sessionComponent.MessageCallback = (s, f, o, io) => { this.Run(s, f, o, io); };
            sessionComponent.DisposeCallback = s => { this.Dispose(); };
        }

        public void Run(Session s, byte flag, ushort opcode, System.IO.MemoryStream memoryStream)
        {
            OpcodeTypeComponent opcodeTypeComponent = Game.Scene.GetComponent<OpcodeTypeComponent>();
            Type responseType = opcodeTypeComponent.GetType(opcode);

            object message;
            try
            {
                object instance = opcodeTypeComponent.GetInstance(opcode);
                message = s.Network.MessagePacker.DeserializeFrom(instance, memoryStream);
                Log.Debug($"recv: {JsonHelper.ToJson(message)}");
            }
            catch (Exception e)
            {
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                Log.Error($"opcode: {opcode} {s.Network.Count} {e} ");
                return;
            }

            if ((flag & 0x01) > 0)
            {
                IResponse response = message as IResponse;
                if (response == null)
                {
                    throw new Exception($"flag is response, but hotfix message is not! {opcode}");
                }

                Action<IResponse> action;
                if (!this.requestCallback.TryGetValue(response.RpcId, out action))
                {
                    return;
                }
                this.requestCallback.Remove(response.RpcId);

                action(response);
                return;
            }

            Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, message));
        }

        public void Send(IMessage message)
        {
            Send(0x00, message);
        }

        public void Send(byte flag, IMessage message)
        {
            ushort opcode = Game.Scene.GetComponent<OpcodeTypeComponent>().GetOpcode(message.GetType());
            session.Send(flag, opcode, message);
        }
        
        public Task<IResponse> Call(IRequest request)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (response.Error > ErrorCode.ERR_Exception)
                    {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };

            request.RpcId = rpcId;

            this.Send(0x00, request);
            return tcs.Task;
        }

        public Task<IResponse> Call(IRequest request, CancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (response.Error > ErrorCode.ERR_Exception)
                    {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };

            cancellationToken.Register(() => { this.requestCallback.Remove(rpcId); });

            request.RpcId = rpcId;

            this.Send(0x00, request);
            return tcs.Task;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
        }
    }
}