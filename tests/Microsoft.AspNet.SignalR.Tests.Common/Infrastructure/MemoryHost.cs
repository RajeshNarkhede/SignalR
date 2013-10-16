﻿using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.Owin.Testing;
using Owin;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hosting.Memory
{
    public class MemoryHost : DefaultHttpClient, IDisposable
    {
        private static int instanceId;
        private TestServer _host;
        private bool _disposed;

        public string InstanceName { get; set; }        

        public MemoryHost()
        {
            var id = Interlocked.Increment(ref instanceId);
            InstanceName = Process.GetCurrentProcess().ProcessName + id;
        }

        public void Configure(Action<IAppBuilder> startup)
        {
            _host = TestServer.Create(app => {
                app.Use((context, next) =>
                {
                    if (_disposed)
                    {
                        context.Response.StatusCode = 503;
                        return Task.FromResult<object>(null);
                    }
                    
                    return next.Invoke();
                });

                startup(app);
            });
        }


        protected override HttpMessageHandler CreateHandler()
        {
            return _host.Handler;
        }

        public void Dispose()
        {
            _disposed = true;
            _host.Dispose();
        }        
    }
}
