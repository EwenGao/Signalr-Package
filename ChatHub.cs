using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Core
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// 接收客户端推送过来的消息，通过函数名称来判断回调客户端的什么函数
        /// </summary>
        /// <param name="json">json格式的消息数据</param>
        /// <param name="methodName">调用的前端JS函数名称</param>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("SendMessage")]
        public void SendMessage(string json, string methodName, string groupName)
        {
            IClientProxy client = Clients.Group(groupName);
            client.Invoke(methodName, json);

        }
        /// <summary>
        /// 推送的消息只有自己可以看
        /// </summary>
        /// <param name="json">json数据</param>
        [HubMethodName("SendToYourself")]
        public void SendToYourself(string json)
        {
            Clients.Client(Context.ConnectionId).ReceiveYourself(json);
        }

        /// <summary>
        /// 接收客户端审核推送的消息
        /// </summary>
        /// <param name="html">html格式的数据</param>
        /// <param name="msgId">被审核消息的编号</param>
        /// <param name="methodName">调用的前端JS函数名称</param>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("CheckMessage")]
        public void CheckMessage(string html, string msgId,string methodName, string groupName)
        {
            //调用客户端审核通过的js函数
            IClientProxy client = Clients.Group(groupName);
            client.Invoke(methodName, html, msgId);
        }
        /// <summary>
        /// 清除客户端的消息
        /// </summary>
        /// <param name="msgId">被清除的消息的消息编号</param>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("DeleteMessage")]
        public void DeleteMessage(string msgId, string groupName)
        {
            //调用客户端清除的js函数
            Clients.Group(groupName).DeletedMessage(msgId);
        }

        [HubMethodName("AddGroup")]
        public void AddGroup(string connectionId, string groupName)
        {
            Groups.Add(connectionId, groupName);
        }

        //[HubMethodName("CheckGroupMessage")]
        //public void CheckGroupMessage(string html, string msgId, string groupName)
        //{
        //    Clients.Group(groupName).CheckedGroupMessage(html, msgId);
        //}

        /// <summary>
        /// 客户端页面加载成功后，主动链接Signalr，分组字符串如果为空字符串，分组无效
        /// </summary>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("Connect")]
        public void Connect(string groupName)
        {
            //将分组字符串加入Signalr分组
            Groups.Add(Context.ConnectionId, groupName);
            //把connectionId推到客户端，以便后面使用
            Clients.Client(Context.ConnectionId).Connected(Context.ConnectionId);
            Clients.Group(groupName).Connectting();
        }
    }
}
