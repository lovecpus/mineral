using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sky.Core;
using System.Collections.Generic;
using Sky.Network.RPC.Command;

namespace Sky.Network.RPC
{
    public class RpcServer : IDisposable
    {
        protected readonly LocalNode _localNode;
        IWebHost _host;

        protected Dictionary<string, RpcCommand.ProcessHandler> processHandlers = new Dictionary<string, RpcCommand.ProcessHandler>()
        {
            // Block
            { RpcCommand.Block.GetBlock, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetBlock) },
            { RpcCommand.Block.GetBlocks, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetBlocks) },
            { RpcCommand.Block.GetBlockHash, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetBlockHash) },
            { RpcCommand.Block.GetHeight, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetHeight) },
            { RpcCommand.Block.GetCurrentBlockHash, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetCurrentBlockHash) },
            { RpcCommand.Block.GetTransaction, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetTransaction) },

            // Node
            { RpcCommand.Node.NodeList, new RpcCommand.ProcessHandler(RpcProcessCommand.OnNodeList) },

            // Wallet
            { RpcCommand.Wallet.GetAccount, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetAccount) },
            { RpcCommand.Wallet.GetAddress, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetAddress) },
            { RpcCommand.Wallet.GetBalance, new RpcCommand.ProcessHandler(RpcProcessCommand.OnGetBalance) },
            { RpcCommand.Wallet.SendTo, new RpcCommand.ProcessHandler(RpcProcessCommand.OnSendTo) },
            { RpcCommand.Wallet.FreezeBalance, new RpcCommand.ProcessHandler(RpcProcessCommand.OnFreezeBalance) },
            { RpcCommand.Wallet.UnfreezeBalance, new RpcCommand.ProcessHandler(RpcProcessCommand.OnUnfreezeBalance) },
            { RpcCommand.Wallet.VoteWitness, new RpcCommand.ProcessHandler(RpcProcessCommand.OnVoteWitness) },
        };

        public RpcServer(LocalNode localNode)
        {
            _localNode = localNode;
        }

        static JObject CreateErrorResponse(JToken id, int code, string message, string data = null)
        {
            JObject response = CreateResponse(id);
            response["error"] = new JObject();
            response["error"]["code"] = code;
            response["error"]["message"] = message;
            if (data != null)
                response["error"]["data"] = data;
            return response;
        }

        static JObject CreateResponse(JToken id)
        {
            JObject response = new JObject();
            response["jsonrpc"] = "2.0";
            response["id"] = id;
            return response;
        }

        public void Dispose()
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }
        }

        protected virtual JObject Process(JToken id, string method, JArray parameters)
        {
            return processHandlers.ContainsKey(method) 
                ? processHandlers[method](_localNode, parameters) : CreateErrorResponse(id, -1, string.Format("Not found method : {0}", method));
        }

        async Task ProcessAsync(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";
            if (context.Request.Method != "GET" && context.Request.Method != "POST")
                return;

            JObject request = null;
            if (context.Request.Method == "GET")
            {
                string jsonrpc = context.Request.Query["jsonrpc"];
                string id = context.Request.Query["id"];
                string method = context.Request.Query["method"];
                string parameters = context.Request.Query["params"];
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(method) && !string.IsNullOrEmpty(parameters))
                {
                    try
                    {
                        parameters = Encoding.UTF8.GetString(Convert.FromBase64String(parameters));
                    }
                    catch (FormatException) { }
                    request = new JObject();
                    if (!string.IsNullOrEmpty(jsonrpc))
                        request["jsonrpc"] = jsonrpc;
                    request["id"] = double.Parse(id);
                    request["method"] = method;
                    request["params"] = JArray.Parse(parameters);
                }
            }
            else if (context.Request.Method == "POST")
            {
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    try
                    {
                        request = JObject.Parse(reader.ReadToEnd());
                    }
                    catch (FormatException) { }
                }
            }
            JObject response;
            if (request == null)
            {
                response = CreateErrorResponse(null, -32700, "Parse error");
            }
            else
            {
                response = ProcessRequest(context, request);
            }
            if (response == null)
                return;
            context.Response.ContentType = "application/json-rpc";
            await context.Response.WriteAsync(response.ToString(), Encoding.UTF8);
        }

        JObject ProcessRequest(HttpContext context, JObject request)
        {
            if (!request.ContainsKey("id"))
                return null;
            if (!request.ContainsKey("method") || !request.ContainsKey("params") || !(request["params"] is JArray))
                return CreateErrorResponse(request["id"], -32600, "Invalid Request");
            JObject result = null;
            try
            {
                JToken id = request["id"];
                string method = request["method"].Value<string>();
                JArray parameters = (JArray)request["params"];
                result = Process(id, method, parameters);
            }
            catch (Exception e)
            {
#if DEBUG
                return CreateErrorResponse(request["id"], e.HResult, e.Message, e.StackTrace);
#else
                return CreateErrorResponse(request["id"], e.HResult, e.Message);
#endif
            }
            JObject response = CreateResponse(request["id"]);
            response["result"] = result;
            return response;
        }

        public void Start(ushort port, string sslCert = null, string password = null)
        {
            _host = new WebHostBuilder().UseKestrel(options => options.Listen(IPAddress.Any, port, listenOptions =>
            {
                if (!string.IsNullOrEmpty(sslCert))
                    listenOptions.UseHttps(sslCert, password);
            }))
            .Configure(app =>
            {
                app.UseResponseCompression();
                app.Run(ProcessAsync);
            })
            .ConfigureServices(services =>
            {
                services.AddResponseCompression(options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json-rpc" });
                });

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });
            })
            .Build();
            _host.Start();
        }
    }
}
