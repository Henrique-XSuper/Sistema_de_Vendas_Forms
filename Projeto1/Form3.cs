using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form3 : Form
    {
        // Ajuste se necessário (este usa Windows Authentication)
        private string connectionString = @"Server=OSA0716282W11-1\SQLEXPRESS;Database=projeto;Trusted_Connection=True;";

        private int selectedId = -1;

        public Form3()
        {
            InitializeComponent();

            // Garante que os botões estarão conectados aos handlers mesmo se o Designer não tiver feito isso.
            btnCadastra.Click += btnCadastra_Click;
            btnPesquisar.Click += btnPesquisar_Click;
            btnEditar.Click += btnEditar_Click;
            btnConfirmar.Click += btnConfirmar_Click;
            btnDeletar.Click += btnDeletar_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnVoltar.Click += btnVoltar_Click;
            listBox1.DoubleClick += listBox1_DoubleClick; // opcional: editar ao dar 2 cliques

            CarregarImoveis();
        }

        #region Utilitários
        private void MostrarErro(string msg, Exception ex = null)
        {
            if (ex == null)
                MessageBox.Show(msg, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show(msg + Environment.NewLine + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LimparCampos()
        {
            textCodigo.Clear();
            textEndereco.Clear();
            textTipo.Clear();
            textValor.Clear();
            selectedId = -1;
        }

        // Tenta converter string para decimal com suporte a vírgula/ponto
        private bool TryParseDecimalFlexible(string s, out decimal value)
        {
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
                return true;
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return true;

            // tenta limpar possíveis milhares e normalizar
            string alt = s?.Replace(".", "").Replace(",", ".") ?? "";
            return decimal.TryParse(alt, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private string ItemToDisplay(IDataRecord record)
        {
            decimal val = record.GetDecimal(record.GetOrdinal("Valor"));
            return $"{record["Id"]} - {record["Endereco"]} | {record["Tipo"]} | R$ {val:N2}";
        }
        #endregion

        #region Carregar / Pesquisar
        private void CarregarImoveis()
        {
            listBox1.Items.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Endereco, Tipo, Valor FROM Imovel ORDER BY Id", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add(ItemToDisplay(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao carregar imóveis.", ex);
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    var sql = "SELECT Id, Endereco, Tipo, Valor FROM Imovel WHERE 1=1";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (!string.IsNullOrWhiteSpace(textCodigo.Text))
                    {
                        sql += " AND Id = @Id";
                        if (int.TryParse(textCodigo.Text, out int idVal))
                            cmd.Parameters.AddWithValue("@Id", idVal);
                        else
                        {
                            MostrarErro("Código inválido.");
                            return;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(textEndereco.Text))
                    {
                        sql += " AND Endereco LIKE @Endereco";
                        cmd.Parameters.AddWithValue("@Endereco", "%" + textEndereco.Text.Trim() + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(textTipo.Text))
                    {
                        sql += " AND Tipo LIKE @Tipo";
                        cmd.Parameters.AddWithValue("@Tipo", "%" + textTipo.Text.Trim() + "%");
                    }

                    cmd.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool any = false;
                        while (reader.Read())
                        {
                            any = true;
                            listBox1.Items.Add(ItemToDisplay(reader));
                        }

                        if (!any)
                            MessageBox.Show("Nenhum registro encontrado.", "Pesquisar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro na pesquisa.", ex);
            }
        }
        #endregion

        #region Cadastrar / Editar / Confirmar / Deletar / Cancelar
        private void btnCadastra_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEndereco.Text))
            {
                MostrarErro("Informe o endereço.");
                return;
            }

            if (!TryParseDecimalFlexible(textValor.Text, out decimal valor))
            {
                MostrarErro("Valor inválido.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Imovel (Endereco, Tipo, Valor) VALUES (@Endereco, @Tipo, @Valor)", conn))
                {
                    cmd.Parameters.AddWithValue("@Endereco", textEndereco.Text.Trim());
                    cmd.Parameters.AddWithValue("@Tipo", textTipo.Text.Trim());

                    var pValor = new SqlParameter("@Valor", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = valor };
                    cmd.Parameters.Add(pValor);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Imóvel cadastrado com sucesso!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CarregarImoveis();
                        LimparCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao cadastrar imóvel.", ex);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MostrarErro("Selecione um imóvel na lista para editar.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            string idStr = item.Split(new[] { " - " }, StringSplitOptions.None)[0];
            if (!int.TryParse(idStr, out int id))
            {
                MostrarErro("ID inválido do item selecionado.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Endereco, Tipo, Valor FROM Imovel WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            selectedId = id;
                            textCodigo.Text = reader["Id"].ToString();
                            textEndereco.Text = reader["Endereco"].ToString();
                            textTipo.Text = reader["Tipo"].ToString();
                            textValor.Text = Convert.ToDecimal(reader["Valor"]).ToString("N2", CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            MostrarErro("Registro não encontrado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao carregar dados para edição.", ex);
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MostrarErro("Nenhum imóvel selecionado para confirmar edição.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textEndereco.Text))
            {
                MostrarErro("Informe o endereço.");
                return;
            }

            if (!TryParseDecimalFlexible(textValor.Text, out decimal valor))
            {
                MostrarErro("Valor inválido.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("UPDATE Imovel SET Endereco = @Endereco, Tipo = @Tipo, Valor = @Valor WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Endereco", textEndereco.Text.Trim());
                    cmd.Parameters.AddWithValue("@Tipo", textTipo.Text.Trim());

                    var pValor = new SqlParameter("@Valor", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = valor };
                    cmd.Parameters.Add(pValor);

                    cmd.Parameters.AddWithValue("@Id", selectedId);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Imóvel atualizado com sucesso!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CarregarImoveis();
                        LimparCampos();
                    }
                    else
                    {
                        MostrarErro("Nenhuma linha atualizada.");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao atualizar imóvel.", ex);
            }
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MostrarErro("Selecione um imóvel para deletar.");
                return;
            }

            string item = listBox1.SelectedItem.ToString();
            string idStr = item.Split(new[] { " - " }, StringSplitOptions.None)[0];
            if (!int.TryParse(idStr, out int id))
            {
                MostrarErro("ID inválido do item selecionado.");
                return;
            }

            var resp = MessageBox.Show("Deseja realmente deletar este imóvel?", "Confirmar exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Imovel WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Imóvel deletado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CarregarImoveis();
                        LimparCampos();
                    }
                    else
                    {
                        MostrarErro("Nenhuma linha deletada.");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErro("Erro ao deletar imóvel.", ex);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        // Double click na lista abre edição (opcional)
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            btnEditar_Click(sender, e);
        }
    }
}
