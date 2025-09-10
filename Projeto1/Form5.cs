using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form5 : Form
    {
        private string connectionString = @"Server=OSA0716282W11-1\SQLEXPRESS;Database=projeto;Trusted_Connection=True;";
        private int vendaId = 0; // Usado para rastrear a venda selecionada

        public Form5()
        {
            InitializeComponent();
            LoadVendas();
            SetControlsInitialState();
        }

        private void SetControlsInitialState()
        {
            textCodigo.Enabled = false;
            textMarca.Enabled = false;
            textModelo.Enabled = false;
            textValor.Enabled = false;
            textAno.Enabled = false;

            btnConfirmar.Enabled = false;
            btnCancelar.Enabled = true;
            btnCadastra.Enabled = true;
            btnEditar.Enabled = false;
            btnDeletar.Enabled = false;
            btnVerificarAutomovel.Enabled = true;
            btnVoltar.Enabled = true;
        }

        private void LoadVendas()
        {
            listBox1.Items.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT v.Id, v.Marca, v.Modelo, v.Valor, v.DataVenda, 
                                a.Ano 
                                FROM Venda v 
                                INNER JOIN Automovel a ON v.IdAutomovel = a.Id 
                                ORDER BY v.Id";

                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string item = $"ID: {reader["Id"]} - {reader["Marca"]} {reader["Modelo"]} - " +
                                     $"Valor: {Convert.ToDecimal(reader["Valor"]):C} - " +
                                     $"Data: {Convert.ToDateTime(reader["DataVenda"]):d} - " +
                                     $"Ano: {reader["Ano"]}";
                        listBox1.Items.Add(item);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar vendas: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearFields()
        {
            textCodigo.Text = "";
            textMarca.Text = "";
            textModelo.Text = "";
            textValor.Text = "";
            textAno.Text = "";
            textCliente.Text = "";
            textVendedor.Text = "";
        }

        private void EnableFields(bool enable)
        {
            textMarca.Enabled = enable;
            textModelo.Enabled = enable;
            textValor.Enabled = enable;
            textAno.Enabled = enable;
            textCliente.Enabled = enable;
            textVendedor.Enabled = enable;
        }

        private void btnCadastra_Click(object sender, EventArgs e)
        {
            ClearFields();
            EnableFields(true);
            textCodigo.Text = "(novo)";
            textMarca.Focus();

            btnConfirmar.Enabled = true;
            btnCancelar.Enabled = true;
            btnCadastra.Enabled = false;
            btnEditar.Enabled = false;
            btnDeletar.Enabled = false;
            btnVerificarAutomovel.Enabled = false;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textCodigo.Text) || textCodigo.Text == "(novo)")
            {
                MessageBox.Show("Selecione uma venda para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EnableFields(true);
            textMarca.Focus();

            btnConfirmar.Enabled = true;
            btnCancelar.Enabled = true;
            btnCadastra.Enabled = false;
            btnEditar.Enabled = false;
            btnDeletar.Enabled = false;
            btnVerificarAutomovel.Enabled = false;

            MessageBox.Show("Modo edição ativado. Faça as alterações necessárias e clique em Confirmar.", "Edição", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnVerificarAutomovel_Click(object sender, EventArgs e)
        {
            EnableFields(true);

            if (string.IsNullOrEmpty(textMarca.Text) || string.IsNullOrEmpty(textModelo.Text) || string.IsNullOrEmpty(textAno.Text))
            {
                MessageBox.Show("Preencha Marca, Modelo e Ano para verificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textAno.Text, out int ano))
            {
                MessageBox.Show("Ano deve ser um número válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int? idAutomovel = GetAutomovelId();

            if (idAutomovel != null)
            {
                MessageBox.Show($"✅ Automóvel encontrado! ID: {idAutomovel}\nPode prosseguir com a venda.",
                              "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("❌ Automóvel não encontrado!\nCadastre-o primeiro na tabela Automovel.",
                              "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int? GetAutomovelId()
        {
            if (string.IsNullOrEmpty(textMarca.Text) || string.IsNullOrEmpty(textModelo.Text) || string.IsNullOrEmpty(textAno.Text))
            {
                return null;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Id FROM Automovel WHERE Marca = @Marca AND Modelo = @Modelo AND Ano = @Ano";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Marca", textMarca.Text);
                cmd.Parameters.AddWithValue("@Modelo", textModelo.Text);
                cmd.Parameters.AddWithValue("@Ano", int.Parse(textAno.Text));

                con.Open();
                object result = cmd.ExecuteScalar();

                return result != null ? (int)result : (int?)null;
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (textCodigo.Text == "(novo)")
            {
                InsertVenda();
            }
            else
            {
                UpdateVenda();
            }

            LoadVendas();
            ClearFields();
            SetControlsInitialState();
        }

        private void InsertVenda()
        {
            if (string.IsNullOrEmpty(textMarca.Text) || string.IsNullOrEmpty(textModelo.Text) ||
                !decimal.TryParse(textValor.Text, out decimal valor) || !int.TryParse(textAno.Text, out int ano) ||
                string.IsNullOrEmpty(textCliente.Text) || string.IsNullOrEmpty(textVendedor.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int? idAutomovel = GetAutomovelId();

                if (idAutomovel == null)
                {
                    MessageBox.Show("❌ ERRO: Este automóvel não está cadastrado!\n" +
                                  "Primeiro cadastre o automóvel na tabela Automovel antes de registrar a venda.",
                                  "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Venda (IdAutomovel, IdCliente, Vendedor, Marca, Modelo, DataVenda, Valor) 
                                   VALUES (@IdAutomovel, @IdCliente, @Vendedor, @Marca, @Modelo, @DataVenda, @Valor)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdAutomovel", idAutomovel);
                    cmd.Parameters.AddWithValue("@IdCliente", textCliente.Text); // Adicionando o ID do cliente
                    cmd.Parameters.AddWithValue("@Vendedor", textVendedor.Text);
                    cmd.Parameters.AddWithValue("@Marca", textMarca.Text);
                    cmd.Parameters.AddWithValue("@Modelo", textModelo.Text);
                    cmd.Parameters.AddWithValue("@DataVenda", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Valor", valor);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Venda registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao registrar venda: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateVenda()
        {
            if (!int.TryParse(textCodigo.Text, out int idVenda))
            {
                MessageBox.Show("Código inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textMarca.Text) || string.IsNullOrEmpty(textModelo.Text) ||
                !decimal.TryParse(textValor.Text, out decimal valor) || !int.TryParse(textAno.Text, out int ano) ||
                string.IsNullOrEmpty(textCliente.Text) || string.IsNullOrEmpty(textVendedor.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int? idAutomovel = GetAutomovelId();

                if (idAutomovel == null)
                {
                    MessageBox.Show("❌ ERRO: Este automóvel não está cadastrado!\n" +
                                  "Primeiro cadastre o automóvel na tabela Automovel antes de atualizar a venda.",
                                  "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Venda SET IdAutomovel = @IdAutomovel, IdCliente = @IdCliente, Vendedor = @Vendedor, 
                                   Marca = @Marca, Modelo = @Modelo, Valor = @Valor WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", idVenda);
                    cmd.Parameters.AddWithValue("@IdAutomovel", idAutomovel);
                    cmd.Parameters.AddWithValue("@IdCliente", textCliente.Text);
                    cmd.Parameters.AddWithValue("@Vendedor", textVendedor.Text);
                    cmd.Parameters.AddWithValue("@Marca", textMarca.Text);
                    cmd.Parameters.AddWithValue("@Modelo", textModelo.Text);
                    cmd.Parameters.AddWithValue("@Valor", valor);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Venda atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar venda: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetControlsInitialState();
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textCodigo.Text) || textCodigo.Text == "(novo)")
            {
                MessageBox.Show("Selecione uma venda para deletar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textCodigo.Text, out int idVenda))
            {
                MessageBox.Show("Código inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirmResult = MessageBox.Show($"Tem certeza que deseja deletar a venda ID: {idVenda}?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        string query = "DELETE FROM Venda WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Id", idVenda);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Venda deletada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadVendas();
                        ClearFields();
                        SetControlsInitialState();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao deletar venda: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            string searchTerm = textModelo.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Por favor, insira um modelo para pesquisar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            listBox1.Items.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT v.Id, v.Marca, v.Modelo, v.Valor, v.DataVenda, 
                                a.Ano 
                                FROM Venda v 
                                INNER JOIN Automovel a ON v.IdAutomovel = a.Id 
                                WHERE v.Modelo LIKE @Modelo 
                                ORDER BY v.Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Modelo", "%" + searchTerm + "%");

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string item = $"ID: {reader["Id"]} - {reader["Marca"]} {reader["Modelo"]} - " +
                                     $"Valor: {Convert.ToDecimal(reader["Valor"]):C} - " +
                                     $"Data: {Convert.ToDateTime(reader["DataVenda"]):d} - " +
                                     $"Ano: {reader["Ano"]}";
                        listBox1.Items.Add(item);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao pesquisar vendas: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            string selecionado = listBox1.SelectedItem.ToString();

            try
            {
                int idStart = selecionado.IndexOf("ID: ") + 4;
                int idEnd = selecionado.IndexOf(" - ");
                string idStr = selecionado.Substring(idStart, idEnd - idStart);

                if (!int.TryParse(idStr, out int id))
                {
                    MessageBox.Show("ID inválido no item selecionado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                textCodigo.Text = id.ToString();

                // Extrai os dados do texto formatado
                string restante = selecionado.Substring(idEnd + 3);
                string[] partes = restante.Split('-');

                textMarca.Text = partes[0].Split(' ')[0].Trim();
                textModelo.Text = partes[0].Split(' ')[1].Trim();

                string valorStr = partes[1].Replace("Valor:", "").Replace("R$", "").Trim();
                textValor.Text = valorStr;

                string anoStr = partes[3].Replace("Ano:", "").Trim();
                textAno.Text = anoStr;

                EnableFields(false);
                btnConfirmar.Enabled = false;
                btnDeletar.Enabled = true;
                btnEditar.Enabled = true;
                btnVerificarAutomovel.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados da venda selecionada: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}