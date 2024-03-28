using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace StudIA
{
    public class Chat
    {
        private Conexao conexao;

        public Chat()
        {
            conexao = new Conexao();
        }

        public DataTable BuscarHistoricoChat(int idUsuario)
        {
            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    string query = "SELECT * FROM Chat WHERE IdUsuario = @IdUsuario ORDER BY DataEnvio";

                    using (MySqlCommand cmd = new MySqlCommand(query, minhaConexao))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar histórico de chat: {ex.Message}");
            }
            finally
            {
                conexao.FecharConexao();
            }
        }

        public void AdicionarMensagem(int idUsuario, string conteudo)
        {
            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    string query = "INSERT INTO Chat (IdUsuario, Conteudo, Remetente, DataEnvio) VALUES (@IdUsuario, @Conteudo, @Remetente, NOW())";

                    using (MySqlCommand cmd = new MySqlCommand(query, minhaConexao))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@Conteudo", conteudo);
                        cmd.Parameters.AddWithValue("@Remetente", "usuario");

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar mensagem: {ex.Message}");
            }
            finally
            {
                conexao.FecharConexao();
            }
        }

        public void AdicionarMensagemIA(int idUsuario, string conteudo)
        {
            try
            {
                using (MySqlConnection minhaConexao = conexao.AbrirConexao())
                {
                    string query = "INSERT INTO Chat (IdUsuario, Conteudo, Remetente, DataEnvio) VALUES (@IdUsuario, @Conteudo, @Remetente, NOW())";

                    using (MySqlCommand cmd = new MySqlCommand(query, minhaConexao))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@Conteudo", conteudo);
                        cmd.Parameters.AddWithValue("@Remetente", "IA");

                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar mensagem da IA: {ex.Message}");
            }
            finally
            {
                conexao.FecharConexao();
            }
        }
    }
}
