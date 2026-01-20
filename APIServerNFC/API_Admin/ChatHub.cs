using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
namespace APIServerNFC.API_Admin
{
    public class ChatHub:Hub
    {
        //Xứ lý 1 hàm chung
        public async Task SendMsgJsonHub(string jsonMsg)
        {
            try
            {
                JsonMsgAndroid jsonMsgAndroid = JsonConvert.DeserializeObject<JsonMsgAndroid>(jsonMsg);
                if (string.IsNullOrEmpty(jsonMsgAndroid.typemsg))
                {
                    jsonMsgAndroid.message = "Vui lòng nhập loại tin nhắn";
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                    return;
                }
                bool check = false;
                jsonMsgAndroid.clientid = Context.ConnectionId;
                switch (jsonMsgAndroid.typemsg)
                {
                    case "sendmsgall":
                       
                        await SendMessageAll(jsonMsgAndroid);
                        check = true;
                        break;
                    case "sendmsgtogroup":
                        await SendMessageToGroup(jsonMsgAndroid);
                        check = true;
                        break;
                    case "sendmsgtogroupexceptme":
                        await SendMessageToGroupExceptMe(jsonMsgAndroid);
                        check = true;
                        break;
                    case "sendmsgtoclient":

                        await SendMsgToClient(jsonMsgAndroid);
                        check = true;
                        break;
                    case "getconnectionid":
                        await SendConnectionId(jsonMsgAndroid);
                        check = true;
                        break;
                    case "joingroup":
                        await JoinGroup(jsonMsgAndroid);
                        check = true;
                        break;
                    case "joingrouplist":
                        await JoinGroupList(jsonMsgAndroid);
                        check = true;
                        break;
                    case "leavegroup":
                        await LeaveGroup(jsonMsgAndroid);
                        check = true;
                        break;
                    
                }
                if (check == false)
                {
                    jsonMsgAndroid.message = "Loại tin nhắn không đúng";
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                    return;
                }
            }
            catch (Exception ex)
            {
                JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
                jsonMsgAndroid.clientid = Context.ConnectionId;
                jsonMsgAndroid.message = "Đã có lỗi xảy ra: " + ex.Message;
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                return;
            }
        }
        public async Task SendConnectionId(JsonMsgAndroid jsonMsgAndroid)
        {
            jsonMsgAndroid.message = Context.ConnectionId;
            jsonMsgAndroid.clientid= Context.ConnectionId;
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
            // Gửi Connection ID về cho client
            //return Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
        }
        public async Task SendMessageAll(JsonMsgAndroid jsonMsgAndroid)
        {
            // Gửi tin nhắn tới tất cả các client đã kết nối
            await Clients.All.SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
        }
       
        public async Task JoinGroup(JsonMsgAndroid jsonMsgAndroid)
        {
            // Thêm người dùng vào nhóm
            await Groups.AddToGroupAsync(Context.ConnectionId, jsonMsgAndroid.topic);
            jsonMsgAndroid.message = $"Kết nối với nhóm {jsonMsgAndroid.topic} thành công";
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
        }
        public async Task JoinGroupList(JsonMsgAndroid jsonMsgAndroid)
        {
            // Thêm người dùng vào nhóm
            //Topic cách nhau bởi dấu ;
            string[] topics = jsonMsgAndroid.topic.Split(';');
            bool checkall = true;
            foreach (string topic in topics)
            {
                try
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, topic);
                }
                catch(Exception ex)
                {
                    checkall = false;
                    jsonMsgAndroid.message = $"Kết nối với nhóm {jsonMsgAndroid.topic} không thành công";
                }
            }
            if(checkall==true)
                jsonMsgAndroid.message = $"Kết nối với nhóm {jsonMsgAndroid.topic} thành công";
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
        }
        //public async Task SendMessageToGroup(string groupName, string user, string message)
        //{
        //    // Gửi tin nhắn đến nhóm
        //    await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        //}

        // Xóa người dùng khỏi nhóm
        public async Task LeaveGroup(JsonMsgAndroid jsonMsgAndroid)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, jsonMsgAndroid.topic);
            jsonMsgAndroid.message= $"{jsonMsgAndroid.clientid} đã rời nhóm {jsonMsgAndroid.topic}";
            await Clients.Group(jsonMsgAndroid.topic).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
        }
        public async Task SendMessageToGroup(JsonMsgAndroid jsonMsgAndroid)
        {
            await Clients.Group(jsonMsgAndroid.topic).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());

        }
        public async Task SendMessageToGroupExceptMe(JsonMsgAndroid jsonMsgAndroid)
        {
            await Clients.OthersInGroup(jsonMsgAndroid.topic).SendAsync("ReceiveMessage",jsonMsgAndroid.ToJson());
           
        }
        public async Task SendMsgToClient(JsonMsgAndroid jsonMsgAndroid)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
        }
       

    }
}
