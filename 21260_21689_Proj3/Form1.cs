using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Media;

/*
    Técnicas de Programação I
    Projeto III
    Alunos: Nicoli Ferreira - 21689
            Samuel de Campos Ferraz - 21260

    Professor: Francisco da Fonseca Rodrigues
 */

namespace _21260_21689_Proj3
{
    public partial class frmJogoForca : Form
    {
        StreamReader arquivoLido;
        VetorDicionario oDicio;
        Dicionario Dicio;
        int posicaoDeInclusao;
        int tempo;
        string nome;
        string str;
        bool jogando;
        SoundPlayer player;
        SoundPlayer playerBack;
        public frmJogoForca()
        {
            InitializeComponent();
        }
        private void Jogo_Forca_Load(object sender, EventArgs e)
        {
            // abertura do arquivo de palavras
            oDicio = new VetorDicionario(100);
            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                arquivoLido = new StreamReader(dlgAbrir.FileName);
                while (!arquivoLido.EndOfStream)
                {
                    Dicio = new Dicionario();
                    Dicio.LerDados(arquivoLido);
                    oDicio.Incluir(Dicio);
                    oDicio.OrdemAlfabetica(ListSortDirection.Ascending);
                    oDicio.ExibirDados(dgvForca);
                }
                arquivoLido.Close();
            }
            else
                Close();      // caso a pessoa não escolhe nenhum, irá fechar o form's

            oDicio.PosicionarNoPrimeiro(); // posiciona no 1o registro a visitação nos dados
            AtualizarTela();

            // prepara as imagens para os botões do cadastro
            int indice = 0;
            toolStrip1.ImageList = imlBotoes;
            foreach (ToolStripItem item in toolStrip1.Items)
                if (item is ToolStripButton) // se não é separador:
                    (item as ToolStripButton).ImageIndex = indice++;

            // começa a tocar o som de fundo do jogo
            playerBack = new SoundPlayer("backgroundSound.wav");
            playerBack.PlayLooping();

            // o timer de horário do computador começa a contar
            tmrHorarioPc.Start();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //botão Início
            btnExcluir.Enabled = btnAlterar.Enabled = btnAdicionar.Enabled = true;
            txtDica.ReadOnly = true;
            txtPalavra.ReadOnly = true;
            btnSalvar.Enabled = false;
            oDicio.PosicionarNoPrimeiro();
            AtualizarTela();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // botão Voltar
            btnExcluir.Enabled = btnAlterar.Enabled = btnAdicionar.Enabled = true;
            txtDica.ReadOnly = true;
            txtPalavra.ReadOnly = true;
            btnSalvar.Enabled = false;
            oDicio.RetrocederPosicao();
            AtualizarTela();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // botão Avançar
            btnExcluir.Enabled = btnAlterar.Enabled = btnAdicionar.Enabled = true;
            txtDica.ReadOnly = true;
            txtPalavra.ReadOnly = true;
            btnSalvar.Enabled = false;
            oDicio.AvancarPosicao();
            AtualizarTela();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            // botão Último
            btnExcluir.Enabled = btnAlterar.Enabled = btnAdicionar.Enabled = true;
            txtDica.ReadOnly = true;
            txtPalavra.ReadOnly = true;
            btnSalvar.Enabled = false;
            oDicio.PosicionarNoUltimo();
            AtualizarTela();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            // botão Adicionar
            oDicio.SituacaoAtual = Situacao.incluindo;
            LimparTela();
            txtPalavra.ReadOnly = false;
            txtPalavra.Focus();
            txtDica.ReadOnly = true;
            btnAlterar.Enabled = btnExcluir.Enabled = false;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            // botão Alterar
            // permitimos ao usuario editar o registro atualmente
            // exibido na tela
            oDicio.SituacaoAtual = Situacao.editando;
            txtDica.ReadOnly = false;
            txtDica.Focus();
            btnSalvar.Enabled = true;
            btnExcluir.Enabled = false;
            txtPalavra.ReadOnly = true;  // não deixamos usuário alterar palavra (chave primária)
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            // botão Salvar
            if (oDicio.SituacaoAtual == Situacao.incluindo)  // só guarda nova palavra no vetor se estiver incluindo
            {
                // criamos objeto com o registro da nova palavra digitada no formulário
                var novoDicio = new Dicionario(txtPalavra.Text.ToUpper(), txtDica.Text); //Passa a palvra em maiúculo

                oDicio.Incluir(novoDicio, posicaoDeInclusao);

                oDicio.SituacaoAtual = Situacao.navegando;  // voltamos ao modo de navegação

                oDicio.PosicaoAtual = posicaoDeInclusao;
                oDicio.OrdemAlfabetica(ListSortDirection.Ascending);
                oDicio.GravarDados(dlgAbrir.FileName);
                oDicio.ExibirDados(dgvForca);
                AtualizarTela();
            }
            else
                if (oDicio.SituacaoAtual == Situacao.editando)
            {
                oDicio[oDicio.PosicaoAtual].Dica = txtDica.Text;
                oDicio.SituacaoAtual = Situacao.navegando;
                oDicio.OrdemAlfabetica(ListSortDirection.Ascending);
                oDicio.GravarDados(dlgAbrir.FileName);
                oDicio.ExibirDados(dgvForca);
            }
            btnExcluir.Enabled = btnAlterar.Enabled = true;
            btnSalvar.Enabled = false;    // desabilita pois a inclusão terminou
            txtPalavra.ReadOnly = true;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            // botão Excluir
            if (MessageBox.Show("Deseja realmente excluir?", "Exclusão",
              MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)     // pergunta ao usuário caso ele queira excluir
                {
                    oDicio.Excluir(oDicio.PosicaoAtual);
                    if (oDicio.PosicaoAtual >= oDicio.Tamanho)
                        oDicio.PosicionarNoUltimo();

                        oDicio.GravarDados(dlgAbrir.FileName);
                        oDicio.ExibirDados(dgvForca);
                        AtualizarTela();
                }
        }

        private void tpCadastro_Enter(object sender, EventArgs e)
        {
            oDicio.ExibirDados(dgvForca);
            frmJogoForca.ActiveForm.Width = 959;
        }
        private void LimparTela()
        {
            txtPalavra.Clear();
            txtDica.Clear();
        }
        private void AtualizarTela()
        {
            if (!oDicio.EstaVazio)
            {
                int indice = oDicio.PosicaoAtual;
                txtPalavra.Text = oDicio[indice].Palavra + "";
                txtDica.Text = oDicio[indice].Dica;
                stlbRegistro.Text = "  Registro: " + (oDicio.PosicaoAtual + 1) + "/" + oDicio.Tamanho;
                TestarBotoes();
            }
        }
        private void TestarBotoes()
        {
            btnInicio.Enabled = true;
            btnVoltar.Enabled = true;
            btnAvancar.Enabled = true;
            btnUltimo.Enabled = true;
            if (oDicio.EstaNoInicio)
            {
                btnInicio.Enabled = false;
                btnVoltar.Enabled = false;
            }

            if (oDicio.EstaNoFim)
            {
                btnAvancar.Enabled = false;
                btnUltimo.Enabled = false;
            }
        }
        private void txtPalavra_Leave(object sender, EventArgs e)
        {
            if (oDicio.SituacaoAtual == Situacao.incluindo ||
            oDicio.SituacaoAtual == Situacao.pesquisando)
                if (txtPalavra.Text == "")
                {
                    MessageBox.Show("Digite uma palavra!");
                    txtPalavra.Focus();
                }
                else  // temos um valor digitado no txtPalavra
                {
                    string palavraProcurada = txtPalavra.Text.ToLower();
                    int posicao;
                    bool achou = oDicio.Existe(palavraProcurada, out posicao);
                    switch (oDicio.SituacaoAtual)
                    {
                        case Situacao.incluindo:
                            if (achou)
                            {
                                MessageBox.Show("Palavra já existe!");
                                oDicio.SituacaoAtual = Situacao.navegando;
                                txtPalavra.Text = oDicio[posicao].Palavra;
                                txtDica.Text = oDicio[posicao].Dica;
                            }
                            else  // a matrícula não existe e podemos incluí-la 
                            {     // incluí-la no índice do vetor interno dados de oDicio
                                txtPalavra.ReadOnly = true;
                                txtDica.ReadOnly = false;
                                txtDica.Focus();
                                btnSalvar.Enabled = true;  // habilita quando é possível incluir
                                posicaoDeInclusao = posicao;  // guarda índice de inclusão em variável global
                            }
                            break;

                        case Situacao.pesquisando:
                            if (achou)
                            {
                                // a variável posicao contém o índice da palavra que se buscou
                                oDicio.PosicaoAtual = posicao;   // reposiciona o índice da posição visitada
                                txtPalavra.Text = oDicio[posicao].Palavra;
                                txtDica.Text = oDicio[posicao].Dica;
                            }
                            else
                            {
                                MessageBox.Show("Palavra digitada não foi encontrada.");
                                oDicio.ExibirDados(dgvForca);
                                AtualizarTela();  // reexibir o registro que aparecia antes de limparmos a tela
                            }
                            oDicio.SituacaoAtual = Situacao.navegando;
                            txtPalavra.ReadOnly = true;
                            break;
                    }
                }
        }

        private void frmJogoForca_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dlgAbrir.FileName != "")
            {
                // Salva palavras no dicionário
                oDicio.GravarDados(dlgAbrir.FileName);
                EnviarParaPortaSerial('I');
            }
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // botão Buscar
            oDicio.SituacaoAtual = Situacao.pesquisando;  // entramos no modo de busca
            LimparTela();
            txtPalavra.ReadOnly = false;
            txtPalavra.Focus();
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // botão Cancelar
            oDicio.SituacaoAtual = Situacao.navegando;
            AtualizarTela();
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            // botão Sair
            Close();
        }
        int posicaoSorteada = -1;
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            // botão Iniciar
            DesabilitarCadastro();      // não permite que a pessoa possa olhar o cadastro enquanto estiver jogando
            nome = txbNome.Text;

            // prepara a interface para o jogo
            btnIniciar.Enabled = false;
            pTeclado.Enabled = true;
            cbxArduino.Enabled = true;
            jogando = true;

            // pega todos os botões do teclado (panel) e permite que o usuário possa clicá-lo
            foreach (var botao in pTeclado.Controls)
            {
                Button b = (Button)botao;
                b.Enabled = true;
            }

            // sorteia a palavra
            var Sorteador = new Random();
            // pega a posição da palavra e procura na classe VetorDicionario
            posicaoSorteada = Sorteador.Next(oDicio.Tamanho);
            // prepara o dgvLetras para pegar o tamanho da palavra e adicionar as colunas adequadas
            dgvLetras.ColumnCount = oDicio[posicaoSorteada].Palavra.Length;
            dgvLetras.Width = 21 * oDicio[posicaoSorteada].Palavra.Length;
            dgvLetras.AutoResizeColumns();
            DesabilitarCadastro();
            Reiniciar();

            // caso o cbxDica for clicado, irá mostrar a dica da posição sorteada e ligar o timer 
            if (cbxDica.Checked)
            {
                lbDica.Text = oDicio[posicaoSorteada].Dica;
                cbxDica.Enabled = false;
                tempo = 60;
                tmrTempo.Enabled = true;
            }
        }
        private void tmrTempo_Tick(object sender, EventArgs e)
        {
            // prepara o label Tempo para pegar os segundos
            lbTempo.Text = tempo.ToString();
            tempo--;
            // quando o tempo for igual a 0, automaticamente é acionado o método Perdeu
            if (tempo < 0)
                Perdeu();
        }

        int erros = 0;
        int pontos = 0;
        int letrasAcertadas = 0;
        private void btnA_Click(object sender, EventArgs e)
        {
            // pega a "letra" de cada botão clicado, e o botão clicado é desativado
            char letra = (sender as Button).Text[0]; 
            (sender as Button).Enabled = false;

            // é mostrado na tela qual letra foi acionada
            lbLetras.Text += $"{letra} ";

            // verifica se o botão que foi clicado, continha a letra correta
            int posicaoLetra = Dicio.IndiceDaLetra(letra, oDicio[posicaoSorteada].Palavra);
            if (posicaoLetra == -1) // se a letra for incorreta
            {
                // é contado os erros e são mostrados na interface do jogo
                erros++;
                lbErros.Text = erros.ToString();
                switch (erros)
                {
                    // de acordo com a quantidade de erros, são acionados os "cases" para cada um;
                    // são enviados caracteres para o arduino;
                    // as imagens de acordo com cada erro, são mostradas na tela
                    case 1:
                        EnviarParaPortaSerial('A');
                        pbCabeça.Visible = true;
                        pbQueixo.Visible = true;
                        led1aceso.Visible = true;
                        break;
                    case 2:
                        EnviarParaPortaSerial('B');
                        pbPeitoral.Visible = true;
                        led2aceso.Visible = true;
                        break;
                    case 3:
                        EnviarParaPortaSerial('C');
                        pbBraçoEsq.Visible = true;
                        led3aceso.Visible = true;
                        break;
                    case 4:
                        EnviarParaPortaSerial('D');
                        pbBraçoDir.Visible = true;
                        led4aceso.Visible = true;
                        break;
                    case 5:
                        EnviarParaPortaSerial('E');
                        pbPerna.Visible = true;
                        led5aceso.Visible = true;
                        break;
                    case 6:
                        EnviarParaPortaSerial('F');
                        pbPeEsq.Visible = true;
                        led6aceso.Visible = true;
                        break;
                    case 7:
                        EnviarParaPortaSerial('G');
                        pbPeDir.Visible = true;
                        led7aceso.Visible = true;
                        break;
                    case 8:
                        EnviarParaPortaSerial('H');
                        pbCabeçaMorto.Visible = true;
                        pbCabeça.Visible = false;
                        pbEnforcado.Visible = true;
                        led8aceso.Visible = true;
                        // como é o limite de erros, automaticamente o método Perdeu é acionado
                        Perdeu();
                        break;
                }
            }
            else
                while (posicaoLetra != -1) 
                {
                    // caso a pessoa acerte a letra, a quantidade de pontos aumenta e é mostrada na interface
                    // o dgvLetras vai se compondo das letras acertadas
                    pontos++;
                    letrasAcertadas++;
                    lbPontos.Text = pontos.ToString();
                    dgvLetras[posicaoLetra, 0].Value = letra;
                    posicaoLetra = Dicio.IndiceDaLetra(letra, oDicio[posicaoSorteada].Palavra);
                }

            if (letrasAcertadas == oDicio[posicaoSorteada].Palavra.Length) // verifica se o jogador já acertou todas as letras
                Ganhou();
        }
        private void Reiniciar() // volta o jogo no modo inicial
        {
            lbExibe.ForeColor = Color.Black;
            lbExibe.Text = "Adivinhe a palavra!";

            // todos os sistemas de contagem são zerados
            letrasAcertadas = erros = pontos = 0;
            for (int i = 0; i < oDicio[posicaoSorteada].Palavra.Length; i++) // limpa o dgvLetras
                dgvLetras[i, 0].Value = null;

            Dicio.LimparAcertos();  // limpa os acertos
            lbLetras.Text = lbDica.Text = lbErros.Text = lbPontos.Text = "";    // limpa todos os label's

            // todas as imagens (menos da forca) ficam invisíveis
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            pbQueixo.Visible = false;
            pbPeitoral.Visible = false;
            pbBraçoEsq.Visible = false;
            pbBraçoDir.Visible = false;
            pbPerna.Visible = false;
            pbPeEsq.Visible = false;
            pbPeDir.Visible = false;
            pbCabeça.Visible = false;
            pbCabeça.Visible = false;
            pbCabeçaMorto.Visible = false;
            pbEnforcado.Visible = false;
            pbBracoEsqVitoria.Visible = false;
            pbCaboBand.Visible = false;
            pbBandeira.Visible = false;
            pbCabecaVitoria.Visible = false;
            led1aceso.Visible = false;
            led2aceso.Visible = false;
            led3aceso.Visible = false;
            led4aceso.Visible = false;
            led5aceso.Visible = false;
            led6aceso.Visible = false;
            led7aceso.Visible = false;
            led8aceso.Visible = false;

            txbPortaSerial.ReadOnly = true;
            // a imagem do enforcado volta ao lugar de origem
            pbEnforcado.Location = new Point(pbCabeça.Location.X, pbCabeça.Location.Y);
            // o timer da animação do enforcado subindo é parada (caso tenha perdido)
            trmMorte.Stop();
            pbEnforcado.Visible = false;
            // o txt do arduino é limpado
            txtSerial.Clear();
            // os led's do arduino são desligados
            EnviarParaPortaSerial('I');
        }
        private void Ganhou()
        {
            // o player da música de fundo é parada
            playerBack.Stop();
            // toca a música de vitória
            player = new SoundPlayer("victory.wav");
            player.Play();

            lbExibe.ForeColor = Color.Green;
            lbExibe.Text = $"Você ganhou!";

            // desativa o teclado de botões
            pTeclado.Enabled = false;

            // ativa as imagens de vitória e deixa invisível as demais 
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pbQueixo.Visible = true;
            pbPeitoral.Visible = true;
            pbBraçoEsq.Visible = false;
            pbBraçoDir.Visible = true;
            pbPerna.Visible = true;
            pbPeEsq.Visible = true;
            pbPeDir.Visible = true;
            pbCabeça.Visible = false;
            pbBandeira.Visible = true;
            pbBracoEsqVitoria.Visible = true;
            pbCaboBand.Visible = true;
            pbCabecaVitoria.Visible = true;
            // o timer de tempo (caso tenha clicado na dica) é parado
            tmrTempo.Stop();

            txbPortaSerial.ReadOnly = false;
            // configuramos de volta o cbxDica
            cbxDica.Checked = false;
            cbxDica.Enabled = true;
            // o botão iniciar fica ativado p/ jogar novamente
            btnIniciar.Enabled = true;
            // o cadastro é habilitado
            HabilitarCadastro();

            // caso a pessoa tenha escrito seu nome, aparece um messageBox com o nome dela, seus pontos e erros;
            // caso contrário, é escrito "Anônimo" invés do nome da pessoa
            if (txbNome.Text == "")
            {
                MessageBox.Show($"PARABÉNS VOCÊ GANHOU!\r\n" +
                                $"Nome: Anônimo\r\n" +
                                $"Pontos: {pontos}\r\n" +
                                $"Erros: {erros}");
            }
            else
            {
                MessageBox.Show($"PARABÉNS VOCÊ GANHOU!\r\n" +
                                $"Nome: {txbNome.Text}\r\n" +
                                $"Pontos: {pontos}\r\n" +
                                $"Erros: {erros}");
            }
            // o player de fundo é ativado novamente
            playerBack.Play();
        }
        private void Perdeu()
        {
            // o player da música de fundo é parada
            playerBack.Stop();
            // caso a pessoa perdeu, é tocada a música adequada
            player = new SoundPlayer("fail.wav");
            player.Play();

            lbExibe.ForeColor = Color.Red;
            lbExibe.Text = $"Você perdeu!";

            // a animação do "anjo" é feita
            trmMorte.Start();
            pbEnforcado.BringToFront();
            // o teclado de botões é desativado
            pTeclado.Enabled = false;

            for (int i = 0; i < oDicio[posicaoSorteada].Palavra.Length; i++) //Escreve a palavra no dgvLetras quando o usuário perder
                dgvLetras[i, 0].Value = oDicio[posicaoSorteada].Palavra[i];

            // ativa as imagens do enforcado morto
            pbQueixo.Visible = true;
            pbPeitoral.Visible = true;
            pbBraçoEsq.Visible = true;
            pbBraçoDir.Visible = true;
            pbPerna.Visible = true;
            pbPeEsq.Visible = true;
            pbPeDir.Visible = true;
            pbCabeçaMorto.Visible = true;
            pbCabeça.Visible = false;
            pbBandeira.Visible = false;
            pbBracoEsqVitoria.Visible = false;
            pbCaboBand.Visible = false;
            pbEnforcado.Visible = true;
            // o timer de tempo (caso tenha clicado na dica) é parado
            tmrTempo.Stop();

            txbPortaSerial.ReadOnly = false;
            // configuramos de volta o cbxDica
            cbxDica.Checked = false;
            cbxDica.Enabled = true;
            // o botão iniciar fica ativado p/ jogar novamente
            btnIniciar.Enabled = true;
            // o cadastro é habilitado
            HabilitarCadastro();

            // caso a pessoa tenha escrito seu nome, aparece um messageBox com o nome dela, seus pontos e erros;
            // caso contrário, é escrito "Anônimo" invés do nome da pessoa
            if (txbNome.Text == "")
            {
                MessageBox.Show($"VOCÊ PERDEU!\r\n" +
                                $"Nome: Anônimo\r\n" +
                                $"Pontos: {pontos}\r\n" +
                                $"Erros: {erros}");
            }
            else
            {
                MessageBox.Show($"VOCÊ PERDEU!\r\n" +
                                $"Nome: {txbNome.Text}\r\n" +
                                $"Pontos: {pontos}\r\n" +
                                $"Erros: {erros}");
            }
            playerBack.Play();
        }
        private void DesabilitarCadastro()
        {
            // remove a aba de cadastro
            tcJogodaForca.TabPages.Remove(tpCadastro);
        }
        private void HabilitarCadastro()
        {
            // habilita a aba de cadastro novamente
            tcJogodaForca.TabPages.Add(tpCadastro);
        }
        private void tmrHorarioPc_Tick(object sender, EventArgs e)
        {
            // a data do computador é passada para os label's adequados
            sslbHora.Text = "Hora: "+DateTime.Now.ToString(" HH:mm:ss");
            sslbData.Text = "Data: "+DateTime.Now.ToString(" dd/MM/yyyy");
        }
        private void trmMorte_Tick(object sender, EventArgs e)
        {
            // a localização do enforcado vai subindo para poder fazer uma animação
            this.pbEnforcado.Location = new Point(pbEnforcado.Location.X + 2, pbEnforcado.Location.Y - 2);
        }
        private void tpForca_Enter(object sender, EventArgs e)
        {
            // caso a pessoa clique no cbxArduino, o tamanho do formulário aumenta
            if (cbxArduino.Checked)
                frmJogoForca.ActiveForm.Width = 1349;
        }

        private void frmJogoForca_Resize(object sender, EventArgs e)
        {
            pbEnforcado.Location = new Point(pbCabeça.Location.X, pbCabeça.Location.Y);
        }

        private void cbxDica_CheckedChanged(object sender, EventArgs e)
        {
            // caso a pessoa clicar no cbxDica e estiver jogando é mostrada a dica e contado o timer
            if (cbxDica.Checked && jogando == true)
            {
                lbDica.Text = oDicio[posicaoSorteada].Dica;
                cbxDica.Enabled = false;
                tempo = 60;
                tmrTempo.Enabled = true;
            }
        }

        // Arduino
        private void cbxArduino_CheckedChanged(object sender, EventArgs e)
        {
            painelSerial.Visible = true;
            if (cbxArduino.Checked)
            {
                // caso o cbxArduino seja clicado, irá aparecer o painel do arduino:
                painelSerial.Visible = true;
                frmJogoForca.ActiveForm.Width = 1349;
                txbPortaSerial.Focus();
            }
            else
            {
                // se não, ele é redimensionado
                painelSerial.Visible = false;
                frmJogoForca.ActiveForm.Width = 849;
            }
        }
        private void txbPortaSerial_Leave(object sender, EventArgs e)
        {
            //a porta serial é fechada
            PortaSerial.Close();
            if (txbPortaSerial.Text == "" || txbPortaSerial.Text == "COM")
            {
                // caso a pessoa não digite uma COM irá aparecer a mensagem:
                MessageBox.Show("Insira uma porta serial!");
            }
            else 
            {
                // se não, a portaSerial pega a COM do TextBox
                PortaSerial.PortName = txbPortaSerial.Text;
                try
                {
                    // tenta abrir a porta serial
                    PortaSerial.Open();
                    PortaSerial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(PortaSerial_DataReceived);
                }
                catch (Exception)
                {
                    // se não, aparecerá a mensagem:
                    MessageBox.Show("Erro ao abrir porta serial!");
                    cbxArduino.Checked = false;
                }
            }
        }
        private void PortaSerial_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // o dataReceived codifica para byte e tenta fazer a leitura de dados
            byte[] buffer = new byte[PortaSerial.ReadBufferSize];
            // o bytesRead pega os dados lidos pela Porta Serial
            int bytesRead = PortaSerial.Read(buffer, 0, buffer.Length);
            // o str pega a codificação p/ ASCII dos dados lidos
            str = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // invoca o método que irá mandar os dados para o textBox Serial
            this.Invoke(new EventHandler(DisplayText));
        }
        private void DisplayText(object sender, EventArgs e)
        {
            // o txtSerial pega os dados invocados e os apresenta
            txtSerial.Text += str;
        }
        private void EnviarParaPortaSerial(char comando)
        {
            // se a porta estiver aberta, a portaSerial as escreve
            if (PortaSerial.IsOpen)
                PortaSerial.Write(comando.ToString());
        }
    }
}
