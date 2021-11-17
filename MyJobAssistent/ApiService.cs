﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyJobAssistent
{
    public class ApiService : IApiService
    {
        public List<AppHealthActionConfig> _appHealthConfigs;
        public ApiService()
        {
            _appHealthConfigs = new List<AppHealthActionConfig>
            {
                new AppHealthActionConfig{ EndpointName= "FirstApi", EndPoint = "https://papi.liveoptics-dev.com/v2/api/health", ApiType="Web API" },
                new AppHealthActionConfig{ EndpointName= "SecondApi", EndPoint = "http://dellrestapi-env.eba-enmpcutb.us-east-2.elasticbeanstalk.com/health", ApiType="Web API" }
            };
        }        

        public async Task<List<AppHealthConfig>> GetHealthStatus()
        {
            List<AppHealthConfig> appHealthConfigs = new List<AppHealthConfig>();

            foreach (AppHealthConfig appHealthConfig in _appHealthConfigs)
            {
                AppHealthStatus appHealthStatus = await GetApiHealth(appHealthConfig);

                if (appHealthStatus.Status == AppHealthStatus.Healthy)
                    appHealthConfig.BackColor = Color.Green;

                appHealthConfigs.Add(appHealthConfig);
            }

            return appHealthConfigs;
        }

        private async Task<AppHealthStatus> GetApiHealth(AppHealthConfig appHealthConfig)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(appHealthConfig.EndPoint);

                var content = await response.Content.ReadAsStringAsync();
                var appHealthStatus = JsonConvert.DeserializeObject<AppHealthStatus>(content);
                return appHealthStatus;
            }
        }

        public async Task<List<AppHealthActionConfig>> SaveHealthConfigs(List<AppHealthActionConfig> appHealthConfigs)
        {
            _appHealthConfigs = appHealthConfigs;
            return appHealthConfigs;
        }
    }
}
