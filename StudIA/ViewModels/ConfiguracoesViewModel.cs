using MySql.Data.MySqlClient;
using StudIA.Core;
using StudIA.Models;
using StudIA.Views;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StudIA.ViewModels
{
    public class ConfiguracoesViewModel : ViewModelBase
    {
        private int _id = SessionService.UsuarioAtual.Id;
        private string _nomeusuario = SessionService.UsuarioAtual.Username;
        private string _email = SessionService.UsuarioAtual.Email;
        private BitmapImage _image = SessionService.UsuarioAtual.PerfilBitmapImage;

        private ICommand _trocarImagemCommand;

        private ICommand _logoutCommand;

        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(Logout);
                }
                return _logoutCommand;
            }
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

        public string NomeUsuario
        {
            get { return _nomeusuario; }
            set
            {
                if (_nomeusuario != value)
                {
                    _nomeusuario = value;
                    OnPropertyChanged(nameof(NomeUsuario));
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

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        public ICommand TrocarImagemCommand
        {
            get
            {
                if (_trocarImagemCommand == null)
                {
                    _trocarImagemCommand = new RelayCommand(TrocarImagem);
                }
                return _trocarImagemCommand;
            }
        }

        private void Logout(object parameter)
        {
            SessionService.UsuarioAtual = null;
            LoginView loginView = new LoginView();
            loginView.Show();
            MainView mainView = System.Windows.Application.Current.Windows.OfType<MainView>().FirstOrDefault();
            if (mainView != null)
            {
                mainView.Close();
            }
        }

        private void TrocarImagem(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            DialogResult resultado = openFileDialog.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                try
                {
                    BitmapImage novaImagem = new BitmapImage(new Uri(openFileDialog.FileName));
                    Image = novaImagem;

                    // Atualize a propriedade Image do usuário atual com a nova imagem
                    SessionService.UsuarioAtual.PerfilBitmapImage = Image;


                    // Atualize a imagem no banco de dados
                    AtualizarImagemNoBancoDeDados(SessionService.UsuarioAtual.Id, openFileDialog.FileName);


                }
                catch (Exception ex)
                {
                    // Trate a exceção, se necessário
                    System.Windows.MessageBox.Show("Erro ao carregar a nova imagem: " + ex.Message);
                }
            }
        }


        private void AtualizarImagemNoBancoDeDados(int userId, string imagePath)
        {
            try
            {
                using (MySqlConnection conexao = new Conexao().AbrirConexao())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conexao;
                        cmd.CommandText = "UPDATE Usuario SET FotoDePerfil = @Imagem WHERE IdUsuario = @UserId";

                        // Converta a imagem para bytes e adicione como parâmetro
                        byte[] imagemBytes = File.ReadAllBytes(imagePath);
                        cmd.Parameters.AddWithValue("@Imagem", imagemBytes);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Trate a exceção, se necessário
                Console.WriteLine("Erro ao atualizar imagem no banco de dados: " + ex.Message);
            }
        }
    }


}
