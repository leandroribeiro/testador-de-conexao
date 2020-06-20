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

            

            BloquearBotao();

            var servidor = txtServidor.Text;
            var porta = txtPorta.Text;
            var bancoDeDados = BancoDeDados;
            var usuario = txtUsuario.Text;
            var senha = txtSenha.Text;

            AdicionarMensagemNoHistorico("Testando Modo 1 ...");

            _cnxStr = $"Data Source={servidor},{porta};Initial Catalog={bancoDeDados};Persist Security Info=True;User Id={usuario};Password={senha};MultipleActiveResultSets=True;App=TesteDeConexao";

            if (!TestaConexao())
            {
                AdicionarMensagemNoHistorico("Testando Modo 2 ...");
                _cnxStr = $"Server={servidor},{porta};Database={bancoDeDados};User Id={usuario};Password={senha}";
                TestaConexao();
            }

            HabilitarBotao();
        }

        private bool TestaConexao()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(_cnxStr))
            {
                try
                {
                    var sqlComm = new SqlCommand("SELECT @@VERSION", conn)
                    {
                        CommandType = CommandType.Text
                    };

                    var da = new SqlDataAdapter {SelectCommand = sqlComm};

                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        AdicionarMensagemNoHistorico("Conectado :)");
                        AdicionarMensagemNoHistorico($"CnxStr {_cnxStr}");
                    }

                    var versao = dt.Rows[0][0].ToString();

                    AdicionarMensagemNoHistorico($"Versão {versao}");

                    return true;
                }
                catch (Exception ex)
                {
                    AdicionarMensagemNoHistorico("Falhou :\\");
                    AdicionarMensagemNoHistorico(ex.Message);

                    return false;
                }
            } // using

        } // function

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
            if (string.IsNullOrWhiteSpace(txtServidor.Text))
            {
                e.Cancel = true;
                txtServidor.Focus();
                errorProvider1.SetError(txtServidor, "Você precisa preencher o nome ou IP do banco de dados");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtServidor, "");
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
            txtServidor.Text = "localhost";
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
