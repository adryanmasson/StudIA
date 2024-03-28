using StudIA.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudIA.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {


        public HomeView()
        {

            InitializeComponent();

            DataContext = new MensagensViewModel();

        }



        public object Mensagens
        {
            get { return GetValue(MensagensProperty); }
            set { SetValue(MensagensProperty, value); }
        }

        public static readonly DependencyProperty MensagensProperty =
            DependencyProperty.Register("Mensagens", typeof(object), typeof(HomeView), new PropertyMetadata(null));

        private void btnSend_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (DataContext as MensagensViewModel)?.SendCommand.Execute(e);
            }
        }
        private async void btnAvaliacao_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var viewModel = DataContext as MensagensViewModel;

                if (viewModel != null)
                {
                    // Escolher a mensagem específica
                    string mensagemParaGpt = "Prepare uma avaliação, com 1 a 5 questões," +
                        " de acordo com a quantidade de conteúdo disponível, utilizando como base APENAS o conteúdo citado acima," +
                        " sendo completamente possível responder o questionário apenas com o texto acima," +
                        " sem recorrer a conhecimentos prévios. Cada questão correta vale 10 pontos." +
                        " Inicie a sua resposta com 'Vamos começar a avaliação! Teremos x questões," +
                        " cada uma valendo 10 pontos!', com 'x' sendo a quantidade de questões que você fará." +
                        "Cada questão deve possuir 5 alternativas, sendo 1 certa e 3 erradas." +
                        " As alternativas são a, b, c, d. Não mostre as questões todas de uma vez." +
                        " Me mostre uma questão e aguarde minha resposta." +
                        " Caso eu erre, me explique o por quê estou errado.Nessa mesma mensagem me mostre outra questão" +
                        " e assim consecutivamente. Não pare de mandar as questões enquanto não tiver atingido" +
                        " o número de questões estabelecido por você.";
                    await viewModel.EnviarMensagemParaGptSemAdicionarAsync(mensagemParaGpt);

                }
            }
        }

    }
}