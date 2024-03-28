using StudIA.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace StudIA.ViewModels
{
    public class PlacarViewModel : ViewModelBase
    {
        private UsuarioDAO usuarioDAO;

        private ObservableCollection<UserModel> usuarios;
        public ObservableCollection<UserModel> Usuarios
        {
            get { return usuarios; }
            set
            {
                usuarios = value;
                OnPropertyChanged(nameof(Usuarios));
            }
        }

        public PlacarViewModel()
        {
            usuarioDAO = new UsuarioDAO();
            CarregarUsuarios();
        }

        private void CarregarUsuarios()
        {
            // Obter a lista de usuários do banco de dados e ordenar por pontuação
            var usuariosOrdenados = usuarioDAO.ObterTodosUsuarios().OrderByDescending(u => u.Pontuacao);

            // Atribuir a coleção ordenada à propriedade Usuarios
            Usuarios = new ObservableCollection<UserModel>(usuariosOrdenados);
        }
    }
}
