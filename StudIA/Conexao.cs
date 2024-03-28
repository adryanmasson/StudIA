using MySql.Data.MySqlClient;
using StudIA.Models;
using System;
using System.Collections.Generic;

namespace StudIA
{
    public class Conexao
    {
        private string connectionString;
        private MySqlConnection conexao;

        public Conexao()
        {
            // Configure a string de conexão com suas informações específicas
            connectionString = "server=localhost;userid=root;password=root;database=StudIA;";
            conexao = new MySqlConnection(connectionString);
        }

        public MySqlConnection AbrirConexao()
        {
            try
            {
                conexao.Open();
                return conexao;
            }
            catch (MySqlException ex)
            {
                // Trate exceções de conexão aqui
                throw new Exception($"Erro ao abrir a conexão: {ex.Message}");
            }
        }

        public void FecharConexao()
        {
            try
            {
                conexao.Close();
            }
            catch (MySqlException ex)
            {
                // Trate exceções de fechamento de conexão aqui
                throw new Exception($"Erro ao fechar a conexão: {ex.Message}");
            }
        }
    }

    public class UsuarioDAO
    {
        private Conexao conexao;

        public UsuarioDAO()
        {
            conexao = new Conexao();
        }

        public List<UserModel> ObterTodosUsuarios()
        {
            List<UserModel> usuarios = new List<UserModel>();

            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    string query = "SELECT IdUsuario, NomeUsuario, Pontuacao FROM Usuario";

                    using (MySqlCommand cmd = new MySqlCommand(query, minhaConexao))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel usuario = new UserModel
                                {
                                    Id = reader.GetInt32("IdUsuario"),
                                    Username = reader.GetString("NomeUsuario"),
                                    Pontuacao = reader.GetInt32("Pontuacao")
                                };

                                usuarios.Add(usuario);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Lide com exceções aqui
            }

            return usuarios;
        }
    }
}
