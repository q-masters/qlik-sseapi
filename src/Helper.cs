namespace Qlik.Sse
{
    #region Usings
    using Google.Protobuf;
    using Grpc.Core;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    #endregion

    #region ProtobufHelper
    /// <summary>
    /// 
    /// </summary>
    public static class ProtobufHelper
    {
        private static readonly ConcurrentDictionary<Type, Tuple<string, MessageParser>> parserDic = new ConcurrentDictionary<Type, Tuple<string, MessageParser>>();

        /// <summary>
        /// Extension Method to easy parsing of Qlik Header Messages. If one Message of
        /// the requested type is found. This is returned.
        /// </summary>
        /// <typeparam name="T">The type of Result</typeparam>
        /// <param name="md">This for Extending the Metadata Class</param>
        /// <returns>Returns the parsed Message.</returns>
        public static T ParseIMessageFirstOrDefault<T>(this Metadata md) where T : class, Google.Protobuf.IMessage<T>, new()
        {
            var type = typeof(T);
            Tuple<string, MessageParser> parser = null;
            if (parserDic.ContainsKey(type))
            {
                parser = parserDic[type];
            }
            else
            {
                parser = new Tuple<string, MessageParser>($"qlik-{type.Name.ToLowerInvariant()}-bin", new MessageParser<T>(() => new T()));
                parserDic.TryAdd(type, parser);
            }

            foreach (var item in md)
            {
                if (item.Key == parser?.Item1)
                {
                    return parser?.Item2?.ParseFrom(item.ValueBytes) as T;
                }
            }

            return default;
        }

        /// <summary>
        /// Extension Method to easy parsing of Qlik Header Messages
        /// </summary>
        /// <typeparam name="T">The type of Result</typeparam>
        /// <param name="md">This for Extending the Metadata Class</param>
        /// <returns>Returns the parsed Message.</returns>
        public static IEnumerable<T> ParseIMessages<T>(this Metadata md) where T : class, Google.Protobuf.IMessage<T>, new()
        {
            var type = typeof(T);
            Tuple<string, MessageParser> parser = null;
            if (parserDic.ContainsKey(type))
            {
                parser = parserDic[type];
            }
            else
            {
                parser = new Tuple<string, MessageParser>($"qlik-{type.Name.ToLowerInvariant()}-bin", new MessageParser<T>(() => new T()));
                parserDic.TryAdd(type, parser);
            }

            foreach (var item in md)
            {
                if (item.Key == parser?.Item1)
                {
                    yield return parser?.Item2?.ParseFrom(item.ValueBytes) as T;
                }
            }

            yield return default;
        }

    }
    #endregion
}
