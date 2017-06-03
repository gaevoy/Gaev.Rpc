using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Gaev.Rpc.Owin
{
    internal class RpcMiddleware : OwinMiddleware
    {
        private readonly OwinResponder _responder;
        internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

        public RpcMiddleware(OwinMiddleware next, OwinResponder responder) : base(next)
        {
            _responder = responder;
        }

        public override async Task Invoke(IOwinContext ctx)
        {
            if (ctx.Request.Path.Value.StartsWith("/rpc"))
            {
                var requestJson = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject(requestJson, JsonSettings);
                var response = await _responder.Handle(request);
                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(response, JsonSettings));
            }
            else
            {
                await Next.Invoke(ctx);
            }
        }
    }
}