using APIServerNFC.API_Admin;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

using MQTTnet;
using System.Text;
using DevExpress.XtraPrinting;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace APIServerNFC.Controllers
{
    //public class MqttService : BackgroundService
    public class MqttService
    {

        public MQTTnet.IMqttClient _mqttClient;
        public MqttClientOptions _options;
        private readonly ICommandHandler _commandHandler;
        //  public string mqttserver= "123.31.41.25";
        public string mqttserver = "quocanhgas.com";
        public int port = 1883;//Port TCP
        public string usermsg = "huynhit";
        string password = "huynhit@mqtt";
        string clientId = "";
        //string cacrtBase64 = "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlFR1RDQ0F3R2dBd0lCQWdJ\r\nVUUxQnBCVTFyajM5enB2YWtrcGpQcVNFMVMrNHdEUVlKS29aSWh2Y05BUUVMDQpC\r\nUUF3Z1pzeEN6QUpCZ05WQkFZVEFsWk9NUkV3RHdZRFZRUUlEQWhFVDA1SElFNUJT\r\nVEVSTUE4R0ExVUVCd3dJDQpRa2xGVGlCSVQwRXhHREFXQmdOVkJBb01EMU5EUVU1\r\nVFNVRWdVRUZEU1VaSlF6RUxNQWtHQTFVRUN3d0NTVlF4DQpHVEFYQmdOVkJBTU1F\r\nSE5qWVc1emFXRXVaR1J1Y3k1dVpYUXhKREFpQmdrcWhraUc5dzBCQ1FFV0ZWWlZW\r\nRWhCDQpTVWhWV1U1SVFFZE5RVWxNTGtOUFRUQWVGdzB5TkRFeE1qSXdNelV6TlRk\r\nYUZ3MHpOREV4TWpBd016VXpOVGRhDQpNSUdiTVFzd0NRWURWUVFHRXdKV1RqRVJN\r\nQThHQTFVRUNBd0lSRTlPUnlCT1FVa3hFVEFQQmdOVkJBY01DRUpKDQpSVTRnU0U5\r\nQk1SZ3dGZ1lEVlFRS0RBOVRRMEZPVTBsQklGQkJRMGxHU1VNeEN6QUpCZ05WQkFz\r\nTUFrbFVNUmt3DQpGd1lEVlFRRERCQnpZMkZ1YzJsaExtUmtibk11Ym1WME1TUXdJ\r\nZ1lKS29aSWh2Y05BUWtCRmhWV1ZWUklRVWxJDQpWVmxPU0VCSFRVRkpUQzVEVDAw\r\nd2dnRWlNQTBHQ1NxR1NJYjNEUUVCQVFVQUE0SUJEd0F3Z2dFS0FvSUJBUUN5DQpE\r\nNlJhc1ZHS2N1byt1VHFOTXlNSmMxd1p4azNlMEZNczUzZjNoY2RUd2pLWHpqb0tM\r\nSERVMVFQRlVINUw4VFpYDQpGNSs4WmhlR0R3L3NKSTF5Y2orQWFJME1uNFc4UnpX\r\nUmNadFNjRUNhRER3WmJzS3V6Zk8zdHRNKzFXWUttUWdQDQp0UW1mdGRNbytvblZv\r\najJ1NnAyUVY2aWFNajZ2eHpJVW45dHkrTE1ITDB6ZGxRM0h0cnRxMlNuK1ZwZGgw\r\nOHZwDQoxWTRuY3JPMkVZUENJL2hrSXpWaVVQR2d1QTVhQ2VjN2hjaUNjTGRObG9p\r\nbEEzNDdqZWlndThBSnR4ald0cXU3DQp4aHY3NVVyWTBKVmZZZnhmTm9zZnArU2xH\r\nNEZ3Unc3TDJnRUFQV01jVjRmczljTUh5Tkl5OUlGYUpsQTFTMElQDQpnK291MEFp\r\ncVdyNjB6ZUFGTkxXWEFnTUJBQUdqVXpCUk1CMEdBMVVkRGdRV0JCUlpKOU1GSkR5\r\nRWtjYis0S25KDQpKNSs2QzNaZjdUQWZCZ05WSFNNRUdEQVdnQlJaSjlNRkpEeUVr\r\nY2IrNEtuSko1KzZDM1pmN1RBUEJnTlZIUk1CDQpBZjhFQlRBREFRSC9NQTBHQ1Nx\r\nR1NJYjNEUUVCQ3dVQUE0SUJBUUFNSThlME90MElCUEw4WmxGTnpRL2JSbEo3DQpW\r\nTjNrUmF0QnlrL056dWd4RFJkbWdxQVRwSXVYb3JPNzBXZ1ptUVJ2bENGSmhGZUcx\r\nRDRMQXUyN0RPbHZlYWJsDQo3Wm5ZdlNHb1dpME4xcjEvaEMvdTVBYUZrcjA3aFE4\r\nZGlYU2ROM284L3BYVm9jRkwxYUk4QTFNSzBWdUpLVGVWDQpRYnlWSkU3eWN0Mnpl\r\nV0NpUzkwRVZxeUVVeXV4K0w3KzJPUE15NDIyY21OUEFVVG9yUUlaTk1BNTlmQ3hQ\r\neUhqDQo2Q0IrcUxTSEhzM1pnV1lkaDhsWlVwakY4d3FodEI5NHRUZ3FRN1NqZlRy\r\nd2hWb1dsV0dnR0xMVUxoemZQa3YxDQpLWVFSYlBXS3RSMG9GWnpmOVpkalh2NnJk\r\nV3NJMFVNcXE2Q3J0bDQwbzA1N3FVRzYrMWNSSTU2REFzWmMNCi0tLS0tRU5EIENF\r\nUlRJRklDQVRFLS0tLS0NCg==";
        //string clientCertPfxBase64 = "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\r\nd1S1kMAECIHwfIzy3B5rAgIIAA==";


        public string RandomString(int length)
        {

            Random random = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        public MqttService(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            _mqttClient = new MqttClientFactory().CreateMqttClient();
            var mqttFactory = new MqttClientFactory();
            //Tạo 1 clientId ngẫu nhiên để tránh trùng lặp
            clientId = RandomString(10);
            //using (var mqttClient = mqttFactory.CreateMqttClient())
            //{
            //    var ut = new MqttClientOptionsBuilder()
            //        .WithTcpServer(mqttserver, port)//.WithCredentials(usermsg, password) // thêm user và pass để nhận hotline từ mqtt
            //        .WithClientId(clientId)              // Giữ ClientId cố định
            //        .WithCleanSession(false);            // Giữ session & subscription sau reconnect
            //        _options = ut.Build();
            //}
            // Build options gắn với _mqttClient

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttserver, port)
                .WithClientId(clientId)
                .WithCleanSession(false)
                .Build();

            lsttopcic.Add("qhu/data");
           
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                await _commandHandler.HandleCommandAsync(topic, payload);
            };
            _mqttClient.ConnectedAsync += async e =>
            {
                // Subscribe topic khi connect
                await _mqttClient.SubscribeAsync("qhu/data");
                Console.WriteLine("Subscribed to qhu/data");
            };

            _mqttClient.DisconnectedAsync += e =>
            {
                Console.WriteLine("⚠️ MQTT Disconnected!");
                return Task.CompletedTask;
            };

            // Connect ngay
            _ = ConnectAsync();

        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    await _mqttClient.ConnectAsync(_options, stoppingToken);


        //    await Task.Delay(-1, stoppingToken); // Giữ service chạy
        //}
        private async Task SubcribeTopicAsync()
        {
            foreach (var it in lsttopcic)
            {
                await _mqttClient.SubscribeAsync(it);
                Console.WriteLine("[MQTT] Đã subscribe topic");
            }

        }
        public async Task ConnectAsync()
        {
            try
            {
                // Kết nối đến broker
                await _mqttClient.ConnectAsync(_options);
                if (_mqttClient.IsConnected)
                {
                    Console.WriteLine("Kết nối thành công...");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

        }
        public Action<string> getMsgFromMQTT;
        public List<string> lsttopcic = new List<string>();


        public async Task PublishAsync(string topic, string payload)
        {
            JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
            jsonMsgAndroid.topic = topic;
            jsonMsgAndroid.message = payload;
            jsonMsgAndroid.clientid = _options.ClientId;

            string json = JsonConvert.SerializeObject(jsonMsgAndroid);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)//Gửi 1 lần, và không nhận được thì thôi, ko cần chờ gửi lại, vì nếu còn chờ để gửi lại thì dễ die cho server, vì server yếu sinh lý
                .Build();

            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(message);
            }
            else
            {
                Console.WriteLine("Kết nối đã mất. Đang thử kết nối lại...");
                await ReconnectAsync();
            }
        }
        public async Task PublishAsync(string topic, string payload, string usersend, List<string> lstuserreceive)
        {
            JsonMsgAndroid jsonMsgAndroid = new JsonMsgAndroid();
            jsonMsgAndroid.topic = topic;
            jsonMsgAndroid.message = payload;
            jsonMsgAndroid.clientid = _options.ClientId;
            jsonMsgAndroid.usersend = usersend;
            jsonMsgAndroid.lstuserrecive = lstuserreceive;
            string json = JsonConvert.SerializeObject(jsonMsgAndroid);
            // Console.WriteLine(json);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)//Không nhận được thì thôi, ko cần chờ gửi lại, vì nếu còn chờ để gửi lại thì dễ die cho server, vì server yếu sinh lý
                .Build();

            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(message);
            }
            else
            {
                Console.WriteLine("Kết nối đã mất. Đang thử kết nối lại...");
                await ReconnectAsync();
            }
        }
        public async Task PublishMsg(string topic, string msg)
        {

            // Console.WriteLine(json);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)//Không nhận được thì thôi, ko cần chờ gửi lại, vì nếu còn chờ để gửi lại thì dễ die cho server, vì server yếu sinh lý
                .Build();

            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(message);
            }
            else
            {
                Console.WriteLine("Kết nối đã mất. Đang thử kết nối lại...");
                await ReconnectAsync();
            }
        }
        private async Task PingAsync()
        {
            if (_mqttClient.IsConnected)
            {
                // Gửi một tin nhắn ping để kiểm tra kết nối
                var pingMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("ping")
                    .WithPayload("ping")
                    .Build();
                await _mqttClient.PublishAsync(pingMessage);
                Console.WriteLine("Ping sent to broker to check connection.");
            }
            else
            {
                Console.WriteLine("Kết nối đã mất. Đang thử kết nối lại...");
                await ReconnectAsync();
            }
        }
        public async Task ReconnectAsync()
        {
            if (!_mqttClient.IsConnected)
            {
                try
                {
                    await _mqttClient.ConnectAsync(_options);
                    if (_mqttClient.IsConnected)
                        Console.WriteLine("Đã kết nối thành công.");
                    _ = SubcribeTopicAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ MQTT connect failed: " + ex.Message);
                }
            }

        }
    }
}
