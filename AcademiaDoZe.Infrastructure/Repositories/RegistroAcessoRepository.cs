
using System.Data;
using System.Data.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class RegistroAcessoRepository
    : BaseRepository, IRegistroAcessoRepository
{
    public RegistroAcessoRepository(
        string connectionString,
        DatabaseType databaseType)
        : base(connectionString, databaseType)
    {
    }

    public async Task<RegistroAcesso?> ObterEntradaAbertaAsync(
        int pessoaId,
        int tipoPessoa)
    {
        try
        {
            const string sql = """
                SELECT TOP (1)
                    id_registro_acesso,
                    pessoa_id,
                    tipo_pessoa,
                    entrada,
                    saida
                FROM dbo.tb_registro_acesso
                WHERE pessoa_id = @PessoaId
                  AND tipo_pessoa = @TipoPessoa
                  AND saida IS NULL
                ORDER BY entrada DESC
                """;

            await using var command = await CreateCommandAsync(sql);

            command.AddParameter("@PessoaId", pessoaId, DbType.Int32);
            command.AddParameter("@TipoPessoa", tipoPessoa, DbType.Int32);

            await using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync()
                ? Map(reader)
                : null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_OBTER_ACESSO_ABERTO",
                $"Erro ao consultar acesso aberto: {ex.Message}",
                ex);
        }
    }

    public async Task<IReadOnlyList<RegistroAcesso>> ObterPorDataAsync(
        DateTime data)
    {
        try
        {
            const string sql = """
                SELECT
                    id_registro_acesso,
                    pessoa_id,
                    tipo_pessoa,
                    entrada,
                    saida
                FROM dbo.tb_registro_acesso
                WHERE
                    (entrada >= @Inicio AND entrada < @Fim)
                    OR
                    (saida >= @Inicio AND saida < @Fim)
                ORDER BY entrada DESC
                """;

            await using var command = await CreateCommandAsync(sql);

            command.AddParameter(
                "@Inicio", data.Date, DbType.DateTime2);

            command.AddParameter(
                "@Fim", data.Date.AddDays(1), DbType.DateTime2);

            await using var reader = await command.ExecuteReaderAsync();

            var registros = new List<RegistroAcesso>();

            while (await reader.ReadAsync())
                registros.Add(Map(reader));

            return registros;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_LISTAR_ACESSOS",
                $"Erro ao consultar histórico de acessos: {ex.Message}",
                ex);
        }
    }

    public async Task RegistrarEntradaAsync(RegistroAcesso registro)
    {
        try
        {
            const string sql = """
                INSERT INTO dbo.tb_registro_acesso
                    (pessoa_id, tipo_pessoa, entrada)
                VALUES
                    (@PessoaId, @TipoPessoa, @Entrada)
                """;

            await using var command = await CreateCommandAsync(sql);

            command.AddParameter(
                "@PessoaId", registro.PessoaId, DbType.Int32);

            command.AddParameter(
                "@TipoPessoa", registro.TipoPessoa, DbType.Int32);

            command.AddParameter(
                "@Entrada", registro.Entrada, DbType.DateTime2);

            await command.ExecuteNonQueryAsync();
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_REGISTRAR_ENTRADA",
                $"Erro ao registrar entrada: {ex.Message}",
                ex);
        }
    }

    public async Task RegistrarSaidaAsync(RegistroAcesso registro)
    {
        try
        {
            const string sql = """
                UPDATE dbo.tb_registro_acesso
                SET saida = @Saida
                WHERE id_registro_acesso = @Id
                  AND saida IS NULL
                """;

            await using var command = await CreateCommandAsync(sql);

            command.AddParameter(
                "@Id", registro.Id, DbType.Int32);

            command.AddParameter(
                "@Saida",
                registro.Saida ?? DateTime.Now,
                DbType.DateTime2);

            var afetados = await command.ExecuteNonQueryAsync();

            if (afetados == 0)
                throw new InvalidOperationException(
                    "O acesso já foi encerrado ou não existe.");
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_REGISTRAR_SAIDA",
                $"Erro ao registrar saída: {ex.Message}",
                ex);
        }
    }

    private static RegistroAcesso Map(DbDataReader reader)
    {
        return new RegistroAcesso
        {
            Id = reader.GetInt32(
                reader.GetOrdinal("id_registro_acesso")),

            PessoaId = reader.GetInt32(
                reader.GetOrdinal("pessoa_id")),

            TipoPessoa = reader.GetInt32(
                reader.GetOrdinal("tipo_pessoa")),

            Entrada = reader.GetDateTime(
                reader.GetOrdinal("entrada")),

            Saida = reader.IsDBNull(reader.GetOrdinal("saida"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("saida"))
        };
    }
}
