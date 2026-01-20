using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace APIServerNFC.Controllers
{
    public class MqttServiceBackground : BackgroundService
    {
        private readonly MqttService _mqtt;

        public MqttServiceBackground(MqttService mqtt)
        {
            _mqtt = mqtt;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await _mqttClient.ConnectAsync(_options, stoppingToken);

            await _mqtt.ConnectAsync();
            await Task.Delay(-1, stoppingToken); // Giữ service chạy
            //throw new System.NotImplementedException();
        }
    }
}
