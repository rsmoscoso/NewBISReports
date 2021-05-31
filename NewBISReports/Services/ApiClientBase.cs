using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.IO;


namespace NewBISReports.Services
{
    public class ApiClientBase
    {
        private Uri BaseEndpoint { get; set; }

        public ApiClientBase()
        {

        }

        //Método para fazer o GET de informações no banco do BIS
        public async Task<T> GetAsync<T>(HttpClient _apiClient, string relativePath, string queryString)
        {
            BaseEndpoint = _apiClient.BaseAddress;
            var requestUrl = CreateRequestUri(relativePath, queryString); //Cria o endereço completo para se conectar com o BIS e fazer a requisição
            var response = await _apiClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead); //Faz a requisição no BIS
            response.EnsureSuccessStatusCode(); //Verifica se houve sucesso na requisição
            var data = await response.Content.ReadAsAsync(typeof(string)); //Transforma em uma string (json)
            var temp = JsonConvert.DeserializeObject<T>((string)data); //Faz deserializa o json para fazer o retorno do metodo
            return temp;
        }
        //Método para fazer o POST de informações no banco do BIS
        public async Task<string> PostAsync<T1>(HttpClient _apiClient, string relativePath, string queryString, T1 content)
        {
            BaseEndpoint = _apiClient.BaseAddress;
            var requestUrl = CreateRequestUri(relativePath, queryString); //Cria a url de requisição do banco
            var seila = CreateHttpContent<T1>(content);
            var response = await _apiClient.PostAsync(requestUrl.ToString(), seila); //Manda a requisição ao banco (Segundo argumento é transformado em JSON)
            response.EnsureSuccessStatusCode(); //Verifica se houve sucesso na requisição
            return await response.Content.ReadAsStringAsync(); //Retorna a string para verificar se houve sucesso na requisição
        }

        //TODO: podeira implementar o cache como um singleton, fazendo um hash da requestUrl + body em caso de Post e só da requestUrl
        // em caso de GET. aí cada um tem o payload da response e um timer de pelo menos uns 10 segundos. Desta forma, o refresh da tela 
        //poderia ser sempre global. 


        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }
        private HttpContent CreateHttpContent<T>(T content)
        {
            //Diogo - Provavelmente InfraSpeak
            //var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    NullValueHandling = NullValueHandling.Ignore
                };
            }
        }

    }
      
}
