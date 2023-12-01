// See https://aka.ms/new-console-template for more information

using SimArquivosUnix;

using System;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using static SimArquivosUnix.Enum;
using static System.Reflection.Metadata.BlobBuilder;

class Program
{
    private static int blocos = 1;
    private static List<User> users = new List<User>();
    private static List<Inode> listaInodes = new List<Inode>();
    private static Inode inodeRaiz = new Inode(0,"/" , 0, 1000, 1000, 0, SimArquivosUnix.Enum.TipoInode.Diretorio,0);
    private static Inode inodeAtual = inodeRaiz;
    private static string diretorioAtual = inodeAtual.Nome;
    private static string NomeUsuarioAtual;
    private static User usuarioAtual;
    private static int idsInodes = 1;
    private static int idsUsuarios = 2;
    static void Main()
    {
        listaInodes.Add(inodeRaiz);
        users.Add(new User("ADM", "123" , 1, 1, "/",  "/"));
        users.Add(new User("Christian", "123", idsUsuarios, 2, "/", "/"));
        idsUsuarios++;
        users.Add(new User("Outro", "123", idsUsuarios, 3, "/", "/"));
        idsUsuarios++;
        bool sair = false;

        User retUsuario = new User();
        while (retUsuario.NomeUsuario == null) {
            retUsuario = SelecionarUsuario();
        }
        
        User nomeUsuario = retUsuario;
        NomeUsuarioAtual = nomeUsuario.NomeUsuario;
        usuarioAtual = nomeUsuario;
        //Inode InodeRaiz = new Inode(0, 1000, 1000, 0, SimArquivosUnix.Enum.TipoInode.Diretorio);

        do
        {
            Console.Clear(); // Limpa a tela a cada iteração
            
            ExibirMenu();

            // Ler a escolha do usuário
            Console.Write("");
            string escolha = Console.ReadLine();

            string[] entrada = escolha.Trim().Split(' ');

            switch (entrada[0])
            {

                case "touch":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        CriarArquivo(entrada[1]);
                    }
                    
                    break;

                case "gravar_conteudo":
                    if (entrada.Length < 3)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        GravarConteudo(entrada[1] , entrada);
                    }
                    break;

                case "cat":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Cat(entrada[1]);
                    }
                    break;
                case "rm":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        RemoveDiretorioOuArquivo(entrada[1]);
                    }
                    break;
                case "chown":
                    if (entrada.Length != 4)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Chown(entrada[1], entrada[2], entrada[3]);
                    }
                    break;
                case "chmod":
                    if (entrada.Length != 4)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Chmod(entrada[1], entrada[2], entrada[3]);
                    }
                    break;
                case "mkdir":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        CriarDiretorio(entrada[1]);
                    }
                    break;
                case "rmdir":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        RemoveDiretorioOuArquivo(entrada[1]);
                    }
                    break;
                case "cd":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else if (entrada[1] == "..")
                    {
                        CdPai();
                    }
                    else
                    {
                        Cd(entrada[1]);
                    }
                    break;
                case "ls":
                    if (entrada.Length != 1)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Ls();
                    }
                    break;
                case "adduser":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        AddUser(entrada[1]);
                    }
                    break;
                case "rmuser":
                    if (entrada.Length != 2)
                    {
                        Console.WriteLine("numero de parâmetros inválido!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        RemoveUser(entrada[1]);
                    }
                    break;
                case "lsuser":
                    ListarUsers();
                    break;
                case "trocar_user":
                    User ret = SelecionarUsuario();
                    NomeUsuarioAtual = ret.NomeUsuario;
                    usuarioAtual = ret;
                    break;
                case "blocos":
                    Console.WriteLine(blocos);
                    break;
                case "q":
                    sair = true;
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\n.");
            Console.ReadKey();
        } while (!sair);
    }

    static void RemoveUser(string nomeUsuario)
    {
        if (usuarioAtual.Uid != 1)
        {
            Console.WriteLine("Usuário não possui permissão!");
            return;
        }
        foreach (var user in users) {
            if (user.NomeUsuario == nomeUsuario)
            {
                users.Remove(user);
                return;
            }
        }
    }

    static void AddUser(string nomeUsuario) {
        if (usuarioAtual.Uid != 1) {
            Console.WriteLine("Usuário não possui permissão!");
            return;
        }
        users.Add(new User(nomeUsuario, "123", idsUsuarios, 2, "/", "/"));
        idsUsuarios++;
    }

    static void ListarUsers() {
        Console.WriteLine("Usuários: ");
        for (int i = 0; i < users.Count; i++)
        {
            Console.WriteLine(string.Concat(i, " - ", users[i].NomeUsuario));
        }
    }

    static void Chmod(string arquivo, string tipoPermissao, string permissoes)
    {
        int idInodeDestino = -1;
        //listar pastas e verificar se o nome de parametro existe e pegar o id

        foreach (var inode in inodeAtual.BlockPointers)
        {
            if (inode != null)
            {
                idInodeDestino = inode.Nome == arquivo ? inode.Id : idInodeDestino;
            }
        }
        if (idInodeDestino < 0)
        {
            Console.WriteLine("Arquivo não encontrado!");
        }
        else
        {
            Inode inodeDestino = new Inode();
            foreach (var inode in listaInodes)
            {
                inodeDestino = inode.Id == idInodeDestino && inode.TipoInode == TipoInode.Arquivo ? inode : null;
            }
            if (inodeDestino == null)
            {
                Console.WriteLine("Arquivo não encontrado!");
            }
            else
            {
                bool[] Permissoes = new bool[3];
                if (tipoPermissao == "d")
                {
                    //muda permissoes dono inode do arquivo
                    inodeDestino.PermissaoDono[0] = permissoes.Substring(0,1) == "v" ? true : false;
                    inodeDestino.PermissaoDono[1] = permissoes.Substring(1, 1) == "v" ? true : false; 
                    inodeDestino.PermissaoDono[2] = permissoes.Substring(2, 1) == "v" ? true : false; 
                }
                else if (tipoPermissao == "g")
                {
                    //muda permissoes grupo inode do arquivo
                    inodeDestino.PermissaoGrupo[0] = permissoes.Substring(0, 1) == "v" ? true : false;
                    inodeDestino.PermissaoGrupo[1] = permissoes.Substring(1, 1) == "v" ? true : false;
                    inodeDestino.PermissaoGrupo[2] = permissoes.Substring(2, 1) == "v" ? true : false;
                }
                else if (tipoPermissao == "o")
                {
                    //muda permissoes outros inode do arquivo
                    inodeDestino.PermissaoOutrosUsuario[0] = permissoes.Substring(0, 1) == "v" ? true : false;
                    inodeDestino.PermissaoOutrosUsuario[1] = permissoes.Substring(1, 1) == "v" ? true : false;
                    inodeDestino.PermissaoOutrosUsuario[2] = permissoes.Substring(2, 1) == "v" ? true : false;
                }
            }
        }
        
    }

    static void Chown(string arquivo, string user1, string user2) {
        int idInodeDestino = -1;
        //listar pastas e verificar se o nome de parametro existe e pegar o id

        foreach (var inode in inodeAtual.BlockPointers)
        {
            if (inode != null)
            {
                idInodeDestino = inode.Nome == arquivo ? inode.Id : idInodeDestino;
            }
        }
        if (idInodeDestino < 0)
        {
            Console.WriteLine("Arquivo não encontrado!");
        }
        else
        {
            Inode inodeDestino = new Inode();
            foreach (var inode in listaInodes)
            {
                inodeDestino = inode.Id == idInodeDestino && inode.TipoInode == TipoInode.Arquivo ? inode : null;
            }
            if (inodeDestino == null)
            {
                Console.WriteLine("Arquivo não encontrado!");
            }
            else
            {
                if (usuarioAtual.Uid != 1 && inodeDestino.Owner != usuarioAtual.Uid && inodeDestino.Group != usuarioAtual.Gid)
                {
                    Console.WriteLine("Usuário não possui permissão");
                    return;
                }
                int idUser = -1;
                foreach (var user in users)
                {
                    idUser = user.NomeUsuario == user2 ? user.Uid : idUser;
                }
                if (idUser != -1)
                {
                    inodeDestino.Owner = idUser;
                }
            }
        }

    }

    static void Cat(string arquivo) {
        int idInodeDestino = -1;
        //listar pastas e verificar se o nome de parametro existe e pegar o id

        foreach (var inode in inodeAtual.BlockPointers)
        {
            if (inode != null)
            {
                idInodeDestino = inode.Nome == arquivo ? inode.Id : idInodeDestino;
            }
        }
        if (idInodeDestino < 0)
        {
            Console.WriteLine("Diretório não encontrado!");
        }
        else
        {
            Inode inodeDestino = new Inode();
            foreach (var inode in listaInodes)
            {
                inodeDestino = inode.Id == idInodeDestino && inode.TipoInode == TipoInode.Arquivo ? inode : null;
            }
            if (inodeDestino == null)
            {
                Console.WriteLine("Arquivo não encontrado!");
            }
            else
            {
                if (!VerificaPermissaoLeitura(inodeDestino))
                {
                    Console.WriteLine("Usuario não possui permissão de leitura!");
                    return;
                }
                inodeDestino.PrintConteudo(); 
                inodeDestino.LastAccessTime = DateTime.Now;  
            }
        }

    }

    static void GravarConteudo(string arquivo, string[] conteudo) {
        //nome, posição, nbytes, buffer
        int tamanhoConteudo = CalcularQuantidadeTotalCaracteres(conteudo);
        int blocosNecessario = Convert.ToInt32(tamanhoConteudo) / 512;

        if (blocos - blocosNecessario < 0)
        {
            Console.WriteLine("Tamanho de disco insuficiente!");
            return;
        }

        

        int idInodeDestino = -1;
        //listar pastas e verificar se o nome de parametro existe e pegar o id

        foreach (var inode in inodeAtual.BlockPointers)
        {
            if (inode != null)
            {
                idInodeDestino = inode.Nome == arquivo ? inode.Id : idInodeDestino;
            }
        }
        if (idInodeDestino < 0)
        {
            Console.WriteLine("Arquivo não encontrado!");
        }
        else
        {
            Inode inodeDestino = new Inode();
            foreach (var inode in listaInodes)
            {
                inodeDestino = inode.Id == idInodeDestino && inode.TipoInode == TipoInode.Arquivo ? inode : null;
            }
            if (inodeDestino == null)
            {
                Console.WriteLine("Diretório não encontrado!");
            }
            else
            {
                if (!VerificaPermissaoAlteracao(inodeDestino))
                {
                    Console.WriteLine("Usuario não possui permissão!");
                    return;
                }
                int j = 0;
                inodeDestino.conteudo = new string[conteudo.Length -3];
                for (int i = 3; i < conteudo.Length; i++)
                {
                    inodeDestino.conteudo[j] = conteudo[i];
                    j++;
                }
                inodeDestino.Size = Convert.ToInt32(tamanhoConteudo);
                inodeDestino.LastModificationTime = DateTime.Now;
                blocos = blocos - blocosNecessario;
            }
        }
    }

    static int CalcularQuantidadeTotalCaracteres(string[] arrayDeStrings)
    {
        int totalCaracteres = 0;

        // Iterar sobre cada string no array
        foreach (string str in arrayDeStrings)
        {
            // Adicionar o comprimento da string ao total
            totalCaracteres += str.Length;
        }

        return totalCaracteres;
    }

    static void CriarArquivo(string nomeArquivo)
    {
        try
        {
           
            int proximaPosicaoLivre = inodeAtual.ProximaPosicaoInode();
            if (proximaPosicaoLivre == -1) {
                Console.WriteLine("sem espaço no inode");
                return;
            }
            // Cria um arquivo vazio
            int tamanho = 0;

            Inode myInode = new Inode(idsInodes, nomeArquivo, 0, usuarioAtual.Uid, usuarioAtual.Gid, tamanho, SimArquivosUnix.Enum.TipoInode.Arquivo, inodeAtual.Id); // Exemplo de modo, proprietário e grupo
            idsInodes++;
            //adiciona no inode atual
            inodeAtual.BlockPointers[proximaPosicaoLivre] = myInode;
            listaInodes.Add(myInode);
            blocos = blocos - (tamanho / 512);
            Console.WriteLine(String.Concat("Arquivo criado!!! ", blocos, " restantes!"));
            //myInode.InformacoesInode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o arquivo: {ex.Message}");
        }
    }

    static void CriarDiretorio(string nomeArquivo)
    {
        try
        {
            
            int proximaPosicaoInode = inodeAtual.ProximaPosicaoInode();
            if (proximaPosicaoInode == -1 )
            {
                Console.WriteLine("Sem espaço no inode");
                return;
            }
            else if (blocos < 1)
            {
                Console.WriteLine("Espaço em disco insuficiente!");
                return;
            }
            // Cria um arquivo vazio
            int tamanho = 512;//um bloco

            Inode myInode = new Inode(idsInodes, nomeArquivo, 0, usuarioAtual.Uid, usuarioAtual.Gid, tamanho, SimArquivosUnix.Enum.TipoInode.Diretorio, inodeAtual.Id); // Exemplo de modo, proprietário e grupo
            idsInodes++;
            //adiciona no inode atual
            inodeAtual.BlockPointers[proximaPosicaoInode] = myInode;
            listaInodes.Add(myInode);
            blocos = blocos - (tamanho / 512);
            Console.WriteLine(String.Concat("Diretório criado!!! ", blocos, " restantes!"));
            //myInode.InformacoesInode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o diretório: {ex.Message}");
        }
    }

    static void RemoveDiretorioOuArquivo(string nomeDirArquivo)
    {
        try
        {
            int idInodeRemover = -1;
            foreach (var inode in inodeAtual.BlockPointers)
            {
                if (inode != null)
                {
                    idInodeRemover = inode.Nome == nomeDirArquivo ? inode.Id : idInodeRemover;
                    //break;
                }
            }
            if (idInodeRemover < 0)
            {
                Console.WriteLine("Diretório/arquivo não encontrado!");
            }
            else
            {
                //remove do inode atual
                for (int i = 0; i < inodeAtual.BlockPointers.Length; i++)
                {
                    if (inodeAtual.BlockPointers[i].Id == idInodeRemover)
                    {
                        //verifica se tem permissao
                        if (!VerificaPermissaoAlteracao(inodeAtual.BlockPointers[i]))
                        {
                            Console.WriteLine("Usuario não possui permissão!");
                            return;
                        }
                        inodeAtual.BlockPointers[i] = null;
                        break;
                    }
                }
                //remove da lista de inodes 
                foreach (var inode in listaInodes)
                {
                    if (inode.Id == idInodeRemover)
                    {
                        blocos = blocos + (inode.Size / 512);
                        listaInodes.Remove(inode);
                        break;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao remover diretorio/arquivo: {ex.Message}");
        }
    }

    static void Cd(string nomeInodeDestino)
    {
        try
        {
            int idInodeDestino = -1;
            //listar pastas e verificar se o nome de parametro existe e pegar o id
            
                foreach (var inode in inodeAtual.BlockPointers)
                {
                    if (inode != null)
                    {
                        idInodeDestino = inode.Nome == nomeInodeDestino.Trim() ? inode.Id : idInodeDestino;
                    }
                }
                if (idInodeDestino < 0)
                {
                    Console.WriteLine("Diretório não encontrado!");
                }
                else
                {
                    Inode inodeDestino = new Inode();
                    foreach (var inode in listaInodes)
                    {
                        inodeDestino = inode.Id == idInodeDestino && inode.TipoInode == TipoInode.Diretorio ? inode : null;
                    }
                    if (inodeDestino == null)
                    {
                        Console.WriteLine("Diretório não encontrado!");
                    }
                    else
                    {
                        inodeAtual = inodeDestino;
                        diretorioAtual = diretorioAtual + inodeDestino.Nome + "  ";
                    }
                }
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o arquivo: {ex.Message}");
        }
    }

    static void CdPai()
    {
        try
        {
            string dirAtual = inodeAtual.Nome;
            Inode inodeDestino = new Inode();
            foreach (var inode in listaInodes)
            {
                inodeDestino = inode.Id == inodeAtual.IdPai ? inode : inodeDestino;
            }
            if (inodeDestino == null)
            {
                Console.WriteLine("Diretório não encontrado!");
            }
            else
            {
                inodeAtual = inodeDestino;
                
                diretorioAtual = diretorioAtual.Remove(diretorioAtual.Trim().Length - dirAtual.Length, dirAtual.Length);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o arquivo: {ex.Message}");
        }
    }

    static void Ls() {
        Console.WriteLine(String.Concat("Nome".PadRight(20), "Data modificação".PadRight(40), "Tipo".PadRight(20), "Tamanho".PadRight(10), "Permissões"));
        foreach (var item in inodeAtual.BlockPointers)
        {
            if (item != null)
            {
                Console.WriteLine(String.Concat(item.Nome.PadRight(20), item.LastModificationTime.ToString().PadRight(40), item.TipoInode.ToString().PadRight(20), item.Size.ToString().PadRight(10), VerificaPermissoes(item)));
            }
        }
    }

    static bool VerificaPermissaoAlteracao(Inode inode) {

        bool permitido = false;
        //remover
        if (usuarioAtual.Uid == 1 || (inode.PermissaoDono[1] && inode.Owner == usuarioAtual.Uid) || (inode.PermissaoGrupo[1] && inode.Group == usuarioAtual.Gid) )
        {
            permitido = true;   
        }
        else if(inode.PermissaoOutrosUsuario[1])
        {
            permitido = true;
        }

        return permitido;
    }

    static bool VerificaPermissaoLeitura(Inode inode)
    {

        bool permitido = false;
        //remover
        if (usuarioAtual.Uid == 1 || (inode.PermissaoDono[0] && inode.Owner == usuarioAtual.Uid) || (inode.PermissaoGrupo[0] && inode.Group == usuarioAtual.Gid))
        {
            permitido = true;
        }
        else if (inode.PermissaoOutrosUsuario[1])
        {
            permitido = true;
        }

        return permitido;
    }

    static bool VerificaPermissaoExecutar(Inode inode)
    {

        bool permitido = false;
        //remover
        if (usuarioAtual.Uid == 1 || (inode.PermissaoDono[3] && inode.Owner == usuarioAtual.Uid) || (inode.PermissaoGrupo[3] && inode.Group == usuarioAtual.Gid))
        {
            permitido = true;
        }
        else if (inode.PermissaoOutrosUsuario[1])
        {
            permitido = true;
        }

        return permitido;
    }

    static string VerificaPermissoes(Inode inode) {

        string permissoes = "";

        foreach (var item in inode.PermissaoDono)
        {
            permissoes = item ? permissoes + "v" : permissoes + "f";
        }
        foreach (var item in inode.PermissaoGrupo)
        {
            permissoes = item ? permissoes + "v" : permissoes + "f";
        }
        foreach (var item in inode.PermissaoOutrosUsuario)
        {
            permissoes = item ? permissoes + "v" : permissoes + "f";
        }

        permissoes += " ";

        return permissoes;
    }

    static User SelecionarUsuario()
    {
        Console.Clear();
        Console.WriteLine("Selecione o usuário: ");
        for (int i = 0; i < users.Count; i++)
        {
            Console.WriteLine(string.Concat(i , " - " , users[i].NomeUsuario));
        }
        int usuario = Convert.ToInt32(Console.ReadLine());

        Console.Write("Digite a senha: ");
        string senha = Console.ReadLine();

        return users[usuario];
    }

    static void ExibirMenu()
    {
        
        Console.WriteLine("========== Menu ==========");
        Console.WriteLine(" touch || gravar_conteudo || cat || rm || chown || chmod || mkdir || rmdir || cd || ls || adduser|| rmuser|| lsuser || trocar_user || blocos");
       // Console.WriteLine(" mkdir || rmdir || cd || ls || adduser|| rmuser|| lsuser");
        Console.WriteLine("==========================");
        Console.Write(String.Concat(NomeUsuarioAtual, ": ", diretorioAtual , "  "));
    }
}
 
