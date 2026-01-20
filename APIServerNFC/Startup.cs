using APIServerNFC.API_Admin;
using APIServerNFC.Controllers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.OpenApi.Models;
using Quartz;
using System;


namespace APIServerNFC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
           
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIServerNFC", Version = "v1" });
            });
            //Thêm vào để chạy cho host demo,
            services.AddCors(policy =>
            {
                policy.AddPolicy("AllowSpecificOrigin", opt => opt
                .WithOrigins("https://localhost:44352", "http://113.161.144.105:9736", "http://localhost:44490", "http://scansia.ddns.net", "https://scansia.ddns.net")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    );
            });
            services.AddSingleton<ICommandHandler, CommandHandler>();
            services.AddSingleton<MqttService>();
            services.AddHostedService<MqttServiceBackground>();
            //Khởi tạo khi Host được bật và chạy trong suốt quá trình hoạt động của Host
            #region API chính không dùng timer, cái này để cấu hình cho API phụ

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                // Đăng ký job
                var jobKey = new JobKey("MyQuartzJob");
                q.AddJob<ScheduledJobService>(opts => opts.WithIdentity(jobKey));

                // Tạo trigger 1 phút một lần
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("MyQuartzJob-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(1)
                        .RepeatForever()));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            #endregion

            //services.AddSingleton<MqttService>();//Kết nối lâu dài
            services.AddSingleton<FtpService>();//Thêm vào để tải file từ FTP

            services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(1); // Khoảng thời gian client được giữ kết nối
                options.KeepAliveInterval = TimeSpan.FromSeconds(30); // Gửi tín hiệu ping mỗi 30 giây
            });
            //services.AddControllers();
            //services.AddHttpClient();
            services.AddScoped<FcmSender>();
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("Controllers/firebase-service-account.json")
            });
            //services.AddHttpClient<FcmSender>();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.WithOrigins("http://ungdungquocanh.hopto.org:89")
            //                          .AllowAnyHeader()
            //                          .AllowAnyMethod());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIServerNFC v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            //Thêm vào để chạy cho host demo
            app.UseCors("AllowSpecificOrigin");
         
            
          

            app.UseAuthorization();
            //app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");

            });
        }
    }
}
