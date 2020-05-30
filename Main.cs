using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace TesteDeConexao
{
    public partial class Main : Form
    {
        private const string BancoDeDados = "master";
        private string _cnxStr;

        public Main()
        {
            InitializeComponent();
        }

        private void btnTestar_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren(ValidationConstraints.Enabled))
                return;

            AdicionarMensagemNoHistorico("Testando...");

            BloquearBotao();

            var servidor = txtBancoDeDados.Text;
            var porta = txtPorta.Text;
            var bancoDeDados = BancoDeDados;
            var usuario = txtUsuario.Text;
            var senha = txtSenha.Text;

            var dt = new DataTable();
            _cnxStr = $"Data Source={servidor},{porta};Initial Catalog={bancoDeDados};Persist Security Info=True;User Id={usuario};Password={senha};MultipleActiveResultSets=True;App=TesteDeConexao";

            using (var conn = new SqlConnection(_cnxStr))
            {

                try
                {
                    var sqlComm = new SqlCommand("SELECT @@VERSION", conn)
                    {
                        CommandType = CommandType.Text
                    };

                    var da = new SqlDataAdapter { SelectCommand = sqlComm };

                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        AdicionarMensagemNoHistorico("Conectado :)");
                        AdicionarMensagemNoHistorico($"CnxStr {_cnxStr}");
                    }

                    var versao = dt.Rows[0][0].ToString();

                    AdicionarMensagemNoHistorico($"Versão {versao}");
                }
                catch (Exception ex)
                {
                    AdicionarMensagemNoHistorico("Falhou :\\");
                    AdicionarMensagemNoHistorico(ex.Message);
                }

            }

            HabilitarBotao();
        }

        private void HabilitarBotao()
        {
            btnTestar.Text = "TESTAR";
            btnTestar.Enabled = true;
            btnTestar.Cursor = Cursors.Hand;
        }

        private void BloquearBotao()
        {
            btnTestar.Text = "AGUARDE...";
            btnTestar.Enabled = false;
            btnTestar.Cursor = Cursors.No;
        }

        private void AdicionarMensagemNoHistorico(string mensagem)
        {
            var texto = $"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} - {mensagem}";

            lstHistorico.Items.Insert(0, texto);
            lstHistorico.SelectedIndex = 0;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtBancoDeDados_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBancoDeDados.Text))
            {
                e.Cancel = true;
                txtBancoDeDados.Focus();
                errorProvider1.SetError(txtBancoDeDados, "Você precisa preencher o nome ou IP do banco de dados");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtBancoDeDados, "");
            }
        }

        private void txtUsuario_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                e.Cancel = true;
                txtUsuario.Focus();
                errorProvider1.SetError(txtUsuario, "Você precisa preencher o nome do usuário");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtUsuario, "");
            }
        }

        private void txtSenha_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                e.Cancel = true;
                txtSenha.Focus();
                errorProvider1.SetError(txtSenha, "Você precisa preencher a senha");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtSenha, "");
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
#if DEBUG
            txtBancoDeDados.Text = "localhost";
            txtUsuario.Text = "sa";
            txtSenha.Text = "Pass@word";
#endif
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var text = lstHistorico.Items.Cast<object>().Aggregate("", (current, item) => current + (item.ToString() + "\r\n"));

            Clipboard.SetText(text);

            MessageBox.Show("Copiado para área de transferência, utilize Ctr + V para colar...");
        }
    }
}
