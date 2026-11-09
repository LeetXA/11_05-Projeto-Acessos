
using _11_05___Projeto_Acessos;
using System;
using System.Linq;

class Program
{
    static Cadastro cadastro = new Cadastro();

    static void Main(string[] args)
    {
        // Carrega os dados ao iniciar
        cadastro.download();

        int opcao;

        do
        {
            Console.Clear();

            Console.WriteLine("==================================");
            Console.WriteLine("       CONTROLE DE ACESSOS");
            Console.WriteLine("==================================");
            Console.WriteLine("0 - Sair");
            Console.WriteLine("1 - Cadastrar ambiente");
            Console.WriteLine("2 - Consultar ambiente");
            Console.WriteLine("3 - Excluir ambiente");
            Console.WriteLine("4 - Cadastrar usuário");
            Console.WriteLine("5 - Consultar usuário");
            Console.WriteLine("6 - Excluir usuário");
            Console.WriteLine("7 - Conceder permissão de acesso");
            Console.WriteLine("8 - Revogar permissão de acesso");
            Console.WriteLine("9 - Registrar acesso");
            Console.WriteLine("10 - Consultar logs de acesso");
            Console.WriteLine("==================================");

            opcao = LerInteiro("Escolha uma opção: ");

            Console.Clear();

            switch (opcao)
            {
                case 0:
                    cadastro.upload();
                    Console.WriteLine("\nEncerrando o sistema...");
                    break;

                case 1:
                    CadastrarAmbiente();
                    break;

                case 2:
                    ConsultarAmbiente();
                    break;

                case 3:
                    ExcluirAmbiente();
                    break;

                case 4:
                    CadastrarUsuario();
                    break;

                case 5:
                    ConsultarUsuario();
                    break;

                case 6:
                    ExcluirUsuario();
                    break;

                case 7:
                    ConcederPermissao();
                    break;

                case 8:
                    RevogarPermissao();
                    break;

                case 9:
                    RegistrarAcesso();
                    break;

                case 10:
                    ConsultarLogs();
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

            if (opcao != 0)
            {
                Console.WriteLine("\nPressione ENTER para continuar...");
                Console.ReadLine();
            }

        } while (opcao != 0);
    }

    // MÉTODO PARA LER NÚMEROS INTEIROS

    static int LerInteiro(string mensagem)
    {
        int valor;

        while (true)
        {
            Console.Write(mensagem);

            if (int.TryParse(Console.ReadLine(), out valor))
                return valor;

            Console.WriteLine("Digite um número válido!");
        }
    }

    // BUSCAR AMBIENTE

    static Ambiente BuscarAmbiente()
    {
        int id = LerInteiro("Informe o ID do ambiente: ");

        return cadastro.pesquisarAmbiente(
            new Ambiente(id, "")
        );
    }

    // BUSCAR USUÁRIO

    static Usuario BuscarUsuario()
    {
        int id = LerInteiro("Informe o ID do usuário: ");

        return cadastro.pesquisarUsuario(
            new Usuario(id, "")
        );
    }

    // 1 - CADASTRAR AMBIENTE

    static void CadastrarAmbiente()
    {
        Console.WriteLine("=== CADASTRAR AMBIENTE ===");

        int id = LerInteiro("ID: ");

        if (cadastro.pesquisarAmbiente(
            new Ambiente(id, "")) != null)
        {
            Console.WriteLine("Já existe um ambiente com esse ID!");
            return;
        }

        Console.Write("Nome do ambiente: ");
        string nome = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        Ambiente ambiente = new Ambiente(id, nome);

        cadastro.adicionarAmbiente(ambiente);

        Console.WriteLine("Ambiente cadastrado com sucesso!");
    }

    // 2 - CONSULTAR AMBIENTE

    static void ConsultarAmbiente()
    {
        Console.WriteLine("=== CONSULTAR AMBIENTE ===");

        Ambiente ambiente = BuscarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Console.WriteLine("\nID: " + ambiente.Id);
        Console.WriteLine("Nome: " + ambiente.Nome);
        Console.WriteLine("Quantidade de logs: " + ambiente.Logs.Count);

        Console.WriteLine("\nUsuários com permissão:");

        var usuarios = cadastro.Usuarios
            .Where(u => u.possuiPermissao(ambiente))
            .ToList();

        if (usuarios.Count == 0)
        {
            Console.WriteLine("Nenhum usuário possui permissão.");
        }
        else
        {
            foreach (Usuario usuario in usuarios)
            {
                Console.WriteLine(
                    usuario.Id + " - " + usuario.Nome
                );
            }
        }
    }

    // 3 - EXCLUIR AMBIENTE

    static void ExcluirAmbiente()
    {
        Console.WriteLine("=== EXCLUIR AMBIENTE ===");

        Ambiente ambiente = BuscarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Console.Write("Deseja realmente excluir? (S/N): ");
        string resposta = Console.ReadLine();

        if (resposta?.ToUpper() == "S")
        {
            bool sucesso = cadastro.removerAmbiente(ambiente);

            Console.WriteLine(
                sucesso
                ? "Ambiente excluído com sucesso!"
                : "Não foi possível excluir o ambiente."
            );
        }
        else
        {
            Console.WriteLine("Operação cancelada.");
        }
    }

    // 4 - CADASTRAR USUÁRIO

    static void CadastrarUsuario()
    {
        Console.WriteLine("=== CADASTRAR USUÁRIO ===");

        int id = LerInteiro("ID: ");

        if (cadastro.pesquisarUsuario(
            new Usuario(id, "")) != null)
        {
            Console.WriteLine("Já existe um usuário com esse ID!");
            return;
        }

        Console.Write("Nome do usuário: ");
        string nome = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        Usuario usuario = new Usuario(id, nome);

        cadastro.adicionarUsuario(usuario);

        Console.WriteLine("Usuário cadastrado com sucesso!");
    }

    // 5 - CONSULTAR USUÁRIO

    static void ConsultarUsuario()
    {
        Console.WriteLine("=== CONSULTAR USUÁRIO ===");

        Usuario usuario = BuscarUsuario();

        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }

        Console.WriteLine("\nID: " + usuario.Id);
        Console.WriteLine("Nome: " + usuario.Nome);

        Console.WriteLine("\nAmbientes com permissão:");

        if (usuario.Ambientes.Count == 0)
        {
            Console.WriteLine("Nenhuma permissão cadastrada.");
        }
        else
        {
            foreach (Ambiente ambiente in usuario.Ambientes)
            {
                Console.WriteLine(
                    ambiente.Id + " - " + ambiente.Nome
                );
            }
        }
    }

    // 6 - EXCLUIR USUÁRIO

    static void ExcluirUsuario()
    {
        Console.WriteLine("=== EXCLUIR USUÁRIO ===");

        Usuario usuario = BuscarUsuario();

        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }

        bool sucesso = cadastro.removerUsuario(usuario);

        if (sucesso)
        {
            Console.WriteLine("Usuário excluído com sucesso!");
        }
        else
        {
            Console.WriteLine(
                "Não foi possível excluir o usuário!"
            );

            Console.WriteLine(
                "Verifique se ele possui permissões de acesso."
            );
        }
    }

    // 7 - CONCEDER PERMISSÃO

    static void ConcederPermissao()
    {
        Console.WriteLine("=== CONCEDER PERMISSÃO ===");

        Ambiente ambiente = BuscarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Usuario usuario = BuscarUsuario();

        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }

        bool sucesso = usuario.concederPermissao(ambiente);

        Console.WriteLine(
            sucesso
            ? "Permissão concedida com sucesso!"
            : "Usuário já possui permissão nesse ambiente."
        );
    }

    // 8 - REVOGAR PERMISSÃO

    static void RevogarPermissao()
    {
        Console.WriteLine("=== REVOGAR PERMISSÃO ===");

        Ambiente ambiente = BuscarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Usuario usuario = BuscarUsuario();

        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }

        bool sucesso = usuario.revogarPermissao(ambiente);

        Console.WriteLine(
            sucesso
            ? "Permissão revogada com sucesso!"
            : "O usuário não possui permissão nesse ambiente."
        );
    }

    // 9 - REGISTRAR ACESSO

    static void RegistrarAcesso()
    {
        Console.WriteLine("=== REGISTRAR ACESSO ===");

        Ambiente ambiente = BuscarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Usuario usuario = BuscarUsuario();

        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado!");
            return;
        }

        // Verifica se o usuário possui permissão
        bool autorizado = usuario.possuiPermissao(ambiente);

        // Cria o log da tentativa
        Log log = new Log(
            DateTime.Now,
            usuario,
            autorizado
        );

        // Registra o log no ambiente
        ambiente.registrarLog(log);

        if (autorizado)
        {
            Console.WriteLine("\nACESSO AUTORIZADO!");
        }
        else
        {
            Console.WriteLine("\nACESSO NEGADO!");
        }

        Console.WriteLine("Usuário: " + usuario.Nome);
        Console.WriteLine("Ambiente: " + ambiente.Nome);
        Console.WriteLine("Data: " + log.DtAcesso);
    }

    // 10 - CONSULTAR LOGS

    static void ConsultarLogs()
    {
        Console.WriteLine("=== CONSULTAR LOGS DE ACESSO ===");

        Ambiente ambiente = BuscarAmbiente();

        if (ambiente == null)
        {
            Console.WriteLine("Ambiente não encontrado!");
            return;
        }

        Console.WriteLine("\nAmbiente: " + ambiente.Nome);
        Console.WriteLine("\n1 - Todos os logs");
        Console.WriteLine("2 - Somente autorizados");
        Console.WriteLine("3 - Somente negados");

        int filtro = LerInteiro("\nEscolha o filtro: ");

        var logs = ambiente.Logs.AsEnumerable();

        switch (filtro)
        {
            case 1:
                break;

            case 2:
                logs = logs.Where(l => l.TipoAcesso == true);
                break;

            case 3:
                logs = logs.Where(l => l.TipoAcesso == false);
                break;

            default:
                Console.WriteLine("Filtro inválido!");
                return;
        }

        var listaLogs = logs.ToList();

        Console.WriteLine("\n=== RESULTADO DOS LOGS ===");

        if (listaLogs.Count == 0)
        {
            Console.WriteLine("Nenhum log encontrado.");
            return;
        }

        foreach (Log log in listaLogs)
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Usuário: " + log.Usuario.Nome);
            Console.WriteLine("ID: " + log.Usuario.Id);
            Console.WriteLine("Data: " + log.DtAcesso);

            Console.WriteLine(
                "Resultado: " +
                (log.TipoAcesso ? "AUTORIZADO" : "NEGADO")
            );
        }

        Console.WriteLine("----------------------------------");
        Console.WriteLine("Total de registros: " + listaLogs.Count);
    }
}
