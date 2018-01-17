using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remote.Infrastructure.Interfaces;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class ClientTokenList : List<ClientToken>
    {
        /// <summary>
        /// 覆盖List方法
        /// </summary>
        /// <param name="userClient"></param>
        public ClientToken AddNew(ClientToken userClient)
        {
            ClientToken clientCopy = new ClientToken();//从远程端传入的不调用构造函数，初始化有问题，重新copy属性再加入，服务端与服务端对齐即可
            ClientToken.PropertyCopy(userClient, clientCopy);
            base.Add(clientCopy);
            return clientCopy;
        }

        public static ClientToken Find(List<ClientToken> tokenList, IClientToken other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("不能传入空");
            }
            var find = tokenList.Find((item) =>
            {
                try
                {
                    var b1 = item.Address.Equals(other.Address);
                    var b2 = item.ServicePort == other.ServicePort;
                    return b1 && b2;
                }
                catch (Exception e)
                {
                    return false;
                }
            });
            return find;
        }

        /// <summary>
        /// 添加List新方法，查找IUserClient类型
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ClientToken Find(IClientToken other)
        {
            //if (other == null)
            //{
            //    throw new ArgumentNullException("不能传入空");
            //}
            //var find = base.Find((item) =>
            //{
            //    try
            //    {
            //        var b1 = item.Address.Equals(other.Address) ;
            //        var b2 = item.ServicePort == other.ServicePort;
            //        return b1 && b2;
            //    }
            //    catch (Exception e)
            //    {
            //        return false;
            //    }
            //});

            return ClientTokenList.Find(this,other);
        }
    }
}
