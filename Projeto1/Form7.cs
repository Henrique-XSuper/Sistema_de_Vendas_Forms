using System;
using System.Windows.Forms;

namespace Projeto1
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();

            // Associando os eventos de clique diretamente no construtor
            clienteToolStripMenuItem.Click += clienteToolStripMenuItem_Click;
            vendaToolStripMenuItem.Click += vendaToolStripMenuItem_Click;
            vendedorToolStripMenuItem.Click += vendedorToolStripMenuItem_Click;
            automovelToolStripMenuItem.Click += automovelToolStripMenuItem_Click;
            imovelToolStripMenuItem.Click += imovelToolStripMenuItem_Click;
            fornecedorToolStripMenuItem.Click += fornecedorToolStripMenuItem_Click; // Adicionando o manipulador para Fornecedor
        }

        private void clienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 formCliente = new Form1();
            formCliente.Show();
        }

        private void vendaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 formVenda = new Form5();
            formVenda.Show();
        }

        private void vendedorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 formVendedor = new Form2();
            formVendedor.Show();
        }

        private void automovelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 formAutomovel = new Form4();
            formAutomovel.Show();
        }

        private void imovelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 formImovel = new Form3();
            formImovel.Show();
        }

        private void fornecedorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 formFornecedor = new Form6();
            formFornecedor.Show(); // Abrindo o Form6 para Fornecedor
        }
    }
}