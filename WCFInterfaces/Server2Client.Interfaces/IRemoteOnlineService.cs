using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Remote.Infrastructure.DataContracts;

namespace Server2Client.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IPushServiceCallback))]
    public interface IRemoteOnlineService
    {
        /// <summary>
        /// 获取已连接服务器的User
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ClientToken> GetOnlineUsers(ClientToken whoIam);

        /// <summary>
        /// 提交身份识别信息
        /// </summary>
        /// <param name="whoIam"></param>
        [OperationContract(IsOneWay = true)]
        void UpdateWhoIam(ClientToken whoIam);

        /// <summary>
        /// 更新在线状态
        /// </summary>
        /// <param name="token"></param>
        [OperationContract(IsOneWay = true)]
        void UpdateTokenOnlineState(ClientToken token);

        /// <summary>
        /// 获取已存在组名称
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<string> GetExistGroupName(ClientToken whoIam);

        /// <summary>
        /// 尝试连接到另一个客户端,测试用
        /// </summary>
        /// <param name="whoIam"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [OperationContract]
        bool TryConnectToAnotherClient(ClientToken target);
        
        /// <summary>
        /// 开始连接到另一个客户端,通过该方式，将会阻止同一个主机多个Token的发送,但会比指定Token方式更快速
        /// </summary>
        /// <param name="target"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        [OperationContract]
        bool BeginTranslateFile(ClientToken target);

        /// <summary>
        /// 向指定端口的proxy连接发送
        /// </summary>
        /// <param name="prot"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        FileTransferResponsed UpdateFileData(int taskID, FileTransferRequest request);


        [OperationContract]
        BlockTransferResponsed UpdateFileBlockData(int taskID, BlockTransferRequest request);

        /// <summary>
        /// 结束连接
        /// </summary>
        /// <param name="target"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        [OperationContract]
        bool EndTranslateFile(ClientToken target);
    }

    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    // 可以将 XSD 文件添加到项目中。在生成项目后，可以通过命名空间“FileTransfer.Interfaces.ContractType”直接使用其中定义的数据类型。
    }
