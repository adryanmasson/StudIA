using FontAwesome.Sharp;
using Material.Icons;
using Material.Icons.WPF;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StudIA.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentChildView;
        private string username = SessionService.UsuarioAtual.Username;
        private string _caption;
        private MaterialIconKind _icon;
        private BitmapImage _fotoDePerfil;

        public ViewModelBase CurrentChildView
        {
            get { return _currentChildView; }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public MaterialIconKind Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public BitmapImage FotoDePerfil
        {
            get { return _fotoDePerfil; }
            set
            {
                _fotoDePerfil = value;
                OnPropertyChanged(nameof(FotoDePerfil));
            }
        }

        // Commands
        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowPlacarViewCommand { get; }
        public ICommand ShowConfiguracoesViewCommand { get; }

        public MainViewModel()
        {
            // Initialize commands
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowPlacarViewCommand = new ViewModelCommand(ExecuteShowPlacarViewCommand);
            ShowConfiguracoesViewCommand = new ViewModelCommand(ExecuteShowConfiguracoesViewCommand);

            // Default view
            ExecuteShowHomeViewCommand(null);

            // Subscreva ao evento PropertyChanged de SessionService.UsuarioAtual
            SessionService.UsuarioAtual.PropertyChanged += UsuarioAtual_PropertyChanged;

            // Inicialize a FotoDePerfil
            FotoDePerfil = SessionService.UsuarioAtual.PerfilBitmapImage;
        }

        private void UsuarioAtual_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionService.UsuarioAtual.PerfilBitmapImage))
            {
                // Atualize a FotoDePerfil quando PerfilBitmapImage mudar
                FotoDePerfil = SessionService.UsuarioAtual.PerfilBitmapImage;
            }
        }

        private void ExecuteShowPlacarViewCommand(object obj)
        {
            CurrentChildView = new PlacarViewModel();
            Caption = "Placar";
            Icon = MaterialIconKind.UserGroup;
        }

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Home";
            Icon = MaterialIconKind.Home;
        }

        private void ExecuteShowConfiguracoesViewCommand(object obj)
        {
            CurrentChildView = new ConfiguracoesViewModel();
            Caption = "Configurações";
            Icon = MaterialIconKind.Settings;
        }
    }
}
