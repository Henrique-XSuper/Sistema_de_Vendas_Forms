using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form1 : Form
    {
        // String de conexão
        string connectionString = @"Data Source=OSA0716282W11-1\SQLEXPRESS;Initial Catalog=projeto;Integrated Security=True";

        int clienteId = 0; // Usado para edição/deleção

        public Form1()
        {
            InitializeComponent();

            // Eventos dos botões
            btnCadastra.Click += BtnCadastra_Click;
            btnPesquisar.Click += BtnPesquisar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnConfirmar.Click += BtnConfirmar_Click;
            btnDeletar.Click += BtnDeletar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            btnVoltar.Click += BtnVoltar_Click;

            CarregarClientes();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CarregarClientes();
        }

        // --- CRUD ---

        private void BtnCadastra_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Cliente (Cliente, CPF, RG, CEP, DataNascimento, RendaMensal, Gmail, Telefone)
                                 VALUES (@Cliente, @CPF, @RG, @CEP, @DataNascimento, @RendaMensal, @Gmail, @Telefone)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Cliente", textCliente.Text);
                cmd.Parameters.AddWithValue("@CPF", textCPF.Text);
                cmd.Parameters.AddWithValue("@RG", textRG.Text);
                cmd.Parameters.AddWithValue("@CEP", textCEP.Text);
                cmd.Parameters.AddWithValue("@DataNascimento", DateTime.Parse(textNascimento.Text));
                cmd.Parameters.AddWithValue("@RendaMensal", decimal.Parse(textRenda.Text));
                cmd.Parameters.AddWithValue("@Gmail", textGmail.Text);
                cmd.Parameters.AddWithValue("@Telefone", textTelefone.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Cliente cadastrado com sucesso!");
                LimparCampos();
                CarregarClientes();
            }
        }

        private void BtnPesquisar_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            if (string.IsNullOrWhiteSpace(textCodigo.Text))
            {
                MessageBox.Show("Digite o código do cliente para pesquisar.");
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
                string query = "SELECT * FROM Cliente WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", idPesquisa);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listBox1.Items.Add($"{reader["Id"]} - {reader["Cliente"]} - {reader["CPF"]} - {reader["Telefone"]}");
                }

                conn.Close();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione um cliente na lista.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            clienteId = int.Parse(item.Split('-')[0].Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Cliente WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", clienteId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    textCodigo.Text = reader["Id"].ToString();
                    textCliente.Text = reader["Cliente"].ToString();
                    textCPF.Text = reader["CPF"].ToString();
                    textRG.Text = reader["RG"].ToString();
                    textCEP.Text = reader["CEP"].ToString();
                    textNascimento.Text = Convert.ToDateTime(reader["DataNascimento"]).ToString("yyyy-MM-dd");
                    textRenda.Text = reader["RendaMensal"].ToString();
                    textGmail.Text = reader["Gmail"].ToString();
                    textTelefone.Text = reader["Telefone"].ToString();
                }

                conn.Close();
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (clienteId == 0)
            {
                MessageBox.Show("Nenhum cliente selecionado para edição.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Cliente SET Cliente=@Cliente, CPF=@CPF, RG=@RG, CEP=@CEP,
                                 DataNascimento=@DataNascimento, RendaMensal=@RendaMensal,
                                 Gmail=@Gmail, Telefone=@Telefone
                                 WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Id", clienteId);
                cmd.Parameters.AddWithValue("@Cliente", textCliente.Text);
                cmd.Parameters.AddWithValue("@CPF", textCPF.Text);
                cmd.Parameters.AddWithValue("@RG", textRG.Text);
                cmd.Parameters.AddWithValue("@CEP", textCEP.Text);
                cmd.Parameters.AddWithValue("@DataNascimento", DateTime.Parse(textNascimento.Text));
                cmd.Parameters.AddWithValue("@RendaMensal", decimal.Parse(textRenda.Text));
                cmd.Parameters.AddWithValue("@Gmail", textGmail.Text);
                cmd.Parameters.AddWithValue("@Telefone", textTelefone.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Cliente atualizado com sucesso!");
                LimparCampos();
                CarregarClientes();
                clienteId = 0;
            }
        }

        private void BtnDeletar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione um cliente para deletar.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            int id = int.Parse(item.Split('-')[0].Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Cliente WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            MessageBox.Show("Cliente deletado com sucesso!");
            LimparCampos();
            CarregarClientes();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            clienteId = 0;
        }

        private void BtnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- Métodos auxiliares ---
        private void CarregarClientes()
        {
            listBox1.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Cliente";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listBox1.Items.Add($"{reader["Id"]} - {reader["Cliente"]} - {reader["CPF"]} - {reader["Telefone"]}");
                }

                conn.Close();
            }
        }

        private void LimparCampos()
        {
            textCodigo.Clear();
            textCliente.Clear();
            textCPF.Clear();
            textRG.Clear();
            textCEP.Clear();
            textNascimento.Clear();
            textRenda.Clear();
            textGmail.Clear();
            textTelefone.Clear();
        }
    }
}
