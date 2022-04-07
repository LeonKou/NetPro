/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using NetMQ.Sockets;
using System;

namespace NetPro.ZeroMQ
{
    /// <summary>
    /// ZeroMQ
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static class ZeroMQServiceExtensions
    {
        /// <summary>
        /// ZeroMQForPublisher
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddZeroMQForPublisher(this IServiceCollection services, int port = 81, SocketOptions socketOptions = null)
        {
            PublisherSocket publisher = new();
            //发布是由于本机承载故配回环地址即可
            //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
            publisher.Bind($"tcp://*:{port}");

            services.AddSingleton(publisher);
            return services;
        }

        /// <summary>
        /// ZeroMQForPublisher
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddZeroMQForPublisher(this IServiceCollection services, ZeroMQOption zeroMQOption, SocketOptions socketOptions = null)
        {
            PublisherSocket publisher = new();
            //发布是由于本机承载故配回环地址即可
            //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
            //TODO
            //publisher.Options.SendHighWatermark = 1000;
            publisher.Bind($"tcp://*:{zeroMQOption.PublishPort}");

            services.AddSingleton(publisher);
            return services;
        }

        /// <summary>
        /// ZeroMQForPushSocket
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddZeroMQForPushSocket(this IServiceCollection services, int port = 82, SocketOptions socketOptions = null)
        {
            PushSocket pushSocket = new();
            //发布是由于本机承载故配回环地址即可
            //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
            pushSocket.Bind($"tcp://*:{port}");

            services.AddSingleton(pushSocket);
            return services;
        }

        /// <summary>
        /// ZeroMQForPushSocket
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddZeroMQForPushSocket(this IServiceCollection services, ZeroMQOption zeroMQOption, SocketOptions socketOptions = null)
        {
            PushSocket pushSocket = new();
            //发布是由于本机承载故配回环地址即可
            //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
            //publisher.Options.SendHighWatermark = 1000;
            pushSocket.Bind($"tcp://*:{zeroMQOption.PushPort}");

            services.AddSingleton(pushSocket);
            return services;
        }

        /// <summary>
        /// AddZeroMQForResponseSocket
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddZeroMQForResponseSocket(this IServiceCollection services, ZeroMQOption zeroMQOption, SocketOptions socketOptions = null)
        {
            ResponseSocket responseSocket = new();
            responseSocket.Bind($"tcp://*:{zeroMQOption.ResponsePort}");

            services.AddSingleton(responseSocket);
            return services;
        }
    }
}
