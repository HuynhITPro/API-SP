using APIServerNFC.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServerNFC.API_Admin
{
    [ApiController]
    [Route("api/fcm")]
    public class FcmController : ControllerBase
    {
        private readonly FcmSender _fcmSender;

        public FcmController(FcmSender fcmSender)
        {
            _fcmSender = fcmSender;
        }

        // Client gọi API này để đăng ký token vào topic
        [HttpPost("register-topic")]
        public async Task<IActionResult> RegisterTopic([FromBody] TopicNotificationModel model)
        {
            await _fcmSender.SubscribeTokenToTopicAsync(model.Token, model.Topic);
            return Ok(new { success = true });
        }

        // Gửi notification tới topic
        [HttpPost("send-topic")]
        public async Task<IActionResult> SendTopicNotification([FromBody] TopicNotificationModel model)
        {
            string json=  await _fcmSender.SendNotificationToTopicAsync(model.Topic, model.Title, model.Body,model.Link);
           //string jsonoutput = JsonConvert.SerializeObject(model);
            //return Content(jsonoutput);
            return Content(json);
        }
        [HttpPost("send-listtopic")]
        public async Task<IActionResult> SendListTopicNotification([FromBody] TopicNotificationModel model)
        {
            List<string> lsttoken = JsonConvert.DeserializeObject<List<string>>(model.TokenJson);//Truyền vào danh sách topic
            foreach(var it in lsttoken)
            {
                _= _fcmSender.SendNotificationToTopicAsync(it, model.Title, model.Body,model.Link);
            }
            
            //string jsonoutput = JsonConvert.SerializeObject(model);
            //return Content(jsonoutput);
            return Content("OK");
        }
        [HttpPost("send-token")]
        public async Task<IActionResult> SendTokenNotification([FromBody] TopicNotificationModel model)
        {
           List<string>lsttoken= JsonConvert.DeserializeObject<List<string>>(model.TokenJson);
            string json = await _fcmSender.SendDataMessageToTokensAsync(lsttoken,model.Title, model.Body.ToString(),model.Link);
          
            return Content(json);
        }
    }

    // Model
  
    
    public class TopicNotificationModel
    {
        public string TokenJson { get; set; }
        public string Token { get; set; }
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
    }
}
