using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimArquivosUnix.Enum;
using static System.Net.Mime.MediaTypeNames;

namespace SimArquivosUnix
{
    class Inode
    {
        public int Id { get; set; }  
        public int IdPai { get; set; }
        public string Nome { get; set; }
        public int Owner { get; set; }
        public uint Group { get; set; }
        public int Size { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime LastStatusChangeTime { get; set; }
        
        public Inode[] BlockPointers { get; set; }
        public TipoInode TipoInode { get; set; }
        public string[] Conteudo { get; set; }

        public bool[] PermissaoDono;
        public bool[] PermissaoGrupo;
        public bool[] PermissaoOutrosUsuario;

        public string[] conteudo { get; set; }
        public Inode(int id,string nome,ushort mode, int owner, uint group, int size, TipoInode tipoInode, int idPai)
        {
            Id = id;
            Nome = nome;
            Owner = owner;
            Group = group;
            Size = size;
            idPai = idPai;
            LastAccessTime = DateTime.Now;
            LastModificationTime = DateTime.Now;
            LastStatusChangeTime = DateTime.Now;
            TipoInode = tipoInode;
            BlockPointers = new Inode[10];
           
            //ordem : leitura - escrita - executar
            PermissaoDono = new bool[3];
            PermissaoDono[0] = true;
            PermissaoDono[1] = true;
            PermissaoDono[2] = true;

            PermissaoGrupo = new bool[3];
            PermissaoGrupo[0] = true;
            PermissaoGrupo[1] = true;
            PermissaoGrupo[2] = true;

            PermissaoOutrosUsuario = new bool[3];
            PermissaoOutrosUsuario[0] = false;
            PermissaoOutrosUsuario[1] = false;
            PermissaoOutrosUsuario[2] = false;

        }

        public Inode()
        {

        }

        public int ProximaPosicaoInode() {

            for (int i = 0; i < BlockPointers.Length; i++)
            {
                if (BlockPointers[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public void PrintConteudo() {
            if (conteudo == null || conteudo.Length == 0)
            {
                Console.WriteLine("Arquivo vazio!");
                return;
            }
            for (int i = 0; i < conteudo.Length; i++)
            {
                Console.Write(conteudo[i] + " ");
            }
            Console.WriteLine();   
        }

    }

}
