using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudIA
{
    public class ChatGptApi
    {
        private readonly string apiUrl = "https://api.openai.com/v1/chat/completions";
        private readonly HttpClient httpClient;

        public ChatGptApi(string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> ObterRespostaAsync(List<MensagemApi> mensagens)
        {
            try
            {
                var mensagemSistema = new StudIA.MensagemApi
                {
                    role = "system",
                    content = "Você é um assistente focado na área de educação e muito simpático, e deve se comprometer a ensinar." +
                    " Nunca use a palavra 'Correto!', a não ser que esteja fazendo um questionário para o usuário: nesse caso, sempre que o" +
                    " usuário acertar a resposta, comece sua mensagem com 'Correto!'"
                };
                mensagens.Insert(0, mensagemSistema);
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = mensagens
                };

                string requestBodyJson = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Post, apiUrl))
                {
                    request.Content = content;

                    using (var response = await httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string respostaJson = await response.Content.ReadAsStringAsync();

                            // Converter a resposta JSON para um objeto anônimo para acessar o campo 'content'
                            var respostaObj = JsonConvert.DeserializeAnonymousType(respostaJson, new { choices = new[] { new { message = new { content = "" } } } });

                            // Acessar o campo 'content' da primeira escolha (choice) na resposta
                            string conteudoResposta = respostaObj?.choices?.FirstOrDefault()?.message?.content;

                            return conteudoResposta;
                        }
                        else
                        {
                            string respostaJson = await response.Content.ReadAsStringAsync();
                            // Mensagens de depuração
                            MessageBox.Show($"Erro na chamada à API: {response.StatusCode}");

                            MessageBox.Show($"Conteúdo da Resposta: {respostaJson}");
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Mensagens de depuração
                MessageBox.Show($"Erro ao chamar a API do Chat GPT: {ex.Message}");
                return null;
            }
        }

    }

    public class MensagemApi
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
