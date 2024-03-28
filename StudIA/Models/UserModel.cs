using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StudIA.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        private string _username;
        private string _password;
        private string _email;
        private int _pontuacao;
        private byte[] _imageSource;
        private BitmapImage _perfilBitmapImage;
        private ObservableCollection<MensagemModel> _mensagens;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public int Pontuacao
        {
            get { return _pontuacao; }
            set
            {
                if (_pontuacao != value)
                {
                    _pontuacao = value;
                    OnPropertyChanged(nameof(Pontuacao));
                }
            }
        }

        public byte[] ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    OnPropertyChanged(nameof(ImageSource));
                }
            }
        }

        public BitmapImage PerfilBitmapImage
        {
            get { return _perfilBitmapImage; }
            set
            {
                if (_perfilBitmapImage != value)
                {
                    _perfilBitmapImage = value;
                    OnPropertyChanged(nameof(PerfilBitmapImage));
                }
            }
        }

        public ObservableCollection<MensagemModel> Mensagens
        {
            get { return _mensagens; }
            set
            {
                if (_mensagens != value)
                {
                    _mensagens = value;
                    OnPropertyChanged(nameof(Mensagens));
                }
            }
        }

        private Conexao conexao;

        public UserModel()
        {
            conexao = new Conexao();
        }

        public bool AutenticarUsuario(string username, string password)
        {
            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    string query = "SELECT IdUsuario, NomeUsuario, Email, Pontuacao, FotoDePerfil FROM Usuario WHERE NomeUsuario = @NomeUsuario AND Senha = @Senha";

                    using (MySqlCommand cmd = new MySqlCommand(query, minhaConexao))
                    {
                        cmd.Parameters.AddWithValue("@NomeUsuario", username);
                        cmd.Parameters.AddWithValue("@Senha", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Id = reader.GetInt32("IdUsuario");
                                Username = reader.GetString("NomeUsuario");
                                Email = reader.GetString("Email");
                                Pontuacao = reader.GetInt32("Pontuacao");
                                try
                                {
                                    ImageSource = (byte[])reader["FotoDePerfil"];
                                    PerfilBitmapImage = ConverterBytesParaBitmapImage(ImageSource);
                                }
                                catch (InvalidCastException)
                                {
                                    ImageSource = SessionService.bytesImagemPadrao;
                                    PerfilBitmapImage = ConverterBytesParaBitmapImage(ImageSource);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Erro ao converter FotoDePerfil: {ex.Message}");
                                }

                                SessionService.UsuarioAtual = this;

                                return true; // Usuário autenticado
                            }
                            else
                            {
                                return false; // Autenticação falhou
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao autenticar usuário: {ex.Message}");
            }
            finally
            {
                conexao.FecharConexao();
            }
        }

        public void AtualizarPontuacao(int novaPontuacao)
        {
            Pontuacao = novaPontuacao;

            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    string queryAtualizarPontuacao = "UPDATE Usuario SET Pontuacao = @novaPontuacao WHERE IdUsuario = @idUsuario";

                    using (MySqlCommand cmdAtualizarPontuacao = new MySqlCommand(queryAtualizarPontuacao, minhaConexao))
                    {
                        cmdAtualizarPontuacao.Parameters.AddWithValue("@novaPontuacao", novaPontuacao);
                        cmdAtualizarPontuacao.Parameters.AddWithValue("@idUsuario", Id);

                        cmdAtualizarPontuacao.Prepare();
                        cmdAtualizarPontuacao.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar pontuação no banco de dados: {ex.Message}");
            }
            finally
            {
                conexao.FecharConexao();
            }
        }

        public bool CadastrarUsuario(string username, string senha, string email, string confirmasenha)
        {
            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    // Verifica se o e-mail já está cadastrado
                    if (VerificarEmailExistente(email, minhaConexao))
                    {
                        MessageBox.Show("E-mail já cadastrado!");
                        return false; // E-mail já cadastrado
                    }
                    else if (senha != confirmasenha)
                    {
                        MessageBox.Show("As senhas digitadas não são iguais!");
                        return false;
                    }

                    string queryCadasConta = "INSERT INTO Usuario(NomeUsuario, DataCriacao, Email, Senha)" +
                        "VALUES(@usuario, @data, @email, @senha)";

                    using (MySqlCommand cmdCadastro = new MySqlCommand(queryCadasConta, minhaConexao))
                    {
                        cmdCadastro.Parameters.AddWithValue("@usuario", username);
                        cmdCadastro.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmdCadastro.Parameters.AddWithValue("@email", email);
                        cmdCadastro.Parameters.AddWithValue("@senha", senha);

                        cmdCadastro.Prepare();
                        cmdCadastro.ExecuteNonQuery();

                        return true; // Cadastro realizado com sucesso
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar usuário: {ex.Message}");
            }
            finally
            {
                conexao.FecharConexao();
            }
        }

        private bool VerificarEmailExistente(string email, MySqlConnection conexao)
        {
            string queryVerificarEmail = "SELECT * FROM Usuario WHERE Email = @email";

            using (MySqlCommand cmdSelect = new MySqlCommand(queryVerificarEmail, conexao))
            {
                cmdSelect.Parameters.AddWithValue("@email", email);
                cmdSelect.Prepare();

                using (MySqlDataReader reader = cmdSelect.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        public BitmapImage ConverterBytesParaBitmapImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return ConverterBytesParaBitmapImage(SessionService.bytesImagemPadrao);
            }

            BitmapImage imagem = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                stream.Position = 0;

                imagem.BeginInit();
                imagem.CacheOption = BitmapCacheOption.OnLoad;
                imagem.StreamSource = stream;
                imagem.EndInit();
            }

            return imagem;
        }
    }
}
