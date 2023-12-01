using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimArquivosUnix
{
    internal class User
    {
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
        public int Uid { get; set; }
        public uint Gid { get; set; }
        public User(string nomeUsuario, string senha, int uid, uint gid, string diretorioInicial, string shellPadrao)
        {
            NomeUsuario = nomeUsuario;
            Senha = senha;
            Uid = uid;
            Gid = gid;

        }

        public User()
        {
           
        }

    }
}
