using MySql.Data.MySqlClient;
using StudIA.Models;
using StudIA.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace StudIA
{

    public partial class SignUpView : Window
    {

        MySqlConnection connglobal = new MySqlConnection();

        public SignUpView()
        {
            InitializeComponent();
            
        }

        private void btnCadastro_Click(object sender, RoutedEventArgs e)
        {
            UserModel usuario = new UserModel();
            string username = txtUser.Text;
            string email = txtEmail.Text;
            string senha = txtSenha.Password;
            string confirmaSenha =  txtSenhaConfirma.Password;

            bool cadastradoComSucesso = usuario.CadastrarUsuario(username, senha, email, confirmaSenha);

            if (cadastradoComSucesso)
            {
                usuario.AutenticarUsuario(username, senha);
                MainView mainView = new MainView();
                mainView.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Seu cadastro nao pôde ser efetuado!");
            }
        }

        private void txtLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoginView loginView = new LoginView();
            loginView.Show();
            this.Close();
        }
    }
}