using Newtonsoft.Json;
using StudIA.Core;
using StudIA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StudIA.ViewModels
{
    public class MensagensViewModel : ObservableObject
    {
        private readonly Chat chat;
        private ChatGptApi chatGptApi;

        public ObservableCollection<MensagemModel> Mensagens { get; set; }
        public RelayCommand SendCommand { get; set; }

        private UserModel _currentUser;
        private string _mensagemIa;
        private bool msgBoxStatus = true;
        private string _mensagem;

        public UserModel CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public bool MsgBoxStatus
        {
            get { return msgBoxStatus; }
            set
            {
                msgBoxStatus = value;
                OnPropertyChanged();
            }
        }

        public string Mensagem
        {
            get { return _mensagem; }
            set
            {
                _mensagem = value;
                OnPropertyChanged();
            }
        }

        public string MensagemIA
        {
            get { return _mensagemIa; }
            set
            {
                _mensagemIa = value;
                OnPropertyChanged();
            }
        }

        public MensagensViewModel()
        {
            Mensagens = new ObservableCollection<MensagemModel>();
            CurrentUser = SessionService.UsuarioAtual;
            chatGptApi = new ChatGptApi("Sua-Chave-API");
            chat = new Chat();
            CarregarHistoricoChat();

            SendCommand = new RelayCommand(async o =>
            {
                if (!string.IsNullOrEmpty(Mensagem))
                {
                    Mensagens.Add(new MensagemModel
                    {
                        Username = CurrentUser.Username,
                        ImageSource = CurrentUser.PerfilBitmapImage,
                        Time = DateTime.Now,
                        Mensagem = Mensagem,
                        IsNativeOrigin = false,
                        FirstMessage = true
                    });
                    chat.AdicionarMensagem(CurrentUser.Id, Mensagem);
                    await EnviarMensagensParaGptAsync();
                }
            });
        }

        private async Task EnviarMensagensParaGptAsync()
        {
            try
            {
                Mensagem = "Carregando...";
                MsgBoxStatus = false;
                List<MensagemApi> mensagensApi = Mensagens
                    .Select(m => new MensagemApi { role = m.IsNativeOrigin ? "assistant" : "user", content = m.Mensagem })
                    .ToList();

                string respostaGpt = await chatGptApi.ObterRespostaAsync(mensagensApi);

                if (string.IsNullOrEmpty(respostaGpt))
                {
                    MessageBox.Show("Resposta da API vazia ou nula. Nenhuma mensagem da IA adicionada.");
                    return;
                }

                Mensagens.Add(new MensagemModel
                {
                    Username = "Edu",
                    ImageSource = SessionService.ImagemPadraoIA,
                    Time = DateTime.Now,
                    Mensagem = respostaGpt,
                    IsNativeOrigin = true,
                    FirstMessage = true
                });

                if (respostaGpt.StartsWith("Correto!"))
                {
                    int novaPontuacao = CurrentUser.Pontuacao + 10;
                    CurrentUser.AtualizarPontuacao(novaPontuacao);
                }
                

                chat.AdicionarMensagemIA(CurrentUser.Id, respostaGpt);
                MsgBoxStatus = true;
                Mensagem = "";
            }
            catch (Exception ex)
            {
                // Mensagem de depuração
                MessageBox.Show($"Erro ao chamar a API do Chat GPT: {ex.Message}");
            }
        }
        public async Task EnviarMensagemParaGptSemAdicionarAsync(string mensagem)
        {
            try
            {
                Mensagem = "Carregando...";
                MsgBoxStatus = false;
                // Criar lista de mensagens para enviar à API do Chat GPT
                List<MensagemApi> mensagensApi = Mensagens
                    .Select(m => new MensagemApi { role = m.IsNativeOrigin ? "assistant" : "user", content = m.Mensagem })
                    .ToList();

                // Adicionar a nova mensagem do usuário
                mensagensApi.Add(new MensagemApi { role = "user", content = mensagem });

                // Chamar a API do Chat GPT
                string respostaGpt = await chatGptApi.ObterRespostaAsync(mensagensApi);

                Mensagens.Add(new MensagemModel
                {
                    Username = "Edu",
                    ImageSource = SessionService.ImagemPadraoIA,
                    Time = DateTime.Now,
                    Mensagem = respostaGpt,
                    IsNativeOrigin = true,
                    FirstMessage = true
                });

                

                // Tratar a resposta conforme necessário (pode adicionar ao banco de dados, por exemplo)
                chat.AdicionarMensagemIA(CurrentUser.Id, respostaGpt);
                MsgBoxStatus = true;
                Mensagem = "";
            }
            catch (Exception ex)
            {
                // Tratar exceções ao chamar a API do Chat GPT
                MessageBox.Show($"Erro ao chamar a API do Chat GPT: {ex.Message}");
            }
        }




        private void CarregarHistoricoChat()
        {
            // Limpar mensagens atuais
            Mensagens.Clear();

            // Buscar histórico de chat do banco de dados
            DataTable historico = chat.BuscarHistoricoChat(CurrentUser.Id);

            foreach (DataRow row in historico.Rows)
            {
                if (row["Remetente"].ToString() == "IA") {
                    Mensagens.Add(new MensagemModel
                    {
                        Username = "Edu",
                        ImageSource = SessionService.ImagemPadraoIA,
                        Time = Convert.ToDateTime(row["DataEnvio"]),
                        Mensagem = row["Conteudo"].ToString(),
                        IsNativeOrigin = true,
                        FirstMessage = true
                    });
                }
                else
                {
                    Mensagens.Add(new MensagemModel
                    {
                        Username = CurrentUser.Username,
                        ImageSource = CurrentUser.PerfilBitmapImage,
                        Time = Convert.ToDateTime(row["DataEnvio"]),
                        Mensagem = row["Conteudo"].ToString(),
                        IsNativeOrigin = false,
                        FirstMessage = true
                    });
                }

                
            }
        }
    }
}
