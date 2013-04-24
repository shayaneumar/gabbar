/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/

using System;
using System.Net;
using System.Net.Sockets;
using CookComputing.XmlRpc;
using JohnsonControls.Exceptions;

namespace JohnsonControls.XmlRpc
{
    /// <summary>
    /// A proxy class for invoking methods from <typeparam name="T"></typeparam>
    /// via Xml Rpc. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class P2000XmlRpcProxy<T>
    {
        private readonly T _serviceProxy;

        public P2000XmlRpcProxy(Uri serviceUrl)
        {
            if (serviceUrl == null) throw new ArgumentNullException("serviceUrl");

            _serviceProxy = (T)XmlRpcProxyGen.Create(typeof(T));
            ((IXmlRpcProxy)_serviceProxy).Url = serviceUrl.AbsoluteUri;
        }

        /// <summary>
        /// Invokes the specified action. The action is given an object of
        /// type T (an internal proxy object) and it can then call any 
        /// service defined by methods on T.
        /// </summary>
        /// <remarks>
        /// An example call assuming <typeparam name="T"></typeparam>
        /// has a method named <code>SendFaultCode(int)</code>
        /// <code>
        /// Invoke(proxy => proxy.SendFaultCode(3));
        /// </code>
        /// </remarks>
        /// <param name="xmlRpcAction">An action on T that you wish to invoke</param>
        public void Invoke(Action<T> xmlRpcAction)
        {
            Invoke(proxy =>
                       {
                           xmlRpcAction(proxy);
                           return 1;
                       });
        }

        /// <summary>
        /// Invokes the specified function. The function is given a proxy
        /// object of type T and it can then call any service defined
        /// by methods on T and return the respective TResult.
        /// </summary>
        /// An example call if T has a method call "int GetStatusCode(int)":
        /// <code>
        /// return Invoke(proxy => proxy.GetStatusCode(2));
        /// </code>
        /// <returns></returns>
        /// <exception cref="ServiceOperationException">if the proxy method returns a fault code or if there is a web exception.</exception>
        public TResult Invoke<TResult>(Func<T, TResult> xmlRpcMethod)
        {
            if (xmlRpcMethod == null)
            {
                throw new ArgumentNullException("xmlRpcMethod");
            }

            try
            {
                return xmlRpcMethod(_serviceProxy);
            }
            catch (XmlRpcFaultException e)
            {
                if (e.FaultCode == XmlRpcFaultCodes.InvalidSessionInformation)
                {
                    throw new AuthenticationRequiredException();
                }

                throw new ServiceOperationException(e.FaultString, e, e.FaultCode);
            }
            catch (WebException e)
            {
                var se = e.InnerException as SocketException;
                if (se != null && se.SocketErrorCode == SocketError.TimedOut)
                {
                    throw new ServiceTimedOutException(e.Message, e);
                }
                throw new ServiceOperationException(e.Message, e);
            }
        }
    }
}