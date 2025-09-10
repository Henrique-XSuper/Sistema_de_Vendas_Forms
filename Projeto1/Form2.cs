using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form2 : Form
    {
        // String de conexão
        string connectionString = @"Data Source=OSA0716282W11-1\SQLEXPRESS;Initial Catalog=projeto;Integrated Security=True";

        int vendedorId = 0; // usado para edição/deleção

        public Form2()
        {
            InitializeComponent();

            // Associando eventos
            btnCadastra.Click += BtnCadastra_Click;
            btnPesquisar.Click += BtnPesquisar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnConfirmar.Click += BtnConfirmar_Click;
            btnDeletar.Click += BtnDeletar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            btnVoltar.Click += BtnVoltar_Click;

            CarregarVendedores();
        }

        private void BtnCadastra_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Vendedor (Vendedor, CPF, RG, Gmail, Telefone)
                                 VALUES (@Vendedor, @CPF, @RG, @Gmail, @Telefone)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Vendedor", textVendedor.Text);
                cmd.Parameters.AddWithValue("@CPF", textCPF.Text);
                cmd.Parameters.AddWithValue("@RG", textRG.Text);
                cmd.Parameters.AddWithValue("@Gmail", textGmail.Text);
                cmd.Parameters.AddWithValue("@Telefone", textTelefone.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Vendedor cadastrado com sucesso!");
                LimparCampos();
                CarregarVendedores();
            }
        }

        private void BtnPesquisar_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            if (string.IsNullOrWhiteSpace(textCodigo.Text))
            {
                MessageBox.Show("Digite o código do vendedor para pesquisar.");
                return;
            }

            int idPesquisa;
            if (!int.TryParse(textCodigo.Text, out idPesquisa))
            {
                MessageBox.Show("Código inválido.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Vendedor WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", idPesquisa);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    vendedorId = idPesquisa;
                    textCodigo.Text = reader["Id"].ToString();
                    textVendedor.Text = reader["Vendedor"].ToString(); // Alterado para "Vendedor"
                    textCPF.Text = reader["CPF"].ToString();
                    textRG.Text = reader["RG"].ToString();
                    textGmail.Text = reader["Gmail"].ToString();
                    textTelefone.Text = reader["Telefone"].ToString();

                    listBox1.Items.Add($"{reader["Id"]} - {reader["Vendedor"]} - {reader["CPF"]} - {reader["Telefone"]}");
                }
                else
                {
                    MessageBox.Show("Vendedor não encontrado.");
                    vendedorId = 0;
                }

                conn.Close();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione um vendedor na lista.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            vendedorId = int.Parse(item.Split('-')[0].Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Vendedor WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", vendedorId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    textCodigo.Text = reader["Id"].ToString();
                    textVendedor.Text = reader["Vendedor"].ToString(); // Alterado para "Vendedor"
                    textCPF.Text = reader["CPF"].ToString();
                    textRG.Text = reader["RG"].ToString();
                    textGmail.Text = reader["Gmail"].ToString();
                    textTelefone.Text = reader["Telefone"].ToString();
                }

                conn.Close();
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (vendedorId == 0)
            {
                MessageBox.Show("Nenhum vendedor selecionado para edição.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Vendedor SET Vendedor=@Vendedor, CPF=@CPF, RG=@RG, Gmail=@Gmail, Telefone=@Telefone
                                 WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", vendedorId);
                cmd.Parameters.AddWithValue("@Vendedor", textVendedor.Text); // Alterado para "Vendedor"
                cmd.Parameters.AddWithValue("@CPF", textCPF.Text);
                cmd.Parameters.AddWithValue("@RG", textRG.Text);
                cmd.Parameters.AddWithValue("@Gmail", textGmail.Text);
                cmd.Parameters.AddWithValue("@Telefone", textTelefone.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Vendedor atualizado com sucesso!");
                LimparCampos();
                CarregarVendedores();
                vendedorId = 0;
            }
        }

        private void BtnDeletar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione um vendedor para deletar.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            int id = int.Parse(item.Split('-')[0].Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Vendedor WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            MessageBox.Show("Vendedor deletado com sucesso!");
            LimparCampos();
            CarregarVendedores();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            vendedorId = 0;
        }

        private void BtnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- Métodos auxiliares ---
        private void CarregarVendedores()
        {
            listBox1.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Vendedor";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listBox1.Items.Add($"{reader["Id"]} - {reader["Vendedor"]} - {reader["CPF"]} - {reader["Telefone"]}");
                }

                conn.Close();
            }
        }

        private void LimparCampos()
        {
            textCodigo.Clear();
            textVendedor.Clear();
            textCPF.Clear();
            textRG.Clear();
            textGmail.Clear();
            textTelefone.Clear();
        }
    }
}