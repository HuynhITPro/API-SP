namespace APIServerNFC.Controllers
{
    using System.Net.Http;
    using System;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Google.Apis.Auth.OAuth2;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http.HttpResults;
    using System.IO;
    using FirebaseAdmin.Messaging;
    using System.Collections.Generic;
    using DevExpress.DataProcessing.InMemoryDataProcessor;

    public class FcmSender : ControllerBase
    {

        // Lấy Server Key từ Firebase Project Settings > Cloud Messaging
        //private const string ServerKey = "YOUR_SERVER_KEY";
        //private const string ServerKey = "AAAAK8q-YIQ:APA91bFM03NfvuaDDAX_NLBtx-UAF9PP8vjHZTdmNLPl-8a8wXE5OiMyu_Yhhf98-jvsv5z1m3u5YhOPUw-RDREtoZzq9Iv38vKVOS0Nj_wMqoEN7K59Wpi8Y5dVui9ie7JE778qEMOF"; // lấy trong Firebase Project Settings > Cloud Messaging
        //private const string FcmUrl = "https://fcm.googleapis.com/fcm/send";
        // Gửi notification tới topic
        //public async Task<string> SendNotificationToTopicAsync(string topic, string title, string body)
        //{
        //    var message = new
        //    {
        //        to = $"/topics/{topic}",
        //        notification = new
        //        {
        //            title,
        //            body
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(message);
        //    var request = new HttpRequestMessage(HttpMethod.Post, FcmUrl);
        //    request.Headers.Authorization = new AuthenticationHeaderValue("key", ServerKey);
        //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _http.SendAsync(request);
        //    var content = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine(content);
        //    return content;
        //}
        // Subscribe token vào topic

        public async Task<IActionResult> SubscribeTokenToTopicAsync(string token, string topic)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
                new List<string> { token }, topic);
                return Ok(new { Success = true, Message = $"Subscribed to topic: {topic}", Response = "Thành công rồi" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
        public async Task<IActionResult> SubscribeMultipleToTopicAsync(List<string> tokens, string topic)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(tokens, topic);
                return Ok(new { Success = true, Message = $"Subscribed to topic: {topic}", Response = "Thành công rồi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
        // Gửi notification tới topic
        public async Task<string> SendNotificationToTopicAsync(string topic, string title, string body,string link)
        {

            //var projectId = _configuration["Firebase:ProjectId"];
            try
            {
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = new Dictionary<string, string>
                        {
                            { "title", title},
                            { "body", body },
                            { "link", ((string.IsNullOrEmpty(link))?"https://scansia.ddns.net":link) },
                            { "type", "chat" }
                        },
                    Topic = topic
                };

                // Gửi notification
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return messageId;
            }
            catch (Exception ex)
            {
                return $"Error sending notification: {ex.Message}";
            }
        }
      
        public async Task<string> SendDataMessageToTokensAsync(List<string> tokens, string title, string body,string link)
        {
            // Tạo message list
            tokens.Add("eup4mj0uUL9ojMPFIm2jwX:APA91bHFtvegLTUNfInULSMDqbWmAon8bvIOqvb1LFHO9t4VwY3V2hbI54xhSFqenEsU5zDCWX7d71TdxCQ-QCnMxi7rAaId0CYXuTzNK1eq72oc0Z1Ek7A");
            var messages = new List<Message>();
            try

            {
                foreach (var token in tokens)
                {
                    var message = new Message()
                    {
                        Token = token,
                        Notification = new Notification
                        {
                            Title = title,
                            Body = body
                        },
                        Data = new Dictionary<string, string>
                        {
                            { "title", title},
                            { "body", body },
                            { "link",((string.IsNullOrEmpty(link))?"https://scansia.ddns.net":link)},
                            { "type", "chat" }
                        }
                       
                    };
                   // await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    messages.Add(message);
                }
               
                //var response = await FirebaseMessaging.DefaultInstance.SendAsync(message0);

                // Gửi batch
                var response = await FirebaseMessaging.DefaultInstance.SendEachAsync(messages);
            }
            catch (Exception ex)
            {
                return $"Error sending notification: {ex.Message}";
            }
                return "OK";
        }


    }
}


