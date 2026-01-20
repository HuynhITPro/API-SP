using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServerNFC.API_Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        
       
        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        //Viết 1 API xử lý chung
        [HttpPost("SendMsgJson")]
        public async Task<IActionResult> SendMsgJson([FromBody] JsonMsgAndroid jsonMsgAndroid)
        {
            if(string.IsNullOrEmpty(jsonMsgAndroid.typemsg))
            {
                return Ok("Vui lòng nhập loại tin nhắn");
            }
            bool check = false;
            try
            {
                switch (jsonMsgAndroid.typemsg)
                {
                    case "sendmsgall":
                        await _hubContext.Clients.All.SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                        jsonMsgAndroid.message = "Gửi tin nhắn thành công";
                        check = true;
                        break;
                    case "sendmsgtogroup":
                        await _hubContext.Clients.Group(jsonMsgAndroid.topic).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                        jsonMsgAndroid.message = "Gửi tin nhắn thành công";
                        check = true;
                        break;
                    case "sendmsgtogroupexceptme":
                        await _hubContext.Clients.GroupExcept(jsonMsgAndroid.topic, jsonMsgAndroid.clientid).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                        jsonMsgAndroid.message = "Gửi tin nhắn thành công";
                        check = true;
                        break;
                    case "sendmsgtoclient":
                        await _hubContext.Clients.Client(jsonMsgAndroid.clientid).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
                        jsonMsgAndroid.message = "Gửi tin nhắn thành công";
                        check = true;
                        break;
                    case "joingroup":
                        await _hubContext.Groups.AddToGroupAsync(jsonMsgAndroid.clientid, jsonMsgAndroid.topic);

                        jsonMsgAndroid.message = $"Kết nối nhóm {jsonMsgAndroid.topic} thành công";
                        check = true;
                        break;
                    case "joingrouplist"://topic cách nhau dấu ;
                        string[] groupNames = jsonMsgAndroid.topic.Split(';');
                        foreach (var groupName in groupNames)
                        {
                            try
                            {
                                await _hubContext.Groups.AddToGroupAsync(jsonMsgAndroid.clientid, groupName);
                            }
                            catch (Exception ex)
                            {
                                // Ghi lại lỗi nếu cần
                                // Console.WriteLine($"Failed to add connection {jsonMsgAndroid.clientid} to group {groupName}: {ex.Message}");
                            }
                        }
                        jsonMsgAndroid.message = $"Kết nối nhóm {jsonMsgAndroid.topic} thành công";
                        check = true;
                        break;
                    case "leavegroup":
                        await _hubContext.Groups.RemoveFromGroupAsync(jsonMsgAndroid.clientid, jsonMsgAndroid.topic);
                        jsonMsgAndroid.message = $"Đã rời nhóm {jsonMsgAndroid.topic}";
                        check = true;
                        break;
                    case "disconnect":
                        await _hubContext.Clients.Client(jsonMsgAndroid.clientid).SendAsync("ForceDisconnect");
                        jsonMsgAndroid.message = $"Đã hủy kết nối {jsonMsgAndroid.message}";
                        check = true;
                        break;
                }
                if (!check)
                    return Ok("typemsg không đúng định dạng");
            }
            catch (Exception ex)
            { return BadRequest(ex); }
            return Ok(jsonMsgAndroid.message);
        }

        

        // API để gửi tin nhắn đến group ngoại trừ người gửi
        // API gửi tin nhắn đến tất cả các client
        [HttpGet("sendmsgall")]
        public async Task<IActionResult> SendMessage(string msg)
        {
            // Gửi tin nhắn đến tất cả các client kết nối với ChatHub
            await _hubContext.Clients.All.SendAsync("ReceiveMessage",  msg);
            return Ok("Message sent.");
        }
        [HttpPost("joingroup")]
        public async Task<IActionResult> JoinGroup([FromBody] JsonMsgAndroid jsonMsgAndroid)
        {
            // Thêm vào nhóm
            await _hubContext.Groups.AddToGroupAsync(jsonMsgAndroid.clientid, jsonMsgAndroid.topic);
            jsonMsgAndroid.message = $"Tham gia nhóm :{jsonMsgAndroid.topic} thành công";
            return Ok(jsonMsgAndroid.message);
        }
        [HttpPost("leavegroup")]
        public async Task<IActionResult> LeaveGroup([FromBody] JsonMsgAndroid jsonMsgAndroid)
        {
            // Thêm vào nhóm
            await _hubContext.Groups.RemoveFromGroupAsync(jsonMsgAndroid.clientid, jsonMsgAndroid.topic);
            jsonMsgAndroid.message=$"Rời nhóm :{jsonMsgAndroid.topic} thành công";
            return Ok(jsonMsgAndroid.message);
        }
        // API gửi tin nhắn đến nhóm
        [HttpPost("sendmsgtogroup")]
        public async Task<IActionResult> SendMessageToGroup([FromBody] JsonMsgAndroid jsonMsgAndroid)
        {
            
           // Gửi tin nhắn đến tất cả các client trong nhóm
           await _hubContext.Clients.Group(jsonMsgAndroid.topic).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());

            return Ok($"Đã gửi đến {jsonMsgAndroid.topic} thành công.");
        }
        // API gửi tin nhắn đến nhóm
        [HttpPost("sendmsgtogroupexceptme")]
        public async Task<IActionResult> SendMessageToGroupExceptMe([FromBody] JsonMsgAndroid jsonMsgAndroid)
        {
            
            // Gửi tin nhắn đến tất cả các client trong nhóm
            await _hubContext.Clients.GroupExcept(jsonMsgAndroid.topic, jsonMsgAndroid.clientid).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
            return Ok($"Đã gửi đến {jsonMsgAndroid.topic} thành công.");
        }

        // API gửi tin nhắn đến một client cụ thể (bằng ConnectionId)
        [HttpPost("sendmsgtoclient")]
        public async Task<IActionResult> SendMessageToClient([FromBody] JsonMsgAndroid jsonMsgAndroid)
        {
            // Gửi tin nhắn đến client có ConnectionId cụ thể
            await _hubContext.Clients.Client(jsonMsgAndroid.clientid).SendAsync("ReceiveMessage", jsonMsgAndroid.ToJson());
            return Ok($"Đã gửi đến {jsonMsgAndroid.clientid} thành công");
        }
       


    }
}
