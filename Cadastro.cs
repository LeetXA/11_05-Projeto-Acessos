using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _11_05___Projeto_Acessos
{
    internal class Cadastro
    {
        private List<Usuario> usuarios;
        private List<Ambiente> ambientes;

        private string caminhoArquivo =
            Path.Combine(AppContext.BaseDirectory, "dados.json");

        public List<Usuario> Usuarios { get { return usuarios; } }
        public List<Ambiente> Ambientes { get { return ambientes; } }

        public Cadastro()
        {
            usuarios = new List<Usuario>();
            ambientes = new List<Ambiente>();
        }

        // CADASTRO DE USUÁRIOS

        public void adicionarUsuario(Usuario usuario)
        {
            if (usuario != null &&
                pesquisarUsuario(usuario) == null)
            {
                usuarios.Add(usuario);
            }
        }

        public bool removerUsuario(Usuario usuario)
        {
            Usuario encontrado = pesquisarUsuario(usuario);

            if (encontrado == null)
                return false;

            // Só permite remover usuários sem permissões
            if (encontrado.Ambientes.Count > 0)
                return false;

            return usuarios.Remove(encontrado);
        }

        public Usuario pesquisarUsuario(Usuario usuario)
        {
            if (usuario == null)
                return null;

            return usuarios.FirstOrDefault(
                u => u.Id == usuario.Id
            );
        }

        // CADASTRO DE AMBIENTES

        public void adicionarAmbiente(Ambiente ambiente)
        {
            if (ambiente != null &&
                pesquisarAmbiente(ambiente) == null)
            {
                ambientes.Add(ambiente);
            }
        }

        public bool removerAmbiente(Ambiente ambiente)
        {
            Ambiente encontrado = pesquisarAmbiente(ambiente);

            if (encontrado == null)
                return false;

            // Retira as permissões desse ambiente dos usuários
            foreach (Usuario usuario in usuarios)
            {
                usuario.revogarPermissao(encontrado);
            }

            return ambientes.Remove(encontrado);
        }

        public Ambiente pesquisarAmbiente(Ambiente ambiente)
        {
            if (ambiente == null)
                return null;

            return ambientes.FirstOrDefault(
                a => a.Id == ambiente.Id
            );
        }

        // UPLOAD: SALVAR OS DADOS

        public void upload()
        {
            try
            {
                Dados dados = new Dados();

                // Salva usuários
                foreach (Usuario usuario in usuarios)
                {
                    dados.Usuarios.Add(new UsuarioDados
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Ambientes = usuario.Ambientes
                            .Select(a => a.Id).ToList()
                    });
                }

                // Salva ambientes e seus logs
                foreach (Ambiente ambiente in ambientes)
                {
                    AmbienteDados ambienteDados = new AmbienteDados
                    {
                        Id = ambiente.Id,
                        Nome = ambiente.Nome
                    };

                    foreach (Log log in ambiente.Logs)
                    {
                        ambienteDados.Logs.Add(new LogDados
                        {
                            DtAcesso = log.DtAcesso,
                            UsuarioId = log.Usuario.Id,
                            TipoAcesso = log.TipoAcesso
                        });
                    }

                    dados.Ambientes.Add(ambienteDados);
                }

                JsonSerializerOptions opcoes = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(dados, opcoes);

                File.WriteAllText(caminhoArquivo, json);

                Console.WriteLine("\nDados salvos com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar dados: " + ex.Message);
            }
        }

        // DOWNLOAD: CARREGAR OS DADOS

        public void download()
        {
            try
            {
                if (!File.Exists(caminhoArquivo))
                    return;

                string json = File.ReadAllText(caminhoArquivo);

                Dados dados = JsonSerializer.Deserialize<Dados>(json);

                if (dados == null)
                    return;

                usuarios.Clear();
                ambientes.Clear();

                // Primeiro carrega todos os ambientes
                foreach (AmbienteDados a in dados.Ambientes)
                {
                    ambientes.Add(new Ambiente(a.Id, a.Nome));
                }

                // Depois carrega todos os usuários
                foreach (UsuarioDados u in dados.Usuarios)
                {
                    usuarios.Add(new Usuario(u.Id, u.Nome));
                }

                // Reconstrói as permissões dos usuários
                foreach (UsuarioDados u in dados.Usuarios)
                {
                    Usuario usuario = pesquisarUsuario(
                        new Usuario(u.Id, u.Nome)
                    );

                    foreach (int ambienteId in u.Ambientes)
                    {
                        Ambiente ambiente = pesquisarAmbiente(
                            new Ambiente(ambienteId, "")
                        );

                        if (ambiente != null)
                            usuario.concederPermissao(ambiente);
                    }
                }

                // Reconstrói os logs
                foreach (AmbienteDados a in dados.Ambientes)
                {
                    Ambiente ambiente = pesquisarAmbiente(
                        new Ambiente(a.Id, "")
                    );

                    foreach (LogDados l in a.Logs)
                    {
                        Usuario usuario = pesquisarUsuario(
                            new Usuario(l.UsuarioId, "")
                        );

                        if (usuario != null)
                        {
                            Log log = new Log(
                                l.DtAcesso,
                                usuario,
                                l.TipoAcesso
                            );

                            ambiente.registrarLog(log);
                        }
                    }
                }

                Console.WriteLine("Dados carregados com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar dados: " + ex.Message);
            }
        }

        // CLASSES AUXILIARES PARA O JSON

        private class Dados
        {
            public List<UsuarioDados> Usuarios { get; set; } = new();
            public List<AmbienteDados> Ambientes { get; set; } = new();
        }

        private class UsuarioDados
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public List<int> Ambientes { get; set; } = new();
        }

        private class AmbienteDados
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public List<LogDados> Logs { get; set; } = new();
        }

        private class LogDados
        {
            public DateTime DtAcesso { get; set; }
            public int UsuarioId { get; set; }
            public bool TipoAcesso { get; set; }
        }
    }
}
