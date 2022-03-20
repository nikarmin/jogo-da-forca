using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*
    Técnicas de Programação I
    Projeto III
    Alunos: Nicoli Ferreira - 21689
            Samuel de Campos Ferraz - 21260

    Professor: Francisco da Fonseca Rodrigues
 */

class Dicionario
    {
        bool[] acertou = new bool[15];
        string palavra,
               dica;
        const int tamanhoPalavra = 15;
        const int inicioPalavra = 0,
                  inicioDica = inicioPalavra + tamanhoPalavra;
        public Dicionario(string pal, string dic)
        {
            Palavra = pal;
            Dica = dic;
        }

        public Dicionario()
        {
            Palavra = " ";
            Dica = " ";
        }
        public string Palavra { get => palavra; set => palavra = value.TrimEnd(); } // TrimEnd() Ignora as casas a direita da palavra
        public string Dica { get => dica; set => dica = value; }

        public void LerDados(StreamReader arq)
        {
            // faz a leitura dos dados
            if (!arq.EndOfStream)
            {
                String linha = arq.ReadLine();
                Palavra = linha.Substring(inicioPalavra, tamanhoPalavra);
                Dica = linha.Substring(inicioDica);
            }
        }
        public String FormatoDeArquivo()
        {
            // retorna um formato de arquivo adequado para o que estamos usando
            return Palavra.PadRight(15) + Dica;
        }
        public void LimparAcertos()
        {
            // limpa os acertos do array
            for (int i = 0; i < acertou.Length; i++)
                acertou[i] = false;
        }
    public int IndiceDaLetra(char letra, string palavraSorteada)
    {
        int indiceDaLetra = -1;
        // percorre a palavra sorteada
        for (int i = 0; i < palavraSorteada.Length; i++)
            // faz uma verificação se a letra faz parte da palavra
            if (letra == palavraSorteada[i] && acertou[i] == false)
            {
                // caso não, pega o índice da letra e a retorna
                acertou[i] = true;
                indiceDaLetra = i;
                return indiceDaLetra;
            }
        // caso tenha sido encontrada, é retornada o índice da letra correta (-1 ou > -1)
        return indiceDaLetra;
    }
}
