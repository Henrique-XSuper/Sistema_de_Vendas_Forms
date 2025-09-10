using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form6 : Form
    {
        private string connectionString = @"Server=OSA0716282W11-1\SQLEXPRESS;Database=projeto;Trusted_Connection=True;";

        public Form6()
        {
            InitializeComponent();
            LoadFornecedores();
            SetControlsInitialState();
        }

        private void SetControlsInitialState()
        {
            textFornecedor.Enabled = true;
            textProduto.Enabled = true;
            textTipo.Enabled = true;
            textPreco.Enabled = true;
            textGmail.Enabled = true;
            textTelefone.Enabled = true;
            textRecebimento.Enabled = true;
            textCodigo.Enabled = true; // Habilitado para pesquisa

            btnConfirmar.Enabled = false;
            btnCancelar.Enabled = true;
            btnCadastra.Enabled = true;
            btnEditar.Enabled = false;
            btnDeletar.Enabled = false;
        }

        private void LoadFornecedores()
        {
            listBox1.Items.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT Id, Fornecedor, Produto, TipoProduto, PrecoBruto, Gmail, Telefone, DataRecebimento 
                                 FROM FornecedoresProdutos 
                                 ORDER BY Id";

                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string item = $"ID: {reader["Id"]} - Fornecedor: {reader["Fornecedor"]} - Produto: {reader["Produto"]} - " +
                                      $"Tipo: {reader["TipoProduto"]} - Preço: {Convert.ToDecimal(reader["PrecoBruto"]):C} - " +
                                      $"Data: {Convert.ToDateTime(reader["DataRecebimento"]):d}";
                        listBox1.Items.Add(item);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar fornecedores: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearFields()
        {
            textCodigo.Text = "";
            textFornecedor.Text = "";
            textProduto.Text = "";
            textTipo.Text = "";
            textPreco.Text = "";
            textGmail.Text = "";
            textTelefone.Text = "";
            textRecebimento.Text = "";
        }

        private void EnableFields(bool enable)
        {
            textFornecedor.Enabled = enable;
            textProduto.Enabled = enable;
            textTipo.Enabled = enable;
            textPreco.Enabled = enable;
            textGmail.Enabled = enable;
            textTelefone.Enabled = enable;
            textRecebimento.Enabled = enable;
        }

        private void btnCadastra_Click(object sender, EventArgs e)
        {
            ClearFields();
            EnableFields(true);

            btnConfirmar.Enabled = true;
            btnCancelar.Enabled = true;
            btnCadastra.Enabled = false;
            btnEditar.Enabled = false;
            btnDeletar.Enabled = false;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textFornecedor.Text) || string.IsNullOrEmpty(textProduto.Text) ||
                string.IsNullOrEmpty(textTipo.Text) || !decimal.TryParse(textPreco.Text, out decimal precoBruto) ||
                string.IsNullOrEmpty(textGmail.Text) || string.IsNullOrEmpty(textTelefone.Text) ||
                string.IsNullOrEmpty(textRecebimento.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO FornecedoresProdutos (Fornecedor, Produto, TipoProduto, PrecoBruto, Gmail, Telefone, DataRecebimento) 
                                 VALUES (@Fornecedor, @Produto, @TipoProduto, @PrecoBruto, @Gmail, @Telefone, @DataRecebimento)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Fornecedor", textFornecedor.Text);
                cmd.Parameters.AddWithValue("@Produto", textProduto.Text);
                cmd.Parameters.AddWithValue("@TipoProduto", textTipo.Text);
                cmd.Parameters.AddWithValue("@PrecoBruto", precoBruto);
                cmd.Parameters.AddWithValue("@Gmail", textGmail.Text);
                cmd.Parameters.AddWithValue("@Telefone", textTelefone.Text);
                cmd.Parameters.AddWithValue("@DataRecebimento", DateTime.Parse(textRecebimento.Text));

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Fornecedor cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadFornecedores();
                    ClearFields();
                    SetControlsInitialState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao cadastrar fornecedor: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetControlsInitialState();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textCodigo.Text))
            {
                MessageBox.Show("Selecione um fornecedor para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textFornecedor.Text) || string.IsNullOrEmpty(textProduto.Text) ||
                string.IsNullOrEmpty(textTipo.Text) || !decimal.TryParse(textPreco.Text, out decimal precoBruto) ||
                string.IsNullOrEmpty(textGmail.Text) || string.IsNullOrEmpty(textTelefone.Text) ||
                string.IsNullOrEmpty(textRecebimento.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE FornecedoresProdutos SET 
                                    Fornecedor = @Fornecedor, 
                                    Produto = @Produto, 
                                    TipoProduto = @TipoProduto, 
                                    PrecoBruto = @PrecoBruto, 
                                    Gmail = @Gmail, 
                                    Telefone = @Telefone, 
                                    DataRecebimento = @DataRecebimento 
                                 WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", int.Parse(textCodigo.Text));
                cmd.Parameters.AddWithValue("@Fornecedor", textFornecedor.Text);
                cmd.Parameters.AddWithValue("@Produto", textProduto.Text);
                cmd.Parameters.AddWithValue("@TipoProduto", textTipo.Text);
                cmd.Parameters.AddWithValue("@PrecoBruto", precoBruto);
                cmd.Parameters.AddWithValue("@Gmail", textGmail.Text);
                cmd.Parameters.AddWithValue("@Telefone", textTelefone.Text);
                cmd.Parameters.AddWithValue("@DataRecebimento", DateTime.Parse(textRecebimento.Text));

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Fornecedor editado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadFornecedores();
                    ClearFields();
                    SetControlsInitialState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao editar fornecedor: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textCodigo.Text))
            {
                MessageBox.Show("Selecione um fornecedor para deletar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Tem certeza que deseja deletar este fornecedor?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM FornecedoresProdutos WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", int.Parse(textCodigo.Text));

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Fornecedor deletado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadFornecedores();
                        ClearFields();
                        SetControlsInitialState();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao deletar fornecedor: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string query = "SELECT Id, Fornecedor, Produto, TipoProduto, PrecoBruto, Gmail, Telefone, DataRecebimento FROM FornecedoresProdutos WHERE 1=1";
            bool hasCondition = false;

            if (!string.IsNullOrEmpty(textCodigo.Text))
            {
                query += " AND Id = @Id";
                hasCondition = true;
            }

            if (!string.IsNullOrEmpty(textFornecedor.Text))
            {
                query += " AND Fornecedor LIKE @Fornecedor";
                hasCondition = true;
            }

            if (!string.IsNullOrEmpty(textProduto.Text))
            {
                query += " AND Produto LIKE @Produto";
                hasCondition = true;
            }

            if (!string.IsNullOrEmpty(textTipo.Text))
            {
                query += " AND TipoProduto LIKE @TipoProduto";
                hasCondition = true;
            }

            if (!string.IsNullOrEmpty(textGmail.Text))
            {
                query += " AND Gmail LIKE @Gmail";
                hasCondition = true;
            }

            if (!string.IsNullOrEmpty(textTelefone.Text))
            {
                query += " AND Telefone LIKE @Telefone";
                hasCondition = true;
            }

            if (!string.IsNullOrEmpty(textRecebimento.Text))
            {
                query += " AND DataRecebimento = @DataRecebimento";
                hasCondition = true;
            }

            if (!hasCondition)
            {
                MessageBox.Show("Por favor, preencha pelo menos um campo para pesquisa.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(textCodigo.Text))
                    cmd.Parameters.AddWithValue("@Id", int.Parse(textCodigo.Text));
                if (!string.IsNullOrEmpty(textFornecedor.Text))
                    cmd.Parameters.AddWithValue("@Fornecedor", "%" + textFornecedor.Text + "%");
                if (!string.IsNullOrEmpty(textProduto.Text))
                    cmd.Parameters.AddWithValue("@Produto", "%" + textProduto.Text + "%");
                if (!string.IsNullOrEmpty(textTipo.Text))
                    cmd.Parameters.AddWithValue("@TipoProduto", "%" + textTipo.Text + "%");
                if (!string.IsNullOrEmpty(textGmail.Text))
                    cmd.Parameters.AddWithValue("@Gmail", "%" + textGmail.Text + "%");
                if (!string.IsNullOrEmpty(textTelefone.Text))
                    cmd.Parameters.AddWithValue("@Telefone", "%" + textTelefone.Text + "%");
                if (!string.IsNullOrEmpty(textRecebimento.Text))
                    cmd.Parameters.AddWithValue("@DataRecebimento", DateTime.Parse(textRecebimento.Text));

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string item = $"ID: {reader["Id"]} - Fornecedor: {reader["Fornecedor"]} - Produto: {reader["Produto"]} - " +
                                      $"Tipo: {reader["TipoProduto"]} - Preço: {Convert.ToDecimal(reader["PrecoBruto"]):C} - " +
                                      $"Data: {Convert.ToDateTime(reader["DataRecebimento"]):d}";
                        listBox1.Items.Add(item);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao pesquisar fornecedores: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            string selecionado = listBox1.SelectedItem.ToString();
            int id = int.Parse(selecionado.Split('-')[0].Replace("ID: ", "").Trim());

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM FornecedoresProdutos WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textCodigo.Text = reader["Id"].ToString();
                        textFornecedor.Text = reader["Fornecedor"].ToString();
                        textProduto.Text = reader["Produto"].ToString();
                        textTipo.Text = reader["TipoProduto"].ToString();
                        textPreco.Text = reader["PrecoBruto"].ToString();
                        textGmail.Text = reader["Gmail"].ToString();
                        textTelefone.Text = reader["Telefone"].ToString();
                        textRecebimento.Text = Convert.ToDateTime(reader["DataRecebimento"]).ToString("yyyy-MM-dd");

                        EnableFields(true);
                        btnConfirmar.Enabled = false;
                        btnEditar.Enabled = true;
                        btnDeletar.Enabled = true;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar fornecedor: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}