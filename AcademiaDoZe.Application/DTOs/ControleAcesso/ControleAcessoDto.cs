using System;
using System.Collections.Generic;
using System.Text;

namespace AcademiaDoZe.Application.DTOs.ControleAcesso
{
    public class PessoaAcessoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string Situacao { get; set; } = string.Empty;
        public bool PossuiEntradaAberta { get; set; }
        public TipoPessoaAcesso Tipo { get; set; }
    }

    public enum TipoPessoaAcesso
    {
        Aluno,
        Colaborador
    }
}
