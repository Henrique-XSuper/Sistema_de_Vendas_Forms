using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form4 : Form
    {
        private string connectionString = @"Server=OSA0716282W11-1\SQLEXPRESS;Database=projeto;Trusted_Connection=True;";
        private int selectedId = -1; // Armazena o ID do registro selecionado

        public Form4()
        {
            InitializeComponent();
            CarregarDados();
        }

        // Cadastrar
        private void btnCadastra_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textModelo.Text) ||
                string.IsNullOrWhiteSpace(textMarca.Text) ||
                string.IsNullOrWhiteSpace(textValor.Text) ||
                string.IsNullOrWhiteSpace(textAno.Text))
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Automovel (Modelo, Marca, Valor, Ano) VALUES (@Modelo, @Marca, @Valor, @Ano)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Modelo", textModelo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Marca", textMarca.Text.Trim());
                    cmd.Parameters.AddWithValue("@Valor", decimal.Parse(textValor.Text));
                    cmd.Parameters.AddWithValue("@Ano", int.Parse(textAno.Text));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                LimparCampos();
                CarregarDados();
                MessageBox.Show("Automóvel cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao cadastrar", "Não foi possível cadastrar o automóvel.", ex);
            }
        }

        // Pesquisar
        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    // Pesquisa pelo Id se preenchido
                    if (!string.IsNullOrWhiteSpace(textCodigo.Text) && int.TryParse(textCodigo.Text.Trim(), out int idSearch))
                    {
                        cmd.CommandText = "SELECT Id, Modelo, Marca, Valor, Ano FROM Automovel WHERE Id = @Id";
                        cmd.Parameters.AddWithValue("@Id", idSearch);
                    }
                    else
                    {
                        // Pesquisa por Modelo e Marca
                        cmd.CommandText = @"
                            SELECT Id, Modelo, Marca, Valor, Ano 
                            FROM Automovel 
                            WHERE (@Modelo IS NULL OR Modelo LIKE @Modelo) 
                              AND (@Marca IS NULL OR Marca LIKE @Marca)";
                        cmd.Parameters.AddWithValue("@Modelo", string.IsNullOrWhiteSpace(textModelo.Text) ? (object)DBNull.Value : "%" + textModelo.Text.Trim() + "%");
                        cmd.Parameters.AddWithValue("@Marca", string.IsNullOrWhiteSpace(textMarca.Text) ? (object)DBNull.Value : "%" + textMarca.Text.Trim() + "%");
                    }

                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        bool any = false;
                        while (rdr.Read())
                        {
                            any = true;
                            var id = rdr["Id"];
                            var modelo = rdr["Modelo"]?.ToString();
                            var marca = rdr["Marca"]?.ToString();
                            var valor = FormatCurrency(rdr["Valor"]);
                            var ano = rdr["Ano"]?.ToString();
                            listBox1.Items.Add($"{id} - {modelo} - {marca} - R$ {valor} - {ano}");
                        }

                        if (!any)
                            MessageBox.Show("Nenhum registro encontrado.", "Pesquisar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro na pesquisa", "Não foi possível executar a pesquisa.", ex);
            }
        }

        // Editar (carrega dados selecionados para os TextBox)
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para editar.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            selectedId = int.Parse(item.Split('-')[0].Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Automovel WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", selectedId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textModelo.Text = reader["Modelo"].ToString();
                        textMarca.Text = reader["Marca"].ToString();
                        textValor.Text = reader["Valor"].ToString();
                        textAno.Text = reader["Ano"].ToString();
                        textCodigo.Text = reader["Id"].ToString();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao carregar", "Não foi possível carregar os dados para edição.", ex);
            }
        }

        // Confirmar edição
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Nenhum item selecionado para editar.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Automovel SET Modelo=@Modelo, Marca=@Marca, Valor=@Valor, Ano=@Ano WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Modelo", textModelo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Marca", textMarca.Text.Trim());
                    cmd.Parameters.AddWithValue("@Valor", decimal.Parse(textValor.Text));
                    cmd.Parameters.AddWithValue("@Ano", int.Parse(textAno.Text));
                    cmd.Parameters.AddWithValue("@Id", selectedId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                LimparCampos();
                CarregarDados();
                selectedId = -1;
                MessageBox.Show("Automóvel atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao atualizar", "Não foi possível atualizar o automóvel.", ex);
            }
        }

        // Deletar
        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para deletar.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            int id = int.Parse(item.Split('-')[0].Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Automovel WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                CarregarDados();
                LimparCampos();
                MessageBox.Show("Automóvel deletado com sucesso!");
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao deletar", "Não foi possível deletar o automóvel.", ex);
            }
        }

        // Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            selectedId = -1;
            MessageBox.Show("Operação cancelada.");
        }

        // Voltar
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close(); // Fecha o Form4
        }

        // Carrega todos os dados no ListBox
        private void CarregarDados()
        {
            listBox1.Items.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Automovel";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listBox1.Items.Add($"{reader["Id"]} - {reader["Modelo"]} - {reader["Marca"]} - R$ {FormatCurrency(reader["Valor"])} - {reader["Ano"]}");
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao carregar dados", "Não foi possível carregar os dados.", ex);
            }
        }

        // Limpa os campos de texto
        private void LimparCampos()
        {
            textCodigo.Text = "";
            textModelo.Text = "";
            textMarca.Text = "";
            textValor.Text = "";
            textAno.Text = "";
        }

        // Formatação de moeda
        private string FormatCurrency(object valor)
        {
            if (valor == null) return "0,00";
            return Convert.ToDecimal(valor).ToString("N2");
        }

        // Exibir erros de forma padronizada
        private void MostrarErro(string titulo, string mensagem, Exception ex)
        {
            MessageBox.Show($"{mensagem}\n\nErro: {ex.Message}", titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
